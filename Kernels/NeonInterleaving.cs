using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.Arm;

namespace GFXKit.Media.Codecs;

static unsafe class NeonDeinterleave
{
    public static delegate*<ReadOnlySpan<byte>, int, Span<float>, int, int, void> For<T>(int channels) where T : struct, IPcmSample
    {
        if (typeof(T) == typeof(F32) && channels == 1) return &Copy;
        var kernel = NeonTransposes.DecodeKernel<T>(channels);
        return kernel != null ? kernel : &Deinterleave.Scalar<T>;
    }

    static void Copy(scoped ReadOnlySpan<byte> data, int channels, scoped Span<float> output, int stride, int frames) =>
        MemoryCopy.Copy(data[..(frames * 4)], MemoryMarshal.AsBytes(output[..frames]));
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
        MemoryCopy.Copy(MemoryMarshal.AsBytes(input[..frames]), data);
}

static class Neon
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector128<float> Convert24(Vector128<ushort> halves) => PcmScale.Times(halves.AsInt32(), 1f / 2147483648);
}
