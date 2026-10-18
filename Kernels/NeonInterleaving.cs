using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.Arm;

namespace GFXKit.Media.Codecs;

static unsafe class NeonDeinterleave
{
    public static delegate*<ReadOnlySpan<byte>, int, Span<float>, int, int, void> For<T>(int channels) where T : struct, IPcmSample
    {
        if (typeof(T) == typeof(S24)) return channels switch { 1 => &Mono24, 2 => &Stereo24, _ => &Deinterleave.Scalar<T> };
        if (typeof(T) == typeof(F32) && channels == 1) return &Copy;
        var kernel = NeonTransposes.DecodeKernel<T>(channels);
        return kernel != null ? kernel : &Deinterleave.Scalar<T>;
    }

    static void Copy(scoped ReadOnlySpan<byte> data, int channels, scoped Span<float> output, int stride, int frames) =>
        data[..(frames * 4)].CopyTo(MemoryMarshal.AsBytes(output[..frames]));

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    static void Mono24(scoped ReadOnlySpan<byte> data, int channels, scoped Span<float> output, int stride, int frames)
    {
        var target = new Planes(output, stride, 1, frames);
        var f = 0;
        fixed (byte* pinned = data)
        {
            var source = new Pinned(pinned, data.Length, 3, frames);
            for (; f <= frames - 16; f += 16)
            {
                var (b0, b1, b2) = AdvSimd.Arm64.Load3xVector128AndUnzip(source.At(f, 16));
                var into = target.At(f, 16);
                var low = AdvSimd.Arm64.ZipLow(Vector128<byte>.Zero, b0).AsUInt16();
                var high = AdvSimd.Arm64.ZipHigh(Vector128<byte>.Zero, b0).AsUInt16();
                var top = AdvSimd.Arm64.ZipLow(b1, b2).AsUInt16();
                var bottom = AdvSimd.Arm64.ZipHigh(b1, b2).AsUInt16();
                into.Store(0, 0, Neon.Convert24(AdvSimd.Arm64.ZipLow(low, top)));
                into.Store(0, 4, Neon.Convert24(AdvSimd.Arm64.ZipHigh(low, top)));
                into.Store(0, 8, Neon.Convert24(AdvSimd.Arm64.ZipLow(high, bottom)));
                into.Store(0, 12, Neon.Convert24(AdvSimd.Arm64.ZipHigh(high, bottom)));
            }
        }
        Deinterleave.Scalar<S24>(data[(f * 3)..], 1, output[f..], stride, frames - f);
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    static void Stereo24(scoped ReadOnlySpan<byte> data, int channels, scoped Span<float> output, int stride, int frames)
    {
        var target = new Planes(output, stride, 2, frames);
        var f = 0;
        fixed (byte* pinned = data)
        {
            var source = new Pinned(pinned, data.Length, 6, frames);
            for (; f <= frames - 8; f += 8)
            {
                var (h0, h1, h2) = AdvSimd.Arm64.Load3xVector128AndUnzip((ushort*)source.At(f, 8));
                var into = target.At(f, 8);
                var lower = AdvSimd.ShiftLeftLogical(h0, 8);
                var upper = AdvSimd.ShiftLeftAndInsert(AdvSimd.ShiftRightLogical(h0, 8), h1, 8);
                var start = AdvSimd.And(h1, Vector128.Create((ushort)0xFF00));
                into.Store(0, 0, Neon.Convert24(AdvSimd.Arm64.ZipLow(lower, upper)));
                into.Store(0, 4, Neon.Convert24(AdvSimd.Arm64.ZipHigh(lower, upper)));
                into.Store(1, 0, Neon.Convert24(AdvSimd.Arm64.ZipLow(start, h2)));
                into.Store(1, 4, Neon.Convert24(AdvSimd.Arm64.ZipHigh(start, h2)));
            }
        }
        Deinterleave.Scalar<S24>(data[(f * 6)..], 2, output[f..], stride, frames - f);
    }
}

static unsafe class NeonInterleave
{
    public static delegate*<ReadOnlySpan<float>, int, int, Span<byte>, int, void> For<T>(int channels) where T : struct, IPcmTarget
    {
        if (typeof(T) == typeof(F32) && channels == 1) return &Copy;
        var kernel = NeonTransposes.EncodeKernel<T>(channels);
        return kernel != null ? kernel : &Interleave.Scalar<T>;
    }

    static void Copy(scoped ReadOnlySpan<float> input, int stride, int channels, scoped Span<byte> data, int frames) =>
        MemoryMarshal.AsBytes(input[..frames]).CopyTo(data);
}

static class Neon
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector128<float> Convert24(Vector128<ushort> halves) => PcmScale.Times(halves.AsInt32(), 1f / 2147483648);
}
