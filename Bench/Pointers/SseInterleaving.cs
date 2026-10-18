using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;

namespace GFXKit.Benchmarks.Pointers;

using GFXKit.Media;

static unsafe class SseDeinterleave
{
    public static delegate*<byte*, int, float*, nint, int, void> For<T>(int channels) where T : struct, IPcmSample
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
    static void Mono<T>(byte* data, int channels, float* output, nint stride, int frames) where T : struct, IPcmSample
    {
        if (typeof(T) == typeof(F32))
        {
            Buffer.MemoryCopy(data, output, (long)frames * 4, (long)frames * 4);
            return;
        }
        var bytes = default(T).Bytes;
        var f = 0;
        for (; f + 8 <= frames; f += 8, data += 8 * bytes)
        {
            Sse.Store(output + f, default(T).Read4(data));
            Sse.Store(output + f + 4, default(T).Read4(data + 4 * bytes));
        }
        Deinterleave.Scalar<T>(data, 1, output + f, stride, frames - f);
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    static void Stereo<T>(byte* data, int channels, float* output, nint stride, int frames) where T : struct, IPcmSample
    {
        var bytes = default(T).Bytes;
        var f = 0;
        for (; f + 4 <= frames; f += 4, data += 8 * bytes)
        {
            SseNetworks.Split2(default(T).Read4(data), default(T).Read4(data + 4 * bytes), out var c0, out var c1);
            Sse.Store(output + f, c0);
            Sse.Store(output + stride + f, c1);
        }
        Deinterleave.Scalar<T>(data, 2, output + f, stride, frames - f);
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    static void Quad<T>(byte* data, int channels, float* output, nint stride, int frames) where T : struct, IPcmSample
    {
        var bytes = default(T).Bytes;
        var f = 0;
        for (; f + 4 <= frames; f += 4, data += 16 * bytes)
        {
            SseNetworks.Transpose(default(T).Read4(data), default(T).Read4(data + 4 * bytes), default(T).Read4(data + 8 * bytes), default(T).Read4(data + 12 * bytes),
                out var c0, out var c1, out var c2, out var c3);
            Sse.Store(output + f, c0);
            Sse.Store(output + stride + f, c1);
            Sse.Store(output + 2 * stride + f, c2);
            Sse.Store(output + 3 * stride + f, c3);
        }
        Deinterleave.Scalar<T>(data, 4, output + f, stride, frames - f);
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    static void Octo<T>(byte* data, int channels, float* output, nint stride, int frames) where T : struct, IPcmSample
    {
        var bytes = default(T).Bytes;
        var f = 0;
        for (; f + 4 <= frames; f += 4, data += 32 * bytes)
        {
            SseNetworks.Transpose(default(T).Read4(data), default(T).Read4(data + 8 * bytes), default(T).Read4(data + 16 * bytes), default(T).Read4(data + 24 * bytes),
                out var c0, out var c1, out var c2, out var c3);
            SseNetworks.Transpose(default(T).Read4(data + 4 * bytes), default(T).Read4(data + 12 * bytes), default(T).Read4(data + 20 * bytes), default(T).Read4(data + 28 * bytes),
                out var c4, out var c5, out var c6, out var c7);
            var at = output + f;
            Sse.Store(at, c0);
            Sse.Store(at += stride, c1);
            Sse.Store(at += stride, c2);
            Sse.Store(at += stride, c3);
            Sse.Store(at += stride, c4);
            Sse.Store(at += stride, c5);
            Sse.Store(at += stride, c6);
            Sse.Store(at + stride, c7);
        }
        Deinterleave.Scalar<T>(data, 8, output + f, stride, frames - f);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static void Split(float* at, nint stride, Vector128<float> pairs)
    {
        var samples = pairs.AsInt16();
        Sse.Store(at, PcmScale.Scale(Sse2.MultiplyAddAdjacent(samples, Vector128.Create(1).AsInt16()), 1f / 32768));
        Sse.Store(at + stride, PcmScale.Scale(Sse2.MultiplyAddAdjacent(samples, Vector128.Create(1 << 16).AsInt16()), 1f / 32768));
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    static void Pairs2(byte* data, int channels, float* output, nint stride, int frames)
    {
        var f = 0;
        for (; f + 8 <= frames; f += 8, data += 32)
        {
            Split(output + f, stride, Sse.LoadVector128((float*)data));
            Split(output + f + 4, stride, Sse.LoadVector128((float*)(data + 16)));
        }
        Deinterleave.Scalar<S16>(data, 2, output + f, stride, frames - f);
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    static void Pairs4(byte* data, int channels, float* output, nint stride, int frames)
    {
        var f = 0;
        for (; f + 4 <= frames; f += 4, data += 32)
        {
            SseNetworks.Split2(Sse.LoadVector128((float*)data), Sse.LoadVector128((float*)(data + 16)), out var p0, out var p1);
            Split(output + f, stride, p0);
            Split(output + 2 * stride + f, stride, p1);
        }
        Deinterleave.Scalar<S16>(data, 4, output + f, stride, frames - f);
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    static void Pairs6(byte* data, int channels, float* output, nint stride, int frames)
    {
        var f = 0;
        for (; f + 4 <= frames; f += 4, data += 48)
        {
            Transposes.SseSplit3(Sse.LoadVector128((float*)data), Sse.LoadVector128((float*)(data + 16)), Sse.LoadVector128((float*)(data + 32)), out var p0, out var p1, out var p2);
            Split(output + f, stride, p0);
            Split(output + 2 * stride + f, stride, p1);
            Split(output + 4 * stride + f, stride, p2);
        }
        Deinterleave.Scalar<S16>(data, 6, output + f, stride, frames - f);
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    static void Pairs8(byte* data, int channels, float* output, nint stride, int frames)
    {
        var f = 0;
        for (; f + 4 <= frames; f += 4, data += 64)
        {
            SseNetworks.Transpose(Sse.LoadVector128((float*)data), Sse.LoadVector128((float*)(data + 16)), Sse.LoadVector128((float*)(data + 32)),
                Sse.LoadVector128((float*)(data + 48)), out var p0, out var p1, out var p2, out var p3);
            Split(output + f, stride, p0);
            Split(output + 2 * stride + f, stride, p1);
            Split(output + 4 * stride + f, stride, p2);
            Split(output + 6 * stride + f, stride, p3);
        }
        Deinterleave.Scalar<S16>(data, 8, output + f, stride, frames - f);
    }
}

static unsafe class SseInterleave
{
    public static delegate*<float*, nint, int, byte*, int, void> For<T>(int channels) where T : struct, IPcmTarget
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
    static void Mono<T>(float* input, nint stride, int channels, byte* data, int frames) where T : struct, IPcmTarget
    {
        if (typeof(T) == typeof(F32))
        {
            Buffer.MemoryCopy(input, data, (long)frames * 4, (long)frames * 4);
            return;
        }
        var bytes = default(T).Bytes;
        var f = 0;
        for (; f + 8 <= frames; f += 8, data += 8 * bytes)
        {
            default(T).Write4(data, Sse.LoadVector128(input + f));
            default(T).Write4(data + 4 * bytes, Sse.LoadVector128(input + f + 4));
        }
        Interleave.Scalar<T>(input + f, stride, 1, data, frames - f);
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    static void Stereo<T>(float* input, nint stride, int channels, byte* data, int frames) where T : struct, IPcmTarget
    {
        var bytes = default(T).Bytes;
        var f = 0;
        for (; f + 4 <= frames; f += 4, data += 8 * bytes)
        {
            var (l, r) = (Sse.LoadVector128(input + f), Sse.LoadVector128(input + stride + f));
            default(T).Write4(data, Sse.UnpackLow(l, r));
            default(T).Write4(data + 4 * bytes, Sse.UnpackHigh(l, r));
        }
        Interleave.Scalar<T>(input + f, stride, 2, data, frames - f);
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    static void Quad<T>(float* input, nint stride, int channels, byte* data, int frames) where T : struct, IPcmTarget
    {
        var bytes = default(T).Bytes;
        var f = 0;
        for (; f + 4 <= frames; f += 4, data += 16 * bytes)
        {
            SseNetworks.Transpose(Sse.LoadVector128(input + f), Sse.LoadVector128(input + stride + f), Sse.LoadVector128(input + 2 * stride + f),
                Sse.LoadVector128(input + 3 * stride + f), out var i0, out var i1, out var i2, out var i3);
            default(T).Write4(data, i0);
            default(T).Write4(data + 4 * bytes, i1);
            default(T).Write4(data + 8 * bytes, i2);
            default(T).Write4(data + 12 * bytes, i3);
        }
        Interleave.Scalar<T>(input + f, stride, 4, data, frames - f);
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    static void Octo<T>(float* input, nint stride, int channels, byte* data, int frames) where T : struct, IPcmTarget
    {
        var bytes = default(T).Bytes;
        var f = 0;
        for (; f + 4 <= frames; f += 4, data += 32 * bytes)
        {
            var at = input + f;
            SseNetworks.Transpose(Sse.LoadVector128(at), Sse.LoadVector128(at + stride), Sse.LoadVector128(at + 2 * stride), Sse.LoadVector128(at + 3 * stride),
                out var i0, out var i2, out var i4, out var i6);
            SseNetworks.Transpose(Sse.LoadVector128(at + 4 * stride), Sse.LoadVector128(at + 5 * stride), Sse.LoadVector128(at + 6 * stride), Sse.LoadVector128(at + 7 * stride),
                out var i1, out var i3, out var i5, out var i7);
            default(T).Write4(data, i0);
            default(T).Write4(data + 4 * bytes, i1);
            default(T).Write4(data + 8 * bytes, i2);
            default(T).Write4(data + 12 * bytes, i3);
            default(T).Write4(data + 16 * bytes, i4);
            default(T).Write4(data + 20 * bytes, i5);
            default(T).Write4(data + 24 * bytes, i6);
            default(T).Write4(data + 28 * bytes, i7);
        }
        Interleave.Scalar<T>(input + f, stride, 8, data, frames - f);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static Vector128<float> Join(float* at, nint stride) =>
        Sse41.Blend(PcmScale.Quantize(Sse.LoadVector128(at), 32767f / 32768, 32768).AsInt16(),
            Sse2.ShiftLeftLogical(PcmScale.Quantize(Sse.LoadVector128(at + stride), 32767f / 32768, 32768), 16).AsInt16(), 0xAA).AsSingle();

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    static void Pairs2(float* input, nint stride, int channels, byte* data, int frames)
    {
        var f = 0;
        for (; f + 4 <= frames; f += 4, data += 16) Sse.Store((float*)data, Join(input + f, stride));
        Interleave.Scalar<S16>(input + f, stride, 2, data, frames - f);
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    static void Pairs4(float* input, nint stride, int channels, byte* data, int frames)
    {
        var f = 0;
        for (; f + 4 <= frames; f += 4, data += 32)
        {
            var (p0, p1) = (Join(input + f, stride), Join(input + 2 * stride + f, stride));
            Sse.Store((float*)data, Sse.UnpackLow(p0, p1));
            Sse.Store((float*)(data + 16), Sse.UnpackHigh(p0, p1));
        }
        Interleave.Scalar<S16>(input + f, stride, 4, data, frames - f);
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    static void Pairs6(float* input, nint stride, int channels, byte* data, int frames)
    {
        var f = 0;
        for (; f + 4 <= frames; f += 4, data += 48)
        {
            Transposes.SseMerge3(Join(input + f, stride), Join(input + 2 * stride + f, stride), Join(input + 4 * stride + f, stride), out var i0, out var i1, out var i2);
            Sse.Store((float*)data, i0);
            Sse.Store((float*)(data + 16), i1);
            Sse.Store((float*)(data + 32), i2);
        }
        Interleave.Scalar<S16>(input + f, stride, 6, data, frames - f);
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    static void Pairs8(float* input, nint stride, int channels, byte* data, int frames)
    {
        var f = 0;
        for (; f + 4 <= frames; f += 4, data += 64)
        {
            SseNetworks.Transpose(Join(input + f, stride), Join(input + 2 * stride + f, stride), Join(input + 4 * stride + f, stride), Join(input + 6 * stride + f, stride),
                out var i0, out var i1, out var i2, out var i3);
            Sse.Store((float*)data, i0);
            Sse.Store((float*)(data + 16), i1);
            Sse.Store((float*)(data + 32), i2);
            Sse.Store((float*)(data + 48), i3);
        }
        Interleave.Scalar<S16>(input + f, stride, 8, data, frames - f);
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
