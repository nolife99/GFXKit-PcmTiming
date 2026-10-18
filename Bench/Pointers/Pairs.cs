using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;

namespace GFXKit.Benchmarks.Pointers;

using GFXKit.Media;

static unsafe class Pairs
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static void Split(float* at, nint stride, Vector256<float> pairs)
    {
        var samples = pairs.AsInt16();
        Avx.Store(at, S16.Widen(Avx2.MultiplyAddAdjacent(samples, Vector256.Create(1).AsInt16())));
        Avx.Store(at + stride, S16.Widen(Avx2.MultiplyAddAdjacent(samples, Vector256.Create(1 << 16).AsInt16())));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static Vector256<float> Join(float* at, nint stride) =>
        Avx2.Blend(S16.Round(Avx.LoadVector256(at)).AsInt16(), Avx2.ShiftLeftLogical(S16.Round(Avx.LoadVector256(at + stride)), 16).AsInt16(), 0xAA).AsSingle();

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public static void Deinterleave2(byte* data, int channels, float* output, nint stride, int frames)
    {
        var f = 0;
        for (; f + 16 <= frames; f += 16, data += 64)
        {
            Split(output + f, stride, Avx.LoadVector256((float*)data));
            Split(output + f + 8, stride, Avx.LoadVector256((float*)(data + 32)));
        }
        Deinterleave.Scalar<S16>(data, 2, output + f, stride, frames - f);
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public static void Deinterleave4(byte* data, int channels, float* output, nint stride, int frames)
    {
        var f = 0;
        for (; f + 8 <= frames; f += 8, data += 64)
        {
            Networks.Split2(Avx.LoadVector256((float*)data), Avx.LoadVector256((float*)(data + 32)), out var p0, out var p1);
            Split(output + f, stride, p0);
            Split(output + 2 * stride + f, stride, p1);
        }
        Deinterleave.Scalar<S16>(data, 4, output + f, stride, frames - f);
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public static void Deinterleave6(byte* data, int channels, float* output, nint stride, int frames)
    {
        var f = 0;
        for (; f + 8 <= frames; f += 8, data += 96)
        {
            Transposes.Split3(Avx.LoadVector256((float*)data), Avx.LoadVector256((float*)(data + 32)), Avx.LoadVector256((float*)(data + 64)), out var p0, out var p1, out var p2);
            Split(output + f, stride, p0);
            Split(output + 2 * stride + f, stride, p1);
            Split(output + 4 * stride + f, stride, p2);
        }
        Deinterleave.Scalar<S16>(data, 6, output + f, stride, frames - f);
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public static void Deinterleave8(byte* data, int channels, float* output, nint stride, int frames)
    {
        var f = 0;
        for (; f + 8 <= frames; f += 8, data += 128)
        {
            Networks.Split4(Avx.LoadVector256((float*)data), Avx.LoadVector256((float*)(data + 32)), Avx.LoadVector256((float*)(data + 64)),
                Avx.LoadVector256((float*)(data + 96)), out var p0, out var p1, out var p2, out var p3);
            Split(output + f, stride, p0);
            Split(output + 2 * stride + f, stride, p1);
            Split(output + 4 * stride + f, stride, p2);
            Split(output + 6 * stride + f, stride, p3);
        }
        Deinterleave.Scalar<S16>(data, 8, output + f, stride, frames - f);
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public static void Interleave2(float* input, nint stride, int channels, byte* data, int frames)
    {
        var f = 0;
        for (; f + 8 <= frames; f += 8, data += 32) Avx.Store((float*)data, Join(input + f, stride));
        Interleave.Scalar<S16>(input + f, stride, 2, data, frames - f);
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public static void Interleave4(float* input, nint stride, int channels, byte* data, int frames)
    {
        var f = 0;
        for (; f + 8 <= frames; f += 8, data += 64)
        {
            Networks.Merge2(Join(input + f, stride), Join(input + 2 * stride + f, stride), out var i0, out var i1);
            Avx.Store((float*)data, i0);
            Avx.Store((float*)(data + 32), i1);
        }
        Interleave.Scalar<S16>(input + f, stride, 4, data, frames - f);
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public static void Interleave6(float* input, nint stride, int channels, byte* data, int frames)
    {
        var f = 0;
        for (; f + 8 <= frames; f += 8, data += 96)
        {
            Transposes.Merge3(Join(input + f, stride), Join(input + 2 * stride + f, stride), Join(input + 4 * stride + f, stride), out var i0, out var i1, out var i2);
            Avx.Store((float*)data, i0);
            Avx.Store((float*)(data + 32), i1);
            Avx.Store((float*)(data + 64), i2);
        }
        Interleave.Scalar<S16>(input + f, stride, 6, data, frames - f);
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public static void Interleave8(float* input, nint stride, int channels, byte* data, int frames)
    {
        var f = 0;
        for (; f + 8 <= frames; f += 8, data += 128)
        {
            Networks.Merge4(Join(input + f, stride), Join(input + 2 * stride + f, stride), Join(input + 4 * stride + f, stride), Join(input + 6 * stride + f, stride),
                out var i0, out var i1, out var i2, out var i3);
            Avx.Store((float*)data, i0);
            Avx.Store((float*)(data + 32), i1);
            Avx.Store((float*)(data + 64), i2);
            Avx.Store((float*)(data + 96), i3);
        }
        Interleave.Scalar<S16>(input + f, stride, 8, data, frames - f);
    }
}
