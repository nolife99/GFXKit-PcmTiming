using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.Arm;
using System.Runtime.Intrinsics.X86;

namespace GFXKit.Media.Codecs;

interface IPcmSample
{
    int Bytes { get; }

    float Read(ref byte at);

    Vector256<float> Read8(ref byte block, int offset);

    Vector128<float> Read4(ref byte block, int offset);
}

interface IPcmTarget : IPcmSample
{
    void Write(ref byte at, float value);

    void Write8(ref byte block, int offset, Vector256<float> values);

    void Write4(ref byte block, int offset, Vector128<float> values);
}

static class PcmScale
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector256<float> Times(Vector256<int> values, float scale) => Vector256.ConvertToSingle(values) * Vector256.Create(scale);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Clamp(float value, float top) => value >= -1f ? value <= top ? value : top : -1f;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector128<float> Scale(Vector128<int> values, float scale) => Vector128.ConvertToSingle(values) * Vector128.Create(scale);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector128<int> Quantize(Vector128<float> values, float top, float scale) =>
        Sse2.ConvertToVector128Int32(Vector128.MinNative(Vector128.MaxNative(values, Vector128.Create(-1f)), Vector128.Create(top)) * Vector128.Create(scale));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector128<float> Times(Vector128<int> values, float scale) => Vector128.ConvertToSingle(values) * Vector128.Create(scale);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector256<int> Round(Vector256<float> values, float top, float scale) =>
        Avx.ConvertToVector256Int32(Vector256.MinNative(Vector256.MaxNative(values, Vector256.Create(-1f)), Vector256.Create(top)) * Vector256.Create(scale));
}

struct U8 : IPcmSample
{
    public int Bytes => 1;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public float Read(ref byte at) => (at - 128) * (1f / 128);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Vector256<float> Read8(ref byte block, int offset)
    {
        var values = Vector256.ConvertToSingle(Avx2.ConvertToVector256Int32(Packed.Read8(ref block, offset)));
        return Vector256.MultiplyAddEstimate(values, Vector256.Create(1f / 128), Vector256.Create(-1f));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Vector128<float> Read4(ref byte block, int offset) =>
        PcmScale.Scale(Sse41.ConvertToVector128Int32(Vector128.CreateScalar(Packed.Read4(ref block, offset)).AsByte()), 1f / 128) - Vector128.Create(1f);
}

struct S16 : IPcmTarget
{
    public const float Top = 32767f / 32768;

    public int Bytes => 2;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public float Read(ref byte at) => Unsafe.ReadUnaligned<short>(ref at) * (1f / 32768);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Vector256<float> Read8(ref byte block, int offset) => Widen(Avx2.ConvertToVector256Int32(Packed.Read16(ref block, offset).AsInt16()));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector256<float> Widen(Vector256<int> samples) => PcmScale.Times(samples, 1f / 32768);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector256<int> Round(Vector256<float> values) => PcmScale.Round(values, Top, 32768);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Vector128<float> Read4(ref byte block, int offset) => PcmScale.Scale(Sse41.ConvertToVector128Int32(Packed.Read8(ref block, offset).AsInt16()), 1f / 32768);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Write4(ref byte block, int offset, Vector128<float> values)
    {
        var rounded = PcmScale.Quantize(values, Top, 32768);
        Packed.Write8(ref block, offset, Sse2.PackSignedSaturate(rounded, rounded).AsByte());
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Write(ref byte at, float value) => Unsafe.WriteUnaligned(ref at, (short)MathF.Round(PcmScale.Clamp(value, Top) * 32768));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Write8(ref byte block, int offset, Vector256<float> values)
    {
        var rounded = Round(values);
        Packed.Write16(ref block, offset, Sse2.PackSignedSaturate(rounded.GetLower(), rounded.GetUpper()).AsByte());
    }
}

struct S24 : IPcmTarget
{
    public const float Top = 8388607f / 8388608;

    public int Bytes => 3;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public float Read(ref byte at) => (at | Unsafe.Add(ref at, 1) << 8 | (sbyte)Unsafe.Add(ref at, 2) << 16) * (1f / 8388608);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Vector256<float> Read8(ref byte block, int offset)
    {
        var bytes = Vector256.Create(Packed.Read16(ref block, offset), Packed.Read16(ref block, offset + 8));
        var placed = Avx2.Shuffle(bytes, Vector256.Create(
            (byte)0x80, 0, 1, 2, 0x80, 3, 4, 5, 0x80, 6, 7, 8, 0x80, 9, 10, 11,
            0x80, 4, 5, 6, 0x80, 7, 8, 9, 0x80, 10, 11, 12, 0x80, 13, 14, 15));
        return PcmScale.Times(placed.AsInt32(), 1f / 2147483648);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Vector128<float> Read4(ref byte block, int offset)
    {
        var bytes = Sse41.Insert(Packed.Read8(ref block, offset).AsInt32(), Packed.Read4(ref block, offset + 8), 2).AsByte();
        return PcmScale.Scale(Ssse3.Shuffle(bytes, Vector128.Create((byte)0x80, 0, 1, 2, 0x80, 3, 4, 5, 0x80, 6, 7, 8, 0x80, 9, 10, 11)).AsInt32(), 1f / 2147483648);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Write4(ref byte block, int offset, Vector128<float> values)
    {
        var packed = Ssse3.Shuffle(PcmScale.Quantize(values, Top, 8388608).AsByte(), Vector128.Create((byte)0, 1, 2, 4, 5, 6, 8, 9, 10, 12, 13, 14, 0x80, 0x80, 0x80, 0x80));
        Packed.Write8(ref block, offset, packed);
        Packed.Write4(ref block, offset + 8, packed.AsInt32().GetElement(2));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Write(ref byte at, float value)
    {
        var sample = (int)MathF.Round(PcmScale.Clamp(value, Top) * 8388608);
        at = (byte)sample;
        Unsafe.Add(ref at, 1) = (byte)(sample >> 8);
        Unsafe.Add(ref at, 2) = (byte)(sample >> 16);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Write8(ref byte block, int offset, Vector256<float> values)
    {
        var packed = Avx2.Shuffle(PcmScale.Round(values, Top, 8388608).AsByte(), Vector256.Create(
            (byte)0, 1, 2, 4, 5, 6, 8, 9, 10, 12, 13, 14, 0x80, 0x80, 0x80, 0x80,
            0, 1, 2, 4, 5, 6, 8, 9, 10, 12, 13, 14, 0x80, 0x80, 0x80, 0x80));
        var joined = Avx2.PermuteVar8x32(packed.AsInt32(), Vector256.Create(0, 1, 2, 4, 5, 6, 7, 7)).AsByte();
        Packed.Write16(ref block, offset, joined.GetLower());
        Packed.Write8(ref block, offset + 16, joined.GetUpper());
    }
}

struct S32 : IPcmTarget
{
    public const float Top = 1 - 1f / 16777216;

    public int Bytes => 4;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public float Read(ref byte at) => Unsafe.ReadUnaligned<int>(ref at) * (1f / 2147483648);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Vector256<float> Read8(ref byte block, int offset) => PcmScale.Times(Packed.Read32(ref block, offset).AsInt32(), 1f / 2147483648);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Vector128<float> Read4(ref byte block, int offset) => PcmScale.Scale(Packed.Read16(ref block, offset).AsInt32(), 1f / 2147483648);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Write4(ref byte block, int offset, Vector128<float> values) => Packed.Write16(ref block, offset, PcmScale.Quantize(values, Top, 2147483648).AsByte());

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Write(ref byte at, float value) => Unsafe.WriteUnaligned(ref at, (int)MathF.Round(PcmScale.Clamp(value, Top) * 2147483648));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Write8(ref byte block, int offset, Vector256<float> values) => Packed.Write32(ref block, offset, PcmScale.Round(values, Top, 2147483648).AsByte());
}

struct F32 : IPcmTarget
{
    public int Bytes => 4;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public float Read(ref byte at) => Unsafe.ReadUnaligned<float>(ref at);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Vector256<float> Read8(ref byte block, int offset) => Packed.Read32(ref block, offset).AsSingle();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Vector128<float> Read4(ref byte block, int offset) => Packed.Read16(ref block, offset).AsSingle();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Write4(ref byte block, int offset, Vector128<float> values) => Packed.Write16(ref block, offset, values.AsByte());

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Write(ref byte at, float value) => Unsafe.WriteUnaligned(ref at, value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Write8(ref byte block, int offset, Vector256<float> values) => Packed.Write32(ref block, offset, values.AsByte());
}

struct F64 : IPcmSample
{
    public int Bytes => 8;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public float Read(ref byte at) => (float)Unsafe.ReadUnaligned<double>(ref at);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Vector256<float> Read8(ref byte block, int offset) =>
        Vector256.Narrow(Packed.Read32(ref block, offset).AsDouble(), Packed.Read32(ref block, offset + 32).AsDouble());

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Vector128<float> Read4(ref byte block, int offset) =>
        Vector128.Narrow(Packed.Read16(ref block, offset).AsDouble(), Packed.Read16(ref block, offset + 16).AsDouble());
}
