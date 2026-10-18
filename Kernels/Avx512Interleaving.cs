using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;

namespace GFXKit.Media.Codecs;

static unsafe class Avx512Deinterleave
{
    public static bool Supported => Vector512.IsHardwareAccelerated && Avx512BW.IsSupported;

    public static delegate*<ReadOnlySpan<byte>, int, Span<float>, int, int, void> For<T>(int channels) where T : struct, IPcmSample =>
        Supported ? Avx512Transposes.DecodeKernel<T>(channels) : null;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector512<float> Read<T>(scoped ReadOnlySpan<byte> block, int offset) where T : struct, IPcmSample
    {
        if (typeof(T) == typeof(F32)) return Packed512.Read(block, offset).AsSingle();
        if (typeof(T) == typeof(S32)) return Times(Packed512.Read(block, offset).AsInt32(), 1f / 2147483648);
        if (typeof(T) == typeof(S16)) return Times(Avx512F.ConvertToVector512Int32(Packed.Read32(block, offset).AsInt16()), 1f / 32768);
        if (typeof(T) == typeof(U8))
            return Avx512F.FusedMultiplyAdd(Avx512F.ConvertToVector512Single(Avx512F.ConvertToVector512Int32(Packed.Read16(block, offset))), Vector512.Create(1f / 128), Vector512.Create(-1f));
        if (typeof(T) == typeof(F64))
            return Avx512F.InsertVector256(Avx512F.ConvertToVector256Single(Packed512.Read(block, offset).AsDouble()).ToVector512Unsafe(), Avx512F.ConvertToVector256Single(Packed512.Read(block, offset + 64).AsDouble()), 1);
        throw new NotSupportedException();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector512<float> Times(Vector512<int> values, float scale) => Avx512F.Multiply(Avx512F.ConvertToVector512Single(values), Vector512.Create(scale));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector512<float> WidenS24(Vector512<byte> placed) => Times(Avx512F.ShiftRightArithmetic(placed.AsInt32(), 8), 1f / 8388608);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector512<float> WidenS16(Vector512<byte> placed) => Times(Avx512F.ShiftRightArithmetic(placed.AsInt32(), 16), 1f / 32768);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector512<float> WidenU8(Vector512<byte> placed) =>
        Avx512F.FusedMultiplyAdd(Avx512F.ConvertToVector512Single(Avx512F.ShiftRightLogical(placed.AsInt32(), 24)), Vector512.Create(1f / 128), Vector512.Create(-1f));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector512<float> Even(Vector512<float> pairs) => Times(Avx512BW.MultiplyAddAdjacent(pairs.AsInt16(), Vector512.Create(1).AsInt16()), 1f / 32768);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector512<float> Odd(Vector512<float> pairs) => Times(Avx512BW.MultiplyAddAdjacent(pairs.AsInt16(), Vector512.Create(0x10000).AsInt16()), 1f / 32768);
}

static unsafe class Avx512Interleave
{
    public static delegate*<ReadOnlySpan<float>, int, int, Span<byte>, int, void> For<T>(int channels) where T : struct, IPcmTarget =>
        Avx512Deinterleave.Supported ? Avx512Transposes.EncodeKernel<T>(channels) : null;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector512<int> Quantize(Vector512<float> values, float top, float scale) =>
        Avx512F.ConvertToVector512Int32(Avx512F.Multiply(Avx512F.Min(Avx512F.Max(values, Vector512.Create(-1f)), Vector512.Create(top)), Vector512.Create(scale)));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Write<T>(scoped Span<byte> block, int offset, Vector512<float> values) where T : struct, IPcmTarget
    {
        if (typeof(T) == typeof(F32)) Packed512.Write(block, offset, values.AsByte());
        else if (typeof(T) == typeof(S32)) Packed512.Write(block, offset, Quantize(values, S32.Top, 2147483648).AsByte());
        else if (typeof(T) == typeof(S16)) Packed.Write32(block, offset, Avx512F.ConvertToVector256Int16WithSaturation(Quantize(values, S16.Top, 32768)).AsByte());
        else throw new NotSupportedException();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector512<float> Pair(Vector512<float> even, Vector512<float> odd) =>
        Avx512F.Or(Avx512F.ShiftLeftLogical(Quantize(odd, S16.Top, 32768), 16), Avx512F.And(Quantize(even, S16.Top, 32768), Vector512.Create(0xFFFF))).AsSingle();
}

static class Packed512
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector512<byte> Read(scoped ReadOnlySpan<byte> block, int offset) => Unsafe.ReadUnaligned<Vector512<byte>>(ref Packed.At(block, offset, 64));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Write(scoped Span<byte> block, int offset, Vector512<byte> values) => Unsafe.WriteUnaligned(ref Packed.At(block, offset, 64), values);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Store(this scoped Planes.Block block, int channel, int frame, Vector512<float> values) =>
        Unsafe.WriteUnaligned(ref Unsafe.As<float, byte>(ref block.At(channel, frame, 16)), values);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector512<float> Load16(this scoped Planes.Block block, int channel, int frame) =>
        Unsafe.ReadUnaligned<Vector512<float>>(ref Unsafe.As<float, byte>(ref block.At(channel, frame, 16)));
}
