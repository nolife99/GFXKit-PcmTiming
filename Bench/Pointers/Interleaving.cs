using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.Arm;
using System.Runtime.Intrinsics.X86;

namespace GFXKit.Benchmarks.Pointers;

using GFXKit.Media;

static unsafe class Deinterleave
{
    public static delegate*<byte*, int, float*, nint, int, void> For<T>(int channels) where T : struct, IPcmSample
    {
        if (!Avx2.IsSupported) return AdvSimd.Arm64.IsSupported ? NeonDeinterleave.For<T>(channels) : Sse41.IsSupported ? SseDeinterleave.For<T>(channels) : &Scalar<T>;
        if (typeof(T) == typeof(S24) && channels == 2) return &Stereo24;
        if (typeof(T) == typeof(S16))
            switch (channels)
            {
                case 2: return &Pairs.Deinterleave2;
                case 4: return &Pairs.Deinterleave4;
                case 6: return &Pairs.Deinterleave6;
                case 8: return &Pairs.Deinterleave8;
            }
        return channels switch
        {
            1 => &Mono<T>,
            2 => &Stereo<T>,
            3 => &Transposes.Deinterleave3<T>,
            4 => &Quad<T>,
            5 => &Transposes.Deinterleave5<T>,
            6 => &Transposes.Deinterleave6<T>,
            7 => &Transposes.Deinterleave7<T>,
            8 => &Octo<T>,
            _ => &Scalar<T>
        };
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public static void Scalar<T>(byte* data, int channels, float* output, nint stride, int frames) where T : struct, IPcmSample
    {
        var bytes = default(T).Bytes;
        for (var f = 0; f < frames; f++)
        {
            var at = output + f;
            for (var c = 0; c < channels; c++, data += bytes, at += stride) *at = default(T).Read(data);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    static void Mono<T>(byte* data, int channels, float* output, nint stride, int frames) where T : struct, IPcmSample
    {
        var bytes = default(T).Bytes;
        if (typeof(T) == typeof(F32))
        {
            Buffer.MemoryCopy(data, output, (long)frames * 4, (long)frames * 4);
            return;
        }
        var f = 0;
        for (; f + 32 <= frames; f += 32, data += 32 * bytes)
        {
            Avx.Store(output + f, default(T).Read8(data));
            Avx.Store(output + f + 8, default(T).Read8(data + 8 * bytes));
            Avx.Store(output + f + 16, default(T).Read8(data + 16 * bytes));
            Avx.Store(output + f + 24, default(T).Read8(data + 24 * bytes));
        }
        for (; f + 8 <= frames; f += 8, data += 8 * bytes) Avx.Store(output + f, default(T).Read8(data));
        Scalar<T>(data, 1, output + f, stride, frames - f);
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    static void Stereo<T>(byte* data, int channels, float* output, nint stride, int frames) where T : struct, IPcmSample
    {
        var bytes = default(T).Bytes;
        var f = 0;
        for (; f + 8 <= frames; f += 8, data += 16 * bytes)
        {
            Networks.Split2(default(T).Read8(data), default(T).Read8(data + 8 * bytes), out var c0, out var c1);
            Avx.Store(output + f, c0);
            Avx.Store(output + stride + f, c1);
        }
        Scalar<T>(data, 2, output + f, stride, frames - f);
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    static void Stereo24(byte* data, int channels, float* output, nint stride, int frames)
    {
        var first = Vector256.Create(
            (byte)0x80, 0, 1, 2, 0x80, 6, 7, 8, 0x80, 3, 4, 5, 0x80, 9, 10, 11,
            0x80, 0, 1, 2, 0x80, 6, 7, 8, 0x80, 3, 4, 5, 0x80, 9, 10, 11);
        var second = Vector256.Create(
            (byte)0x80, 0, 1, 2, 0x80, 6, 7, 8, 0x80, 3, 4, 5, 0x80, 9, 10, 11,
            0x80, 4, 5, 6, 0x80, 10, 11, 12, 0x80, 7, 8, 9, 0x80, 13, 14, 15);
        var f = 0;
        for (; f + 8 <= frames; f += 8, data += 48)
        {
            var even = Avx2.Shuffle(Vector256.Create(Sse2.LoadVector128(data), Sse2.LoadVector128(data + 24)), first).AsInt64();
            var odd = Avx2.Shuffle(Vector256.Create(Sse2.LoadVector128(data + 12), Sse2.LoadVector128(data + 32)), second).AsInt64();
            Avx.Store(output + f, PcmScale.Times(Avx2.UnpackLow(even, odd).AsInt32(), 1f / 2147483648));
            Avx.Store(output + stride + f, PcmScale.Times(Avx2.UnpackHigh(even, odd).AsInt32(), 1f / 2147483648));
        }
        Scalar<S24>(data, 2, output + f, stride, frames - f);
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    static void Quad<T>(byte* data, int channels, float* output, nint stride, int frames) where T : struct, IPcmSample
    {
        var bytes = default(T).Bytes;
        var f = 0;
        for (; f + 8 <= frames; f += 8, data += 32 * bytes)
        {
            Networks.Split4(default(T).Read8(data), default(T).Read8(data + 8 * bytes), default(T).Read8(data + 16 * bytes), default(T).Read8(data + 24 * bytes),
                out var c0, out var c1, out var c2, out var c3);
            Avx.Store(output + f, c0);
            Avx.Store(output + stride + f, c1);
            Avx.Store(output + 2 * stride + f, c2);
            Avx.Store(output + 3 * stride + f, c3);
        }
        Scalar<T>(data, 4, output + f, stride, frames - f);
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    static void Octo<T>(byte* data, int channels, float* output, nint stride, int frames) where T : struct, IPcmSample
    {
        var bytes = default(T).Bytes;
        var f = 0;
        for (; f + 8 <= frames; f += 8, data += 64 * bytes)
        {
            Networks.Split8(
                default(T).Read8(data), default(T).Read8(data + 8 * bytes), default(T).Read8(data + 16 * bytes), default(T).Read8(data + 24 * bytes),
                default(T).Read8(data + 32 * bytes), default(T).Read8(data + 40 * bytes), default(T).Read8(data + 48 * bytes), default(T).Read8(data + 56 * bytes),
                out var c0, out var c1, out var c2, out var c3, out var c4, out var c5, out var c6, out var c7);
            var at = output + f;
            Avx.Store(at, c0);
            Avx.Store(at += stride, c1);
            Avx.Store(at += stride, c2);
            Avx.Store(at += stride, c3);
            Avx.Store(at += stride, c4);
            Avx.Store(at += stride, c5);
            Avx.Store(at += stride, c6);
            Avx.Store(at + stride, c7);
        }
        Scalar<T>(data, 8, output + f, stride, frames - f);
    }
}

static unsafe class Interleave
{
    public static void Write<T>(scoped PlanarSpan input, scoped Span<byte> data) where T : struct, IPcmTarget
    {
        var need = (long)input.Frames * input.Channels * default(T).Bytes;
        if (data.Length < need) throw new ArgumentException($"{input.Frames} frames of {input.Channels} channels take {need} bytes, and the destination holds {data.Length}", nameof(data));
        fixed (float* samples = input.Storage)
        fixed (byte* bytes = data)
            For<T>(input.Channels)(samples, input.Stride, input.Channels, bytes, input.Frames);
    }

    public static delegate*<float*, nint, int, byte*, int, void> For<T>(int channels) where T : struct, IPcmTarget
    {
        if (!Avx2.IsSupported) return AdvSimd.Arm64.IsSupported ? NeonInterleave.For<T>(channels) : Sse41.IsSupported ? SseInterleave.For<T>(channels) : &Scalar<T>;
        if (typeof(T) == typeof(S16))
            switch (channels)
            {
                case 2: return &Pairs.Interleave2;
                case 4: return &Pairs.Interleave4;
                case 6: return &Pairs.Interleave6;
                case 8: return &Pairs.Interleave8;
            }
        return channels switch
        {
            1 => &Mono<T>,
            2 => &Stereo<T>,
            3 => &Transposes.Interleave3<T>,
            4 => &Quad<T>,
            5 => &Transposes.Interleave5<T>,
            6 => &Transposes.Interleave6<T>,
            7 => &Transposes.Interleave7<T>,
            8 => &Octo<T>,
            _ => &Scalar<T>
        };
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public static void Scalar<T>(float* input, nint stride, int channels, byte* data, int frames) where T : struct, IPcmTarget
    {
        var bytes = default(T).Bytes;
        for (var f = 0; f < frames; f++)
        {
            var at = input + f;
            for (var c = 0; c < channels; c++, data += bytes, at += stride) default(T).Write(data, *at);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    static void Mono<T>(float* input, nint stride, int channels, byte* data, int frames) where T : struct, IPcmTarget
    {
        var bytes = default(T).Bytes;
        if (typeof(T) == typeof(F32))
        {
            Buffer.MemoryCopy(input, data, (long)frames * 4, (long)frames * 4);
            return;
        }
        var f = 0;
        for (; f + 32 <= frames; f += 32, data += 32 * bytes)
        {
            default(T).Write8(data, Avx.LoadVector256(input + f));
            default(T).Write8(data + 8 * bytes, Avx.LoadVector256(input + f + 8));
            default(T).Write8(data + 16 * bytes, Avx.LoadVector256(input + f + 16));
            default(T).Write8(data + 24 * bytes, Avx.LoadVector256(input + f + 24));
        }
        for (; f + 8 <= frames; f += 8, data += 8 * bytes) default(T).Write8(data, Avx.LoadVector256(input + f));
        Scalar<T>(input + f, stride, 1, data, frames - f);
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    static void Stereo<T>(float* input, nint stride, int channels, byte* data, int frames) where T : struct, IPcmTarget
    {
        var bytes = default(T).Bytes;
        var f = 0;
        for (; f + 8 <= frames; f += 8, data += 16 * bytes)
        {
            Networks.Merge2(Avx.LoadVector256(input + f), Avx.LoadVector256(input + stride + f), out var i0, out var i1);
            default(T).Write8(data, i0);
            default(T).Write8(data + 8 * bytes, i1);
        }
        Scalar<T>(input + f, stride, 2, data, frames - f);
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    static void Quad<T>(float* input, nint stride, int channels, byte* data, int frames) where T : struct, IPcmTarget
    {
        var bytes = default(T).Bytes;
        var f = 0;
        for (; f + 8 <= frames; f += 8, data += 32 * bytes)
        {
            Networks.Merge4(Avx.LoadVector256(input + f), Avx.LoadVector256(input + stride + f), Avx.LoadVector256(input + 2 * stride + f),
                Avx.LoadVector256(input + 3 * stride + f), out var i0, out var i1, out var i2, out var i3);
            default(T).Write8(data, i0);
            default(T).Write8(data + 8 * bytes, i1);
            default(T).Write8(data + 16 * bytes, i2);
            default(T).Write8(data + 24 * bytes, i3);
        }
        Scalar<T>(input + f, stride, 4, data, frames - f);
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    static void Octo<T>(float* input, nint stride, int channels, byte* data, int frames) where T : struct, IPcmTarget
    {
        var bytes = default(T).Bytes;
        var f = 0;
        for (; f + 8 <= frames; f += 8, data += 64 * bytes)
        {
            var at = input + f;
            Networks.Split8(
                Avx.LoadVector256(at), Avx.LoadVector256(at + stride), Avx.LoadVector256(at + 2 * stride), Avx.LoadVector256(at + 3 * stride),
                Avx.LoadVector256(at + 4 * stride), Avx.LoadVector256(at + 5 * stride), Avx.LoadVector256(at + 6 * stride), Avx.LoadVector256(at + 7 * stride),
                out var f0, out var f1, out var f2, out var f3, out var f4, out var f5, out var f6, out var f7);
            default(T).Write8(data, f0);
            default(T).Write8(data + 8 * bytes, f1);
            default(T).Write8(data + 16 * bytes, f2);
            default(T).Write8(data + 24 * bytes, f3);
            default(T).Write8(data + 32 * bytes, f4);
            default(T).Write8(data + 40 * bytes, f5);
            default(T).Write8(data + 48 * bytes, f6);
            default(T).Write8(data + 56 * bytes, f7);
        }
        Scalar<T>(input + f, stride, 8, data, frames - f);
    }
}

static class Networks
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Split2(Vector256<float> i0, Vector256<float> i1, out Vector256<float> c0, out Vector256<float> c1)
    {
        c0 = Avx2.Permute4x64(Avx.Shuffle(i0, i1, 0x88).AsDouble(), 0xD8).AsSingle();
        c1 = Avx2.Permute4x64(Avx.Shuffle(i0, i1, 0xDD).AsDouble(), 0xD8).AsSingle();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Merge2(Vector256<float> c0, Vector256<float> c1, out Vector256<float> i0, out Vector256<float> i1)
    {
        var low = Avx.UnpackLow(c0, c1);
        var high = Avx.UnpackHigh(c0, c1);
        i0 = Avx.Permute2x128(low, high, 0x20);
        i1 = Avx.Permute2x128(low, high, 0x31);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Split4(Vector256<float> i0, Vector256<float> i1, Vector256<float> i2, Vector256<float> i3,
        out Vector256<float> c0, out Vector256<float> c1, out Vector256<float> c2, out Vector256<float> c3)
    {
        var a = Avx.Permute2x128(i0, i2, 0x20);
        var b = Avx.Permute2x128(i0, i2, 0x31);
        var c = Avx.Permute2x128(i1, i3, 0x20);
        var d = Avx.Permute2x128(i1, i3, 0x31);
        var t0 = Avx.UnpackLow(a, b).AsDouble();
        var t1 = Avx.UnpackHigh(a, b).AsDouble();
        var t2 = Avx.UnpackLow(c, d).AsDouble();
        var t3 = Avx.UnpackHigh(c, d).AsDouble();
        c0 = Avx.UnpackLow(t0, t2).AsSingle();
        c1 = Avx.UnpackHigh(t0, t2).AsSingle();
        c2 = Avx.UnpackLow(t1, t3).AsSingle();
        c3 = Avx.UnpackHigh(t1, t3).AsSingle();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Merge4(Vector256<float> c0, Vector256<float> c1, Vector256<float> c2, Vector256<float> c3,
        out Vector256<float> i0, out Vector256<float> i1, out Vector256<float> i2, out Vector256<float> i3)
    {
        var t0 = Avx.UnpackLow(c0, c1).AsDouble();
        var t1 = Avx.UnpackHigh(c0, c1).AsDouble();
        var t2 = Avx.UnpackLow(c2, c3).AsDouble();
        var t3 = Avx.UnpackHigh(c2, c3).AsDouble();
        var a = Avx.UnpackLow(t0, t2).AsSingle();
        var b = Avx.UnpackHigh(t0, t2).AsSingle();
        var c = Avx.UnpackLow(t1, t3).AsSingle();
        var d = Avx.UnpackHigh(t1, t3).AsSingle();
        i0 = Avx.Permute2x128(a, b, 0x20);
        i1 = Avx.Permute2x128(c, d, 0x20);
        i2 = Avx.Permute2x128(a, b, 0x31);
        i3 = Avx.Permute2x128(c, d, 0x31);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Split8(Vector256<float> i0, Vector256<float> i1, Vector256<float> i2, Vector256<float> i3,
        Vector256<float> i4, Vector256<float> i5, Vector256<float> i6, Vector256<float> i7,
        out Vector256<float> c0, out Vector256<float> c1, out Vector256<float> c2, out Vector256<float> c3,
        out Vector256<float> c4, out Vector256<float> c5, out Vector256<float> c6, out Vector256<float> c7)
    {
        var t0 = Avx.UnpackLow(i0, i1).AsDouble();
        var t1 = Avx.UnpackHigh(i0, i1).AsDouble();
        var t2 = Avx.UnpackLow(i2, i3).AsDouble();
        var t3 = Avx.UnpackHigh(i2, i3).AsDouble();
        var t4 = Avx.UnpackLow(i4, i5).AsDouble();
        var t5 = Avx.UnpackHigh(i4, i5).AsDouble();
        var t6 = Avx.UnpackLow(i6, i7).AsDouble();
        var t7 = Avx.UnpackHigh(i6, i7).AsDouble();
        var s0 = Avx.UnpackLow(t0, t2).AsSingle();
        var s1 = Avx.UnpackHigh(t0, t2).AsSingle();
        var s2 = Avx.UnpackLow(t1, t3).AsSingle();
        var s3 = Avx.UnpackHigh(t1, t3).AsSingle();
        var s4 = Avx.UnpackLow(t4, t6).AsSingle();
        var s5 = Avx.UnpackHigh(t4, t6).AsSingle();
        var s6 = Avx.UnpackLow(t5, t7).AsSingle();
        var s7 = Avx.UnpackHigh(t5, t7).AsSingle();
        c0 = Avx.Permute2x128(s0, s4, 0x20);
        c1 = Avx.Permute2x128(s1, s5, 0x20);
        c2 = Avx.Permute2x128(s2, s6, 0x20);
        c3 = Avx.Permute2x128(s3, s7, 0x20);
        c4 = Avx.Permute2x128(s0, s4, 0x31);
        c5 = Avx.Permute2x128(s1, s5, 0x31);
        c6 = Avx.Permute2x128(s2, s6, 0x31);
        c7 = Avx.Permute2x128(s3, s7, 0x31);
    }
}
