global using GFXKit.Legacy;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.Arm;
using System.Runtime.Intrinsics.X86;

namespace GFXKit.Legacy;

static class VectorOperators
{
    extension(Vector128<float>)
    {
        public static Vector128<float> operator *(Vector128<float> left, Vector128<float> right) =>
            Sse.IsSupported ? Sse.Multiply(left, right) : AdvSimd.IsSupported ? AdvSimd.Multiply(left, right) : Vector128.Create(left.GetElement(0) * right.GetElement(0), left.GetElement(1) * right.GetElement(1), left.GetElement(2) * right.GetElement(2), left.GetElement(3) * right.GetElement(3));

        public static Vector128<float> operator -(Vector128<float> left, Vector128<float> right) =>
            Sse.IsSupported ? Sse.Subtract(left, right) : AdvSimd.IsSupported ? AdvSimd.Subtract(left, right) : Vector128.Create(left.GetElement(0) - right.GetElement(0), left.GetElement(1) - right.GetElement(1), left.GetElement(2) - right.GetElement(2), left.GetElement(3) - right.GetElement(3));
    }

    extension(Vector256<float>)
    {
        public static Vector256<float> operator *(Vector256<float> left, Vector256<float> right) => Avx.Multiply(left, right);
    }

    extension(Vector128<int>)
    {
        public static Vector128<int> operator <<(Vector128<int> value, int count) =>
            Sse2.IsSupported ? Sse2.ShiftLeftLogical(value, (byte)count) : AdvSimd.ShiftLogical(value, Vector128.Create(count));
    }

    extension(Vector256<int>)
    {
        public static Vector256<int> operator <<(Vector256<int> value, int count) => Avx2.ShiftLeftLogical(value, (byte)count);

        public static Vector256<int> operator >>>(Vector256<int> value, int count) => Avx2.ShiftRightLogical(value, (byte)count);
    }

    extension<T>(Vector128<T>) where T : struct
    {
        public static Vector128<T> operator |(Vector128<T> left, Vector128<T> right) =>
            (Sse2.IsSupported ? Sse2.Or(left.AsByte(), right.AsByte()) : AdvSimd.Or(left.AsByte(), right.AsByte())).As<byte, T>();

        public static Vector128<T> operator ^(Vector128<T> left, Vector128<T> right) =>
            (Sse2.IsSupported ? Sse2.Xor(left.AsByte(), right.AsByte()) : AdvSimd.Xor(left.AsByte(), right.AsByte())).As<byte, T>();
    }

    extension<T>(Vector256<T>) where T : struct
    {
        public static Vector256<T> operator &(Vector256<T> left, Vector256<T> right) => Avx2.And(left.AsByte(), right.AsByte()).As<byte, T>();
    }
}
