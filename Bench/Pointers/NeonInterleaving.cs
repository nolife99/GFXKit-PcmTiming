using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.Arm;

namespace GFXKit.Benchmarks.Pointers;

using GFXKit.Media;

static unsafe class NeonDeinterleave
{
    public static delegate*<byte*, int, float*, nint, int, void> For<T>(int channels) where T : struct, IPcmSample
    {
        if (typeof(T) == typeof(S24)) return channels switch { 1 => &Mono24, 2 => &Stereo24, _ => &Deinterleave.Scalar<T> };
        return channels switch
        {
            1 => &Mono<T>,
            2 => &Split2<T>,
            3 => &Split3<T>,
            4 => &Split4<T>,
            6 when default(T).Bytes <= 4 => &Split6<T>,
            8 when default(T).Bytes <= 4 => &Split8<T>,
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
        var lanes = default(T).Lanes;
        var f = 0;
        for (; f + 2 * lanes <= frames; f += 2 * lanes, data += 32)
        {
            var (a, b) = AdvSimd.Arm64.Load2xVector128(data);
            default(T).Widen(a, output + f);
            default(T).Widen(b, output + f + lanes);
        }
        for (; f + lanes <= frames; f += lanes, data += 16) default(T).Widen(AdvSimd.LoadVector128(data), output + f);
        Deinterleave.Scalar<T>(data, 1, output + f, stride, frames - f);
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    static void Split2<T>(byte* data, int channels, float* output, nint stride, int frames) where T : struct, IPcmSample
    {
        var lanes = default(T).Lanes;
        var f = 0;
        for (; f + lanes <= frames; f += lanes, data += 32)
        {
            var (a, b) = Neon.Load2(data, default(T).Bytes);
            default(T).Widen(a, output + f);
            default(T).Widen(b, output + stride + f);
        }
        Deinterleave.Scalar<T>(data, 2, output + f, stride, frames - f);
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    static void Split3<T>(byte* data, int channels, float* output, nint stride, int frames) where T : struct, IPcmSample
    {
        var lanes = default(T).Lanes;
        var f = 0;
        for (; f + lanes <= frames; f += lanes, data += 48)
        {
            var (a, b, c) = Neon.Load3(data, default(T).Bytes);
            default(T).Widen(a, output + f);
            default(T).Widen(b, output + stride + f);
            default(T).Widen(c, output + 2 * stride + f);
        }
        Deinterleave.Scalar<T>(data, 3, output + f, stride, frames - f);
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    static void Split4<T>(byte* data, int channels, float* output, nint stride, int frames) where T : struct, IPcmSample
    {
        var lanes = default(T).Lanes;
        var f = 0;
        for (; f + lanes <= frames; f += lanes, data += 64)
        {
            var (a, b, c, d) = Neon.Load4(data, default(T).Bytes);
            default(T).Widen(a, output + f);
            default(T).Widen(b, output + stride + f);
            default(T).Widen(c, output + 2 * stride + f);
            default(T).Widen(d, output + 3 * stride + f);
        }
        Deinterleave.Scalar<T>(data, 4, output + f, stride, frames - f);
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    static void Split6<T>(byte* data, int channels, float* output, nint stride, int frames) where T : struct, IPcmSample
    {
        var bytes = default(T).Bytes;
        var lanes = default(T).Lanes;
        var f = 0;
        for (; f + lanes <= frames; f += lanes, data += 96)
        {
            var (a0, a1, a2) = Neon.Load3(data, 2 * bytes);
            var (b0, b1, b2) = Neon.Load3(data + 48, 2 * bytes);
            default(T).Widen(Neon.UnzipEven(a0, b0, bytes), output + f);
            default(T).Widen(Neon.UnzipOdd(a0, b0, bytes), output + stride + f);
            default(T).Widen(Neon.UnzipEven(a1, b1, bytes), output + 2 * stride + f);
            default(T).Widen(Neon.UnzipOdd(a1, b1, bytes), output + 3 * stride + f);
            default(T).Widen(Neon.UnzipEven(a2, b2, bytes), output + 4 * stride + f);
            default(T).Widen(Neon.UnzipOdd(a2, b2, bytes), output + 5 * stride + f);
        }
        Deinterleave.Scalar<T>(data, 6, output + f, stride, frames - f);
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    static void Split8<T>(byte* data, int channels, float* output, nint stride, int frames) where T : struct, IPcmSample
    {
        var bytes = default(T).Bytes;
        var lanes = default(T).Lanes;
        var f = 0;
        for (; f + lanes <= frames; f += lanes, data += 128)
        {
            var (a0, a1, a2, a3) = Neon.Load4(data, 2 * bytes);
            var (b0, b1, b2, b3) = Neon.Load4(data + 64, 2 * bytes);
            default(T).Widen(Neon.UnzipEven(a0, b0, bytes), output + f);
            default(T).Widen(Neon.UnzipOdd(a0, b0, bytes), output + stride + f);
            default(T).Widen(Neon.UnzipEven(a1, b1, bytes), output + 2 * stride + f);
            default(T).Widen(Neon.UnzipOdd(a1, b1, bytes), output + 3 * stride + f);
            default(T).Widen(Neon.UnzipEven(a2, b2, bytes), output + 4 * stride + f);
            default(T).Widen(Neon.UnzipOdd(a2, b2, bytes), output + 5 * stride + f);
            default(T).Widen(Neon.UnzipEven(a3, b3, bytes), output + 6 * stride + f);
            default(T).Widen(Neon.UnzipOdd(a3, b3, bytes), output + 7 * stride + f);
        }
        Deinterleave.Scalar<T>(data, 8, output + f, stride, frames - f);
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    static void Mono24(byte* data, int channels, float* output, nint stride, int frames)
    {
        var f = 0;
        for (; f + 16 <= frames; f += 16, data += 48)
        {
            var (b0, b1, b2) = AdvSimd.Arm64.Load3xVector128AndUnzip(data);
            var low = AdvSimd.Arm64.ZipLow(Vector128<byte>.Zero, b0).AsUInt16();
            var high = AdvSimd.Arm64.ZipHigh(Vector128<byte>.Zero, b0).AsUInt16();
            var top = AdvSimd.Arm64.ZipLow(b1, b2).AsUInt16();
            var bottom = AdvSimd.Arm64.ZipHigh(b1, b2).AsUInt16();
            Neon.Store24(AdvSimd.Arm64.ZipLow(low, top), output + f);
            Neon.Store24(AdvSimd.Arm64.ZipHigh(low, top), output + f + 4);
            Neon.Store24(AdvSimd.Arm64.ZipLow(high, bottom), output + f + 8);
            Neon.Store24(AdvSimd.Arm64.ZipHigh(high, bottom), output + f + 12);
        }
        Deinterleave.Scalar<S24>(data, 1, output + f, stride, frames - f);
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    static void Stereo24(byte* data, int channels, float* output, nint stride, int frames)
    {
        var right = output + stride;
        var f = 0;
        for (; f + 8 <= frames; f += 8, data += 48)
        {
            var (h0, h1, h2) = AdvSimd.Arm64.Load3xVector128AndUnzip((ushort*)data);
            var lower = AdvSimd.ShiftLeftLogical(h0, 8);
            var upper = AdvSimd.ShiftLeftAndInsert(AdvSimd.ShiftRightLogical(h0, 8), h1, 8);
            var start = AdvSimd.And(h1, Vector128.Create((ushort)0xFF00));
            Neon.Store24(AdvSimd.Arm64.ZipLow(lower, upper), output + f);
            Neon.Store24(AdvSimd.Arm64.ZipHigh(lower, upper), output + f + 4);
            Neon.Store24(AdvSimd.Arm64.ZipLow(start, h2), right + f);
            Neon.Store24(AdvSimd.Arm64.ZipHigh(start, h2), right + f + 4);
        }
        Deinterleave.Scalar<S24>(data, 2, output + f, stride, frames - f);
    }
}

static unsafe class NeonInterleave
{
    public static delegate*<float*, nint, int, byte*, int, void> For<T>(int channels) where T : struct, IPcmTarget
    {
        if (typeof(T) == typeof(S24)) return &Interleave.Scalar<T>;
        return channels switch { 1 => &Mono<T>, 2 => &Merge2<T>, 3 => &Merge3<T>, 4 => &Merge4<T>, 6 => &Merge6<T>, 8 => &Merge8<T>, _ => &Interleave.Scalar<T> };
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    static void Mono<T>(float* input, nint stride, int channels, byte* data, int frames) where T : struct, IPcmTarget
    {
        if (typeof(T) == typeof(F32))
        {
            Buffer.MemoryCopy(input, data, (long)frames * 4, (long)frames * 4);
            return;
        }
        var lanes = default(T).Lanes;
        var f = 0;
        for (; f + lanes <= frames; f += lanes, data += 16) AdvSimd.Store(data, default(T).Narrow(input + f));
        Interleave.Scalar<T>(input + f, stride, 1, data, frames - f);
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    static void Merge2<T>(float* input, nint stride, int channels, byte* data, int frames) where T : struct, IPcmTarget
    {
        var lanes = default(T).Lanes;
        var f = 0;
        for (; f + lanes <= frames; f += lanes, data += 32) Neon.Store2(data, default(T).Bytes, default(T).Narrow(input + f), default(T).Narrow(input + stride + f));
        Interleave.Scalar<T>(input + f, stride, 2, data, frames - f);
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    static void Merge3<T>(float* input, nint stride, int channels, byte* data, int frames) where T : struct, IPcmTarget
    {
        var lanes = default(T).Lanes;
        var f = 0;
        for (; f + lanes <= frames; f += lanes, data += 48)
            Neon.Store3(data, default(T).Bytes, default(T).Narrow(input + f), default(T).Narrow(input + stride + f), default(T).Narrow(input + 2 * stride + f));
        Interleave.Scalar<T>(input + f, stride, 3, data, frames - f);
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    static void Merge4<T>(float* input, nint stride, int channels, byte* data, int frames) where T : struct, IPcmTarget
    {
        var lanes = default(T).Lanes;
        var f = 0;
        for (; f + lanes <= frames; f += lanes, data += 64)
            Neon.Store4(data, default(T).Bytes, default(T).Narrow(input + f), default(T).Narrow(input + stride + f), default(T).Narrow(input + 2 * stride + f),
                default(T).Narrow(input + 3 * stride + f));
        Interleave.Scalar<T>(input + f, stride, 4, data, frames - f);
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    static void Merge6<T>(float* input, nint stride, int channels, byte* data, int frames) where T : struct, IPcmTarget
    {
        var bytes = default(T).Bytes;
        var lanes = default(T).Lanes;
        var f = 0;
        for (; f + lanes <= frames; f += lanes, data += 96)
        {
            var (c0, c1) = (default(T).Narrow(input + f), default(T).Narrow(input + stride + f));
            var (c2, c3) = (default(T).Narrow(input + 2 * stride + f), default(T).Narrow(input + 3 * stride + f));
            var (c4, c5) = (default(T).Narrow(input + 4 * stride + f), default(T).Narrow(input + 5 * stride + f));
            Neon.Store3(data, 2 * bytes, Neon.ZipLow(c0, c1, bytes), Neon.ZipLow(c2, c3, bytes), Neon.ZipLow(c4, c5, bytes));
            Neon.Store3(data + 48, 2 * bytes, Neon.ZipHigh(c0, c1, bytes), Neon.ZipHigh(c2, c3, bytes), Neon.ZipHigh(c4, c5, bytes));
        }
        Interleave.Scalar<T>(input + f, stride, 6, data, frames - f);
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    static void Merge8<T>(float* input, nint stride, int channels, byte* data, int frames) where T : struct, IPcmTarget
    {
        var bytes = default(T).Bytes;
        var lanes = default(T).Lanes;
        var f = 0;
        for (; f + lanes <= frames; f += lanes, data += 128)
        {
            var (c0, c1) = (default(T).Narrow(input + f), default(T).Narrow(input + stride + f));
            var (c2, c3) = (default(T).Narrow(input + 2 * stride + f), default(T).Narrow(input + 3 * stride + f));
            var (c4, c5) = (default(T).Narrow(input + 4 * stride + f), default(T).Narrow(input + 5 * stride + f));
            var (c6, c7) = (default(T).Narrow(input + 6 * stride + f), default(T).Narrow(input + 7 * stride + f));
            Neon.Store4(data, 2 * bytes, Neon.ZipLow(c0, c1, bytes), Neon.ZipLow(c2, c3, bytes), Neon.ZipLow(c4, c5, bytes), Neon.ZipLow(c6, c7, bytes));
            Neon.Store4(data + 64, 2 * bytes, Neon.ZipHigh(c0, c1, bytes), Neon.ZipHigh(c2, c3, bytes), Neon.ZipHigh(c4, c5, bytes), Neon.ZipHigh(c6, c7, bytes));
        }
        Interleave.Scalar<T>(input + f, stride, 8, data, frames - f);
    }
}

static unsafe class Neon
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector128<byte> UnzipEven(Vector128<byte> a, Vector128<byte> b, int bytes) => bytes switch
    {
        1 => AdvSimd.Arm64.UnzipEven(a, b),
        2 => AdvSimd.Arm64.UnzipEven(a.AsUInt16(), b.AsUInt16()).AsByte(),
        _ => AdvSimd.Arm64.UnzipEven(a.AsUInt32(), b.AsUInt32()).AsByte()
    };

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector128<byte> UnzipOdd(Vector128<byte> a, Vector128<byte> b, int bytes) => bytes switch
    {
        1 => AdvSimd.Arm64.UnzipOdd(a, b),
        2 => AdvSimd.Arm64.UnzipOdd(a.AsUInt16(), b.AsUInt16()).AsByte(),
        _ => AdvSimd.Arm64.UnzipOdd(a.AsUInt32(), b.AsUInt32()).AsByte()
    };

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector128<byte> ZipLow(Vector128<byte> a, Vector128<byte> b, int bytes) => bytes switch
    {
        2 => AdvSimd.Arm64.ZipLow(a.AsUInt16(), b.AsUInt16()).AsByte(),
        _ => AdvSimd.Arm64.ZipLow(a.AsUInt32(), b.AsUInt32()).AsByte()
    };

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector128<byte> ZipHigh(Vector128<byte> a, Vector128<byte> b, int bytes) => bytes switch
    {
        2 => AdvSimd.Arm64.ZipHigh(a.AsUInt16(), b.AsUInt16()).AsByte(),
        _ => AdvSimd.Arm64.ZipHigh(a.AsUInt32(), b.AsUInt32()).AsByte()
    };

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Store24(Vector128<ushort> halves, float* at) => AdvSimd.Store(at, PcmScale.Times(halves.AsInt32(), 1f / 2147483648));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static (Vector128<byte>, Vector128<byte>) Load2(byte* at, int bytes)
    {
        if (bytes == 1) return AdvSimd.Arm64.Load2xVector128AndUnzip(at);
        if (bytes == 2)
        {
            var (a, b) = AdvSimd.Arm64.Load2xVector128AndUnzip((ushort*)at);
            return (a.AsByte(), b.AsByte());
        }
        if (bytes == 4)
        {
            var (a, b) = AdvSimd.Arm64.Load2xVector128AndUnzip((uint*)at);
            return (a.AsByte(), b.AsByte());
        }
        var (c, d) = AdvSimd.Arm64.Load2xVector128AndUnzip((ulong*)at);
        return (c.AsByte(), d.AsByte());
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static (Vector128<byte>, Vector128<byte>, Vector128<byte>) Load3(byte* at, int bytes)
    {
        if (bytes == 1) return AdvSimd.Arm64.Load3xVector128AndUnzip(at);
        if (bytes == 2)
        {
            var (a, b, c) = AdvSimd.Arm64.Load3xVector128AndUnzip((ushort*)at);
            return (a.AsByte(), b.AsByte(), c.AsByte());
        }
        if (bytes == 4)
        {
            var (a, b, c) = AdvSimd.Arm64.Load3xVector128AndUnzip((uint*)at);
            return (a.AsByte(), b.AsByte(), c.AsByte());
        }
        var (d, e, f) = AdvSimd.Arm64.Load3xVector128AndUnzip((ulong*)at);
        return (d.AsByte(), e.AsByte(), f.AsByte());
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static (Vector128<byte>, Vector128<byte>, Vector128<byte>, Vector128<byte>) Load4(byte* at, int bytes)
    {
        if (bytes == 1) return AdvSimd.Arm64.Load4xVector128AndUnzip(at);
        if (bytes == 2)
        {
            var (a, b, c, d) = AdvSimd.Arm64.Load4xVector128AndUnzip((ushort*)at);
            return (a.AsByte(), b.AsByte(), c.AsByte(), d.AsByte());
        }
        if (bytes == 4)
        {
            var (a, b, c, d) = AdvSimd.Arm64.Load4xVector128AndUnzip((uint*)at);
            return (a.AsByte(), b.AsByte(), c.AsByte(), d.AsByte());
        }
        var (e, f, g, h) = AdvSimd.Arm64.Load4xVector128AndUnzip((ulong*)at);
        return (e.AsByte(), f.AsByte(), g.AsByte(), h.AsByte());
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Store2(byte* at, int bytes, Vector128<byte> a, Vector128<byte> b)
    {
        if (bytes == 2) AdvSimd.Arm64.StoreVectorAndZip((ushort*)at, (a.AsUInt16(), b.AsUInt16()));
        else AdvSimd.Arm64.StoreVectorAndZip((uint*)at, (a.AsUInt32(), b.AsUInt32()));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Store3(byte* at, int bytes, Vector128<byte> a, Vector128<byte> b, Vector128<byte> c)
    {
        if (bytes == 2) AdvSimd.Arm64.StoreVectorAndZip((ushort*)at, (a.AsUInt16(), b.AsUInt16(), c.AsUInt16()));
        else if (bytes == 4) AdvSimd.Arm64.StoreVectorAndZip((uint*)at, (a.AsUInt32(), b.AsUInt32(), c.AsUInt32()));
        else AdvSimd.Arm64.StoreVectorAndZip((ulong*)at, (a.AsUInt64(), b.AsUInt64(), c.AsUInt64()));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Store4(byte* at, int bytes, Vector128<byte> a, Vector128<byte> b, Vector128<byte> c, Vector128<byte> d)
    {
        if (bytes == 2) AdvSimd.Arm64.StoreVectorAndZip((ushort*)at, (a.AsUInt16(), b.AsUInt16(), c.AsUInt16(), d.AsUInt16()));
        else if (bytes == 4) AdvSimd.Arm64.StoreVectorAndZip((uint*)at, (a.AsUInt32(), b.AsUInt32(), c.AsUInt32(), d.AsUInt32()));
        else AdvSimd.Arm64.StoreVectorAndZip((ulong*)at, (a.AsUInt64(), b.AsUInt64(), c.AsUInt64(), d.AsUInt64()));
    }
}
