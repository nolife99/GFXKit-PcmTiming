using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;

namespace GFXKit.Media.Codecs;

static class Pairs
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static Vector256<float> Load(scoped ReadOnlySpan<byte> block, int offset) => Packed.Read32(block, offset).AsSingle();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static void Split(scoped Planes.Block into, int channel, int frame, Vector256<float> pairs)
    {
        var samples = pairs.AsInt16();
        into.Store(channel, frame, S16.Widen(Avx2.MultiplyAddAdjacent(samples, Vector256.Create(1).AsInt16())));
        into.Store(channel + 1, frame, S16.Widen(Avx2.MultiplyAddAdjacent(samples, Vector256.Create(1 << 16).AsInt16())));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static Vector256<byte> Join(scoped Planes.Block from, int channel) =>
        Avx2.Blend(S16.Round(from.Load8(channel, 0)).AsInt16(), Avx2.ShiftLeftLogical(S16.Round(from.Load8(channel + 1, 0)), 16).AsInt16(), 0xAA).AsByte();

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public static void Deinterleave2(scoped ReadOnlySpan<byte> data, int channels, scoped Span<float> output, int stride, int frames)
    {
        var source = new Interleaved(data, 4, frames);
        var target = new Planes(output, stride, 2, frames);
        var f = 0;
        for (; f <= frames - 16; f += 16)
        {
            var block = source.At(f, 16);
            var into = target.At(f, 16);
            Split(into, 0, 0, Load(block, 0));
            Split(into, 0, 8, Load(block, 32));
        }
        Deinterleave.Scalar<S16>(data[(f * 4)..], 2, output[f..], stride, frames - f);
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public static void Deinterleave4(scoped ReadOnlySpan<byte> data, int channels, scoped Span<float> output, int stride, int frames)
    {
        var source = new Interleaved(data, 8, frames);
        var target = new Planes(output, stride, 4, frames);
        var f = 0;
        for (; f <= frames - 8; f += 8)
        {
            var block = source.At(f, 8);
            var into = target.At(f, 8);
            Networks.Split2(Load(block, 0), Load(block, 32), out var p0, out var p1);
            Split(into, 0, 0, p0);
            Split(into, 2, 0, p1);
        }
        Deinterleave.Scalar<S16>(data[(f * 8)..], 4, output[f..], stride, frames - f);
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public static void Deinterleave6(scoped ReadOnlySpan<byte> data, int channels, scoped Span<float> output, int stride, int frames)
    {
        var source = new Interleaved(data, 12, frames);
        var target = new Planes(output, stride, 6, frames);
        var f = 0;
        for (; f <= frames - 8; f += 8)
        {
            var block = source.At(f, 8);
            var into = target.At(f, 8);
            Transposes.Split3(Load(block, 0), Load(block, 32), Load(block, 64), out var p0, out var p1, out var p2);
            Split(into, 0, 0, p0);
            Split(into, 2, 0, p1);
            Split(into, 4, 0, p2);
        }
        Deinterleave.Scalar<S16>(data[(f * 12)..], 6, output[f..], stride, frames - f);
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public static void Deinterleave8(scoped ReadOnlySpan<byte> data, int channels, scoped Span<float> output, int stride, int frames)
    {
        var source = new Interleaved(data, 16, frames);
        var target = new Planes(output, stride, 8, frames);
        var f = 0;
        for (; f <= frames - 8; f += 8)
        {
            var block = source.At(f, 8);
            var into = target.At(f, 8);
            Networks.Split4(Load(block, 0), Load(block, 32), Load(block, 64), Load(block, 96), out var p0, out var p1, out var p2, out var p3);
            Split(into, 0, 0, p0);
            Split(into, 2, 0, p1);
            Split(into, 4, 0, p2);
            Split(into, 6, 0, p3);
        }
        Deinterleave.Scalar<S16>(data[(f * 16)..], 8, output[f..], stride, frames - f);
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public static void Interleave2(scoped ReadOnlySpan<float> input, int stride, int channels, scoped Span<byte> data, int frames)
    {
        var source = new Planes(input, stride, 2, frames);
        var target = new InterleavedTarget(data, 4, frames);
        var f = 0;
        for (; f <= frames - 8; f += 8) Packed.Write32(target.At(f, 8), 0, Join(source.At(f, 8), 0));
        Interleave.Scalar<S16>(input[f..], stride, 2, data[(f * 4)..], frames - f);
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public static void Interleave4(scoped ReadOnlySpan<float> input, int stride, int channels, scoped Span<byte> data, int frames)
    {
        var source = new Planes(input, stride, 4, frames);
        var target = new InterleavedTarget(data, 8, frames);
        var f = 0;
        for (; f <= frames - 8; f += 8)
        {
            var from = source.At(f, 8);
            var block = target.At(f, 8);
            Networks.Merge2(Join(from, 0).AsSingle(), Join(from, 2).AsSingle(), out var i0, out var i1);
            Packed.Write32(block, 0, i0.AsByte());
            Packed.Write32(block, 32, i1.AsByte());
        }
        Interleave.Scalar<S16>(input[f..], stride, 4, data[(f * 8)..], frames - f);
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public static void Interleave6(scoped ReadOnlySpan<float> input, int stride, int channels, scoped Span<byte> data, int frames)
    {
        var source = new Planes(input, stride, 6, frames);
        var target = new InterleavedTarget(data, 12, frames);
        var f = 0;
        for (; f <= frames - 8; f += 8)
        {
            var from = source.At(f, 8);
            var block = target.At(f, 8);
            Transposes.Merge3(Join(from, 0).AsSingle(), Join(from, 2).AsSingle(), Join(from, 4).AsSingle(), out var i0, out var i1, out var i2);
            Packed.Write32(block, 0, i0.AsByte());
            Packed.Write32(block, 32, i1.AsByte());
            Packed.Write32(block, 64, i2.AsByte());
        }
        Interleave.Scalar<S16>(input[f..], stride, 6, data[(f * 12)..], frames - f);
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public static void Interleave8(scoped ReadOnlySpan<float> input, int stride, int channels, scoped Span<byte> data, int frames)
    {
        var source = new Planes(input, stride, 8, frames);
        var target = new InterleavedTarget(data, 16, frames);
        var f = 0;
        for (; f <= frames - 8; f += 8)
        {
            var from = source.At(f, 8);
            var block = target.At(f, 8);
            Networks.Merge4(Join(from, 0).AsSingle(), Join(from, 2).AsSingle(), Join(from, 4).AsSingle(), Join(from, 6).AsSingle(), out var i0, out var i1, out var i2, out var i3);
            Packed.Write32(block, 0, i0.AsByte());
            Packed.Write32(block, 32, i1.AsByte());
            Packed.Write32(block, 64, i2.AsByte());
            Packed.Write32(block, 96, i3.AsByte());
        }
        Interleave.Scalar<S16>(input[f..], stride, 8, data[(f * 16)..], frames - f);
    }
}
