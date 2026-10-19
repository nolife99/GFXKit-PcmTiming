using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;

namespace GFXKit.Media.Codecs;

static unsafe class SseDeinterleave
{
    public static delegate*<ReadOnlySpan<byte>, int, Span<float>, int, int, void> For<T>(int channels) where T : struct, IPcmSample
    {
        if (typeof(T) == typeof(S16))
            switch (channels)
            {
                case 2: return &Pairs2;
                case 4: return &Pairs4;
                case 6: return &Pairs6;
                case 8: return &Pairs8;
            }
        return channels switch
        {
            1 => &Mono<T>,
            2 => &Stereo<T>,
            3 => &Transposes.SseDeinterleave3<T>,
            4 => &Quad<T>,
            5 => &Transposes.SseDeinterleave5<T>,
            6 => &Transposes.SseDeinterleave6<T>,
            7 => &Transposes.SseDeinterleave7<T>,
            8 => &Octo<T>,
            _ => &Deinterleave.Scalar<T>
        };
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    static void Mono<T>(scoped ReadOnlySpan<byte> data, int channels, scoped Span<float> output, int stride, int frames) where T : struct, IPcmSample
    {
        if (typeof(T) == typeof(F32))
        {
            data[..(frames * 4)].CopyTo(MemoryMarshal.AsBytes(output[..frames]));
            return;
        }
        var bytes = default(T).Bytes;
        var source = new Interleaved(data, bytes, frames);
        var target = new Planes(output, stride, 1, frames);
        var f = 0;
        for (; f <= frames - 8; f += 8)
        {
            ref var block = ref source.At(f, 8);
            ref var into = ref target.At(f, 8);
            into.Store(stride, 0, 0, default(T).Read4(ref block, 0));
            into.Store(stride, 0, 4, default(T).Read4(ref block, 4 * bytes));
        }
        Deinterleave.Scalar<T>(data[(f * bytes)..], 1, output[f..], stride, frames - f);
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    static void Stereo<T>(scoped ReadOnlySpan<byte> data, int channels, scoped Span<float> output, int stride, int frames) where T : struct, IPcmSample
    {
        var bytes = default(T).Bytes;
        var source = new Interleaved(data, 2 * bytes, frames);
        var target = new Planes(output, stride, 2, frames);
        var f = 0;
        for (; f <= frames - 4; f += 4)
        {
            ref var block = ref source.At(f, 4);
            ref var into = ref target.At(f, 4);
            SseNetworks.Split2(default(T).Read4(ref block, 0), default(T).Read4(ref block, 4 * bytes), out var c0, out var c1);
            into.Store(stride, 0, 0, c0);
            into.Store(stride, 1, 0, c1);
        }
        Deinterleave.Scalar<T>(data[(f * 2 * bytes)..], 2, output[f..], stride, frames - f);
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    static void Quad<T>(scoped ReadOnlySpan<byte> data, int channels, scoped Span<float> output, int stride, int frames) where T : struct, IPcmSample
    {
        var bytes = default(T).Bytes;
        var source = new Interleaved(data, 4 * bytes, frames);
        var target = new Planes(output, stride, 4, frames);
        var f = 0;
        for (; f <= frames - 4; f += 4)
        {
            ref var block = ref source.At(f, 4);
            ref var into = ref target.At(f, 4);
            SseNetworks.Transpose(default(T).Read4(ref block, 0), default(T).Read4(ref block, 4 * bytes), default(T).Read4(ref block, 8 * bytes), default(T).Read4(ref block, 12 * bytes),
                out var c0, out var c1, out var c2, out var c3);
            into.Store(stride, 0, 0, c0);
            into.Store(stride, 1, 0, c1);
            into.Store(stride, 2, 0, c2);
            into.Store(stride, 3, 0, c3);
        }
        Deinterleave.Scalar<T>(data[(f * 4 * bytes)..], 4, output[f..], stride, frames - f);
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    static void Octo<T>(scoped ReadOnlySpan<byte> data, int channels, scoped Span<float> output, int stride, int frames) where T : struct, IPcmSample
    {
        var bytes = default(T).Bytes;
        var source = new Interleaved(data, 8 * bytes, frames);
        var target = new Planes(output, stride, 8, frames);
        var f = 0;
        for (; f <= frames - 4; f += 4)
        {
            ref var block = ref source.At(f, 4);
            ref var into = ref target.At(f, 4);
            SseNetworks.Transpose(default(T).Read4(ref block, 0), default(T).Read4(ref block, 8 * bytes), default(T).Read4(ref block, 16 * bytes), default(T).Read4(ref block, 24 * bytes),
                out var c0, out var c1, out var c2, out var c3);
            SseNetworks.Transpose(default(T).Read4(ref block, 4 * bytes), default(T).Read4(ref block, 12 * bytes), default(T).Read4(ref block, 20 * bytes), default(T).Read4(ref block, 28 * bytes),
                out var c4, out var c5, out var c6, out var c7);
            into.Store(stride, 0, 0, c0);
            into.Store(stride, 1, 0, c1);
            into.Store(stride, 2, 0, c2);
            into.Store(stride, 3, 0, c3);
            into.Store(stride, 4, 0, c4);
            into.Store(stride, 5, 0, c5);
            into.Store(stride, 6, 0, c6);
            into.Store(stride, 7, 0, c7);
        }
        Deinterleave.Scalar<T>(data[(f * 8 * bytes)..], 8, output[f..], stride, frames - f);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static Vector128<float> Load(ref byte block, int offset) => Packed.Read16(ref block, offset).AsSingle();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static void Split(ref float into, int stride, int channel, int frame, Vector128<float> pairs)
    {
        var samples = pairs.AsInt16();
        into.Store(stride, channel, frame, PcmScale.Scale(Sse2.MultiplyAddAdjacent(samples, Vector128.Create(1).AsInt16()), 1f / 32768));
        into.Store(stride, channel + 1, frame, PcmScale.Scale(Sse2.MultiplyAddAdjacent(samples, Vector128.Create(1 << 16).AsInt16()), 1f / 32768));
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    static void Pairs2(scoped ReadOnlySpan<byte> data, int channels, scoped Span<float> output, int stride, int frames)
    {
        var source = new Interleaved(data, 4, frames);
        var target = new Planes(output, stride, 2, frames);
        var f = 0;
        for (; f <= frames - 8; f += 8)
        {
            ref var block = ref source.At(f, 8);
            ref var into = ref target.At(f, 8);
            Split(ref into, stride, 0, 0, Load(ref block, 0));
            Split(ref into, stride, 0, 4, Load(ref block, 16));
        }
        Deinterleave.Scalar<S16>(data[(f * 4)..], 2, output[f..], stride, frames - f);
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    static void Pairs4(scoped ReadOnlySpan<byte> data, int channels, scoped Span<float> output, int stride, int frames)
    {
        var source = new Interleaved(data, 8, frames);
        var target = new Planes(output, stride, 4, frames);
        var f = 0;
        for (; f <= frames - 4; f += 4)
        {
            ref var block = ref source.At(f, 4);
            ref var into = ref target.At(f, 4);
            SseNetworks.Split2(Load(ref block, 0), Load(ref block, 16), out var p0, out var p1);
            Split(ref into, stride, 0, 0, p0);
            Split(ref into, stride, 2, 0, p1);
        }
        Deinterleave.Scalar<S16>(data[(f * 8)..], 4, output[f..], stride, frames - f);
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    static void Pairs6(scoped ReadOnlySpan<byte> data, int channels, scoped Span<float> output, int stride, int frames)
    {
        var source = new Interleaved(data, 12, frames);
        var target = new Planes(output, stride, 6, frames);
        var f = 0;
        for (; f <= frames - 4; f += 4)
        {
            ref var block = ref source.At(f, 4);
            ref var into = ref target.At(f, 4);
            Transposes.SseSplit3(Load(ref block, 0), Load(ref block, 16), Load(ref block, 32), out var p0, out var p1, out var p2);
            Split(ref into, stride, 0, 0, p0);
            Split(ref into, stride, 2, 0, p1);
            Split(ref into, stride, 4, 0, p2);
        }
        Deinterleave.Scalar<S16>(data[(f * 12)..], 6, output[f..], stride, frames - f);
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    static void Pairs8(scoped ReadOnlySpan<byte> data, int channels, scoped Span<float> output, int stride, int frames)
    {
        var source = new Interleaved(data, 16, frames);
        var target = new Planes(output, stride, 8, frames);
        var f = 0;
        for (; f <= frames - 4; f += 4)
        {
            ref var block = ref source.At(f, 4);
            ref var into = ref target.At(f, 4);
            SseNetworks.Transpose(Load(ref block, 0), Load(ref block, 16), Load(ref block, 32), Load(ref block, 48), out var p0, out var p1, out var p2, out var p3);
            Split(ref into, stride, 0, 0, p0);
            Split(ref into, stride, 2, 0, p1);
            Split(ref into, stride, 4, 0, p2);
            Split(ref into, stride, 6, 0, p3);
        }
        Deinterleave.Scalar<S16>(data[(f * 16)..], 8, output[f..], stride, frames - f);
    }
}

static unsafe class SseInterleave
{
    public static delegate*<ReadOnlySpan<float>, int, int, Span<byte>, int, void> For<T>(int channels) where T : struct, IPcmTarget
    {
        if (typeof(T) == typeof(S16))
            switch (channels)
            {
                case 2: return &Pairs2;
                case 4: return &Pairs4;
                case 6: return &Pairs6;
                case 8: return &Pairs8;
            }
        return channels switch
        {
            1 => &Mono<T>,
            2 => &Stereo<T>,
            3 => &Transposes.SseInterleave3<T>,
            4 => &Quad<T>,
            5 => &Transposes.SseInterleave5<T>,
            6 => &Transposes.SseInterleave6<T>,
            7 => &Transposes.SseInterleave7<T>,
            8 => &Octo<T>,
            _ => &Interleave.Scalar<T>
        };
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    static void Mono<T>(scoped ReadOnlySpan<float> input, int stride, int channels, scoped Span<byte> data, int frames) where T : struct, IPcmTarget
    {
        if (typeof(T) == typeof(F32))
        {
            MemoryMarshal.AsBytes(input[..frames]).CopyTo(data);
            return;
        }
        var bytes = default(T).Bytes;
        var source = new Planes(input, stride, 1, frames);
        var target = new InterleavedTarget(data, bytes, frames);
        var f = 0;
        for (; f <= frames - 8; f += 8)
        {
            ref var from = ref source.At(f, 8);
            ref var block = ref target.At(f, 8);
            default(T).Write4(ref block, 0, from.Load4(stride, 0, 0));
            default(T).Write4(ref block, 4 * bytes, from.Load4(stride, 0, 4));
        }
        Interleave.Scalar<T>(input[f..], stride, 1, data[(f * bytes)..], frames - f);
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    static void Stereo<T>(scoped ReadOnlySpan<float> input, int stride, int channels, scoped Span<byte> data, int frames) where T : struct, IPcmTarget
    {
        var bytes = default(T).Bytes;
        var source = new Planes(input, stride, 2, frames);
        var target = new InterleavedTarget(data, 2 * bytes, frames);
        var f = 0;
        for (; f <= frames - 4; f += 4)
        {
            ref var from = ref source.At(f, 4);
            ref var block = ref target.At(f, 4);
            var (l, r) = (from.Load4(stride, 0, 0), from.Load4(stride, 1, 0));
            default(T).Write4(ref block, 0, Sse.UnpackLow(l, r));
            default(T).Write4(ref block, 4 * bytes, Sse.UnpackHigh(l, r));
        }
        Interleave.Scalar<T>(input[f..], stride, 2, data[(f * 2 * bytes)..], frames - f);
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    static void Quad<T>(scoped ReadOnlySpan<float> input, int stride, int channels, scoped Span<byte> data, int frames) where T : struct, IPcmTarget
    {
        var bytes = default(T).Bytes;
        var source = new Planes(input, stride, 4, frames);
        var target = new InterleavedTarget(data, 4 * bytes, frames);
        var f = 0;
        for (; f <= frames - 4; f += 4)
        {
            ref var from = ref source.At(f, 4);
            ref var block = ref target.At(f, 4);
            SseNetworks.Transpose(from.Load4(stride, 0, 0), from.Load4(stride, 1, 0), from.Load4(stride, 2, 0), from.Load4(stride, 3, 0), out var i0, out var i1, out var i2, out var i3);
            default(T).Write4(ref block, 0, i0);
            default(T).Write4(ref block, 4 * bytes, i1);
            default(T).Write4(ref block, 8 * bytes, i2);
            default(T).Write4(ref block, 12 * bytes, i3);
        }
        Interleave.Scalar<T>(input[f..], stride, 4, data[(f * 4 * bytes)..], frames - f);
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    static void Octo<T>(scoped ReadOnlySpan<float> input, int stride, int channels, scoped Span<byte> data, int frames) where T : struct, IPcmTarget
    {
        var bytes = default(T).Bytes;
        var source = new Planes(input, stride, 8, frames);
        var target = new InterleavedTarget(data, 8 * bytes, frames);
        var f = 0;
        for (; f <= frames - 4; f += 4)
        {
            ref var from = ref source.At(f, 4);
            ref var block = ref target.At(f, 4);
            SseNetworks.Transpose(from.Load4(stride, 0, 0), from.Load4(stride, 1, 0), from.Load4(stride, 2, 0), from.Load4(stride, 3, 0), out var i0, out var i2, out var i4, out var i6);
            SseNetworks.Transpose(from.Load4(stride, 4, 0), from.Load4(stride, 5, 0), from.Load4(stride, 6, 0), from.Load4(stride, 7, 0), out var i1, out var i3, out var i5, out var i7);
            default(T).Write4(ref block, 0, i0);
            default(T).Write4(ref block, 4 * bytes, i1);
            default(T).Write4(ref block, 8 * bytes, i2);
            default(T).Write4(ref block, 12 * bytes, i3);
            default(T).Write4(ref block, 16 * bytes, i4);
            default(T).Write4(ref block, 20 * bytes, i5);
            default(T).Write4(ref block, 24 * bytes, i6);
            default(T).Write4(ref block, 28 * bytes, i7);
        }
        Interleave.Scalar<T>(input[f..], stride, 8, data[(f * 8 * bytes)..], frames - f);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static Vector128<byte> Join(ref float from, int stride, int channel) =>
        Sse41.Blend(PcmScale.Quantize(from.Load4(stride, channel, 0), 32767f / 32768, 32768).AsInt16(),
            (PcmScale.Quantize(from.Load4(stride, channel + 1, 0), 32767f / 32768, 32768) << 16).AsInt16(), 0xAA).AsByte();

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    static void Pairs2(scoped ReadOnlySpan<float> input, int stride, int channels, scoped Span<byte> data, int frames)
    {
        var source = new Planes(input, stride, 2, frames);
        var target = new InterleavedTarget(data, 4, frames);
        var f = 0;
        for (; f <= frames - 4; f += 4) Packed.Write16(ref target.At(f, 4), 0, Join(ref source.At(f, 4), stride, 0));
        Interleave.Scalar<S16>(input[f..], stride, 2, data[(f * 4)..], frames - f);
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    static void Pairs4(scoped ReadOnlySpan<float> input, int stride, int channels, scoped Span<byte> data, int frames)
    {
        var source = new Planes(input, stride, 4, frames);
        var target = new InterleavedTarget(data, 8, frames);
        var f = 0;
        for (; f <= frames - 4; f += 4)
        {
            ref var from = ref source.At(f, 4);
            ref var block = ref target.At(f, 4);
            var (p0, p1) = (Join(ref from, stride, 0).AsSingle(), Join(ref from, stride, 2).AsSingle());
            Packed.Write16(ref block, 0, Sse.UnpackLow(p0, p1).AsByte());
            Packed.Write16(ref block, 16, Sse.UnpackHigh(p0, p1).AsByte());
        }
        Interleave.Scalar<S16>(input[f..], stride, 4, data[(f * 8)..], frames - f);
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    static void Pairs6(scoped ReadOnlySpan<float> input, int stride, int channels, scoped Span<byte> data, int frames)
    {
        var source = new Planes(input, stride, 6, frames);
        var target = new InterleavedTarget(data, 12, frames);
        var f = 0;
        for (; f <= frames - 4; f += 4)
        {
            ref var from = ref source.At(f, 4);
            ref var block = ref target.At(f, 4);
            Transposes.SseMerge3(Join(ref from, stride, 0).AsSingle(), Join(ref from, stride, 2).AsSingle(), Join(ref from, stride, 4).AsSingle(), out var i0, out var i1, out var i2);
            Packed.Write16(ref block, 0, i0.AsByte());
            Packed.Write16(ref block, 16, i1.AsByte());
            Packed.Write16(ref block, 32, i2.AsByte());
        }
        Interleave.Scalar<S16>(input[f..], stride, 6, data[(f * 12)..], frames - f);
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    static void Pairs8(scoped ReadOnlySpan<float> input, int stride, int channels, scoped Span<byte> data, int frames)
    {
        var source = new Planes(input, stride, 8, frames);
        var target = new InterleavedTarget(data, 16, frames);
        var f = 0;
        for (; f <= frames - 4; f += 4)
        {
            ref var from = ref source.At(f, 4);
            ref var block = ref target.At(f, 4);
            SseNetworks.Transpose(Join(ref from, stride, 0).AsSingle(), Join(ref from, stride, 2).AsSingle(), Join(ref from, stride, 4).AsSingle(), Join(ref from, stride, 6).AsSingle(), out var i0, out var i1, out var i2, out var i3);
            Packed.Write16(ref block, 0, i0.AsByte());
            Packed.Write16(ref block, 16, i1.AsByte());
            Packed.Write16(ref block, 32, i2.AsByte());
            Packed.Write16(ref block, 48, i3.AsByte());
        }
        Interleave.Scalar<S16>(input[f..], stride, 8, data[(f * 16)..], frames - f);
    }
}

static class SseNetworks
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Split2(Vector128<float> i0, Vector128<float> i1, out Vector128<float> c0, out Vector128<float> c1)
    {
        c0 = Sse.Shuffle(i0, i1, 0x88);
        c1 = Sse.Shuffle(i0, i1, 0xDD);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Transpose(Vector128<float> i0, Vector128<float> i1, Vector128<float> i2, Vector128<float> i3,
        out Vector128<float> c0, out Vector128<float> c1, out Vector128<float> c2, out Vector128<float> c3)
    {
        var t0 = Sse.UnpackLow(i0, i1);
        var t1 = Sse.UnpackLow(i2, i3);
        var t2 = Sse.UnpackHigh(i0, i1);
        var t3 = Sse.UnpackHigh(i2, i3);
        c0 = Sse.MoveLowToHigh(t0, t1);
        c1 = Sse.MoveHighToLow(t1, t0);
        c2 = Sse.MoveLowToHigh(t2, t3);
        c3 = Sse.MoveHighToLow(t3, t2);
    }
}
