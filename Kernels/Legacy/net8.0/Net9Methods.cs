using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.Arm;
using System.Runtime.Intrinsics.X86;

static class Net9Methods
{
    static bool Fused => Fma.IsSupported || AdvSimd.IsSupported;

    extension(float)
    {
        public static float MultiplyAddEstimate(float left, float right, float addend) => Fused ? MathF.FusedMultiplyAdd(left, right, addend) : left * right + addend;
    }

    extension(double)
    {
        public static double MultiplyAddEstimate(double left, double right, double addend) => Fused ? Math.FusedMultiplyAdd(left, right, addend) : left * right + addend;
    }

    extension(Vector)
    {
        public static Vector<float> MultiplyAddEstimate(Vector<float> left, Vector<float> right, Vector<float> addend)
        {
            if (Vector<float>.Count == 8 && Fma.IsSupported) return Fma.MultiplyAdd(left.AsVector256(), right.AsVector256(), addend.AsVector256()).AsVector();
            if (Vector<float>.Count == 4 && Fma.IsSupported) return Fma.MultiplyAdd(left.AsVector128(), right.AsVector128(), addend.AsVector128()).AsVector();
            if (Vector<float>.Count == 4 && AdvSimd.IsSupported) return AdvSimd.FusedMultiplyAdd(addend.AsVector128(), left.AsVector128(), right.AsVector128()).AsVector();
            return left * right + addend;
        }
    }

    extension(Vector256)
    {
        public static Vector256<float> MultiplyAddEstimate(Vector256<float> left, Vector256<float> right, Vector256<float> addend) =>
            Fma.IsSupported ? Fma.MultiplyAdd(left, right, addend) : Avx.Add(Avx.Multiply(left, right), addend);

        public static Vector256<float> MinNative(Vector256<float> left, Vector256<float> right) => Avx.Min(left, right);

        public static Vector256<float> MaxNative(Vector256<float> left, Vector256<float> right) => Avx.Max(left, right);
    }

    extension(Vector128)
    {
        public static Vector128<float> MinNative(Vector128<float> left, Vector128<float> right) =>
            Sse.IsSupported ? Sse.Min(left, right) : AdvSimd.Min(left, right);

        public static Vector128<float> MaxNative(Vector128<float> left, Vector128<float> right) =>
            Sse.IsSupported ? Sse.Max(left, right) : AdvSimd.Max(left, right);
    }

    extension(Convert)
    {
        public static string ToHexStringLower(byte[] inArray) => ToHexStringLower(inArray.AsSpan());

        public static string ToHexStringLower(ReadOnlySpan<byte> bytes) => Convert.ToHexString(bytes).ToLowerInvariant();

        public static bool TryToHexString(ReadOnlySpan<byte> source, Span<char> destination, out int charsWritten)
        {
            if (destination.Length < source.Length * 2)
            {
                charsWritten = 0;
                return false;
            }
            for (var i = 0; i < source.Length; i++)
            {
                destination[i * 2] = Digit(source[i] >> 4);
                destination[i * 2 + 1] = Digit(source[i] & 15);
            }
            charsWritten = source.Length * 2;
            return true;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static char Digit(int value) => (char)(value < 10 ? '0' + value : 'A' + value - 10);
}
