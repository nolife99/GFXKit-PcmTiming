using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.Arm;
using System.Runtime.Intrinsics.X86;

namespace GFXKit.Benchmarks.Pointers;

using GFXKit.Media;

unsafe interface IPcmSample
{
    int Bytes { get; }

    float Read(byte* at);

    Vector256<float> Read8(byte* at);

    Vector128<float> Read4(byte* at);

    int Lanes { get; }

    void Widen(Vector128<byte> raw, float* at);
}

unsafe interface IPcmTarget : IPcmSample
{
    void Write(byte* at, float value);

    void Write8(byte* at, Vector256<float> values);

    void Write4(byte* at, Vector128<float> values);

    Vector128<byte> Narrow(float* at);
}

static class PcmScale
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector256<float> Times(Vector256<int> values, float scale) => Avx.Multiply(Avx.ConvertToVector256Single(values), Vector256.Create(scale));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Clamp(float value, float top) => value >= -1f ? value <= top ? value : top : -1f;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector128<float> Scale(Vector128<int> values, float scale) => Sse.Multiply(Sse2.ConvertToVector128Single(values), Vector128.Create(scale));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector128<int> Quantize(Vector128<float> values, float top, float scale) =>
        Sse2.ConvertToVector128Int32(Sse.Multiply(Sse.Min(Sse.Max(values, Vector128.Create(-1f)), Vector128.Create(top)), Vector128.Create(scale)));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector128<float> Times(Vector128<int> values, float scale) => AdvSimd.Multiply(AdvSimd.ConvertToSingle(values), Vector128.Create(scale));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector128<int> Round(Vector128<float> values, float top, float scale) =>
        AdvSimd.ConvertToInt32RoundToEven(AdvSimd.Multiply(AdvSimd.Min(AdvSimd.MaxNumber(values, Vector128.Create(-1f)), Vector128.Create(top)), Vector128.Create(scale)));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector256<int> Round(Vector256<float> values, float top, float scale) =>
        Avx.ConvertToVector256Int32(Avx.Multiply(Avx.Min(Avx.Max(values, Vector256.Create(-1f)), Vector256.Create(top)), Vector256.Create(scale)));
}

unsafe struct U8 : IPcmSample
{
    public int Bytes => 1;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public float Read(byte* at) => (*at - 128) * (1f / 128);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Vector256<float> Read8(byte* at)
    {
        var values = Avx.ConvertToVector256Single(Avx2.ConvertToVector256Int32(at));
        return Fma.IsSupported
            ? Fma.MultiplySubtract(values, Vector256.Create(1f / 128), Vector256.Create(1f))
            : Avx.Subtract(Avx.Multiply(values, Vector256.Create(1f / 128)), Vector256.Create(1f));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Vector128<float> Read4(byte* at) => Sse.Subtract(PcmScale.Scale(Sse41.ConvertToVector128Int32(at), 1f / 128), Vector128.Create(1f));

    public int Lanes => 16;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Widen(Vector128<byte> raw, float* at)
    {
        var low = AdvSimd.ZeroExtendWideningLower(raw.GetLower());
        var high = AdvSimd.ZeroExtendWideningUpper(raw);
        Store(AdvSimd.ZeroExtendWideningLower(low.GetLower()), at);
        Store(AdvSimd.ZeroExtendWideningUpper(low), at + 4);
        Store(AdvSimd.ZeroExtendWideningLower(high.GetLower()), at + 8);
        Store(AdvSimd.ZeroExtendWideningUpper(high), at + 12);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static void Store(Vector128<uint> values, float* at) =>
        AdvSimd.Store(at, AdvSimd.FusedMultiplyAdd(Vector128.Create(-1f), AdvSimd.ConvertToSingle(values), Vector128.Create(1f / 128)));
}

unsafe struct S16 : IPcmTarget
{
    const float Top = 32767f / 32768;

    public int Bytes => 2;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public float Read(byte* at) => *(short*)at * (1f / 32768);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Vector256<float> Read8(byte* at) => Widen(Avx2.ConvertToVector256Int32((short*)at));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector256<float> Widen(Vector256<int> samples) => PcmScale.Times(samples, 1f / 32768);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector256<int> Round(Vector256<float> values) => PcmScale.Round(values, Top, 32768);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Vector128<float> Read4(byte* at) => PcmScale.Scale(Sse41.ConvertToVector128Int32((short*)at), 1f / 32768);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Write4(byte* at, Vector128<float> values)
    {
        var rounded = PcmScale.Quantize(values, Top, 32768);
        Sse2.StoreScalar((long*)at, Sse2.PackSignedSaturate(rounded, rounded).AsInt64());
    }

    public int Lanes => 8;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Widen(Vector128<byte> raw, float* at)
    {
        var samples = raw.AsInt16();
        AdvSimd.Store(at, PcmScale.Times(AdvSimd.SignExtendWideningLower(samples.GetLower()), 1f / 32768));
        AdvSimd.Store(at + 4, PcmScale.Times(AdvSimd.SignExtendWideningUpper(samples), 1f / 32768));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Vector128<byte> Narrow(float* at) =>
        AdvSimd.ExtractNarrowingSaturateUpper(AdvSimd.ExtractNarrowingSaturateLower(PcmScale.Round(AdvSimd.LoadVector128(at), Top, 32768)),
            PcmScale.Round(AdvSimd.LoadVector128(at + 4), Top, 32768)).AsByte();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Write(byte* at, float value) => *(short*)at = (short)MathF.Round(PcmScale.Clamp(value, Top) * 32768);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Write8(byte* at, Vector256<float> values)
    {
        var rounded = Round(values);
        Sse2.Store((short*)at, Sse2.PackSignedSaturate(rounded.GetLower(), rounded.GetUpper()));
    }
}

unsafe struct S24 : IPcmTarget
{
    const float Top = 8388607f / 8388608;

    public int Bytes => 3;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public float Read(byte* at) => (at[0] | at[1] << 8 | (sbyte)at[2] << 16) * (1f / 8388608);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Vector256<float> Read8(byte* at)
    {
        var bytes = Vector256.Create(Sse2.LoadVector128(at), Sse2.LoadVector128(at + 8));
        var placed = Avx2.Shuffle(bytes, Vector256.Create(
            (byte)0x80, 0, 1, 2, 0x80, 3, 4, 5, 0x80, 6, 7, 8, 0x80, 9, 10, 11,
            0x80, 4, 5, 6, 0x80, 7, 8, 9, 0x80, 10, 11, 12, 0x80, 13, 14, 15));
        return PcmScale.Times(placed.AsInt32(), 1f / 2147483648);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Vector128<float> Read4(byte* at)
    {
        var bytes = Sse41.Insert(Sse2.LoadScalarVector128((long*)at).AsInt32(), *(int*)(at + 8), 2).AsByte();
        return PcmScale.Scale(Ssse3.Shuffle(bytes, Vector128.Create((byte)0x80, 0, 1, 2, 0x80, 3, 4, 5, 0x80, 6, 7, 8, 0x80, 9, 10, 11)).AsInt32(), 1f / 2147483648);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Write4(byte* at, Vector128<float> values)
    {
        var packed = Ssse3.Shuffle(PcmScale.Quantize(values, Top, 8388608).AsByte(), Vector128.Create((byte)0, 1, 2, 4, 5, 6, 8, 9, 10, 12, 13, 14, 0x80, 0x80, 0x80, 0x80));
        Sse2.StoreScalar((long*)at, packed.AsInt64());
        Sse2.StoreScalar((int*)(at + 8), Sse2.ShiftRightLogical128BitLane(packed, 8).AsInt32());
    }

    public int Lanes => 0;

    public void Widen(Vector128<byte> raw, float* at) => throw new NotSupportedException();

    public Vector128<byte> Narrow(float* at) => throw new NotSupportedException();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Write(byte* at, float value)
    {
        var sample = (int)MathF.Round(PcmScale.Clamp(value, Top) * 8388608);
        (at[0], at[1], at[2]) = ((byte)sample, (byte)(sample >> 8), (byte)(sample >> 16));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Write8(byte* at, Vector256<float> values)
    {
        var packed = Avx2.Shuffle(PcmScale.Round(values, Top, 8388608).AsByte(), Vector256.Create(
            (byte)0, 1, 2, 4, 5, 6, 8, 9, 10, 12, 13, 14, 0x80, 0x80, 0x80, 0x80,
            0, 1, 2, 4, 5, 6, 8, 9, 10, 12, 13, 14, 0x80, 0x80, 0x80, 0x80));
        var joined = Avx2.PermuteVar8x32(packed.AsInt32(), Vector256.Create(0, 1, 2, 4, 5, 6, 7, 7));
        Sse2.Store((int*)at, joined.GetLower());
        Sse2.StoreScalar((long*)(at + 16), joined.GetUpper().AsInt64());
    }
}

unsafe struct S32 : IPcmTarget
{
    const float Top = 1 - 1f / 16777216;

    public int Bytes => 4;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public float Read(byte* at) => *(int*)at * (1f / 2147483648);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Vector256<float> Read8(byte* at) => PcmScale.Times(Avx.LoadVector256((int*)at), 1f / 2147483648);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Vector128<float> Read4(byte* at) => PcmScale.Scale(Sse2.LoadVector128((int*)at), 1f / 2147483648);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Write4(byte* at, Vector128<float> values) => Sse2.Store((int*)at, PcmScale.Quantize(values, Top, 2147483648));

    public int Lanes => 4;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Widen(Vector128<byte> raw, float* at) => AdvSimd.Store(at, PcmScale.Times(raw.AsInt32(), 1f / 2147483648));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Vector128<byte> Narrow(float* at) => PcmScale.Round(AdvSimd.LoadVector128(at), Top, 2147483648).AsByte();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Write(byte* at, float value) => *(int*)at = (int)MathF.Round(PcmScale.Clamp(value, Top) * 2147483648);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Write8(byte* at, Vector256<float> values) => Avx.Store((int*)at, PcmScale.Round(values, Top, 2147483648));
}

unsafe struct F32 : IPcmTarget
{
    public int Bytes => 4;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public float Read(byte* at) => *(float*)at;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Vector256<float> Read8(byte* at) => Avx.LoadVector256((float*)at);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Vector128<float> Read4(byte* at) => Sse.LoadVector128((float*)at);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Write4(byte* at, Vector128<float> values) => Sse.Store((float*)at, values);

    public int Lanes => 4;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Widen(Vector128<byte> raw, float* at) => AdvSimd.Store(at, raw.AsSingle());

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Vector128<byte> Narrow(float* at) => AdvSimd.LoadVector128(at).AsByte();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Write(byte* at, float value) => *(float*)at = value;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Write8(byte* at, Vector256<float> values) => Avx.Store((float*)at, values);
}

unsafe struct F64 : IPcmSample
{
    public int Bytes => 8;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public float Read(byte* at) => (float)*(double*)at;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Vector256<float> Read8(byte* at) =>
        Vector256.Create(Avx.ConvertToVector128Single(Avx.LoadVector256((double*)at)), Avx.ConvertToVector128Single(Avx.LoadVector256((double*)at + 4)));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Vector128<float> Read4(byte* at) =>
        Sse.MoveLowToHigh(Sse2.ConvertToVector128Single(Sse2.LoadVector128((double*)at)), Sse2.ConvertToVector128Single(Sse2.LoadVector128((double*)at + 2)));

    public int Lanes => 2;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Widen(Vector128<byte> raw, float* at) => AdvSimd.Store(at, AdvSimd.Arm64.ConvertToSingleLower(raw.AsDouble()));
}
