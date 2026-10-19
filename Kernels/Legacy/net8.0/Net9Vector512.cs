using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;

static class Net9Vector512
{
    extension(Vector512)
    {
        public static Vector512<float> FusedMultiplyAdd(Vector512<float> left, Vector512<float> right, Vector512<float> addend) => Avx512F.FusedMultiplyAdd(left, right, addend);

        public static Vector512<float> MinNative(Vector512<float> left, Vector512<float> right) => Avx512F.Min(left, right);

        public static Vector512<float> MaxNative(Vector512<float> left, Vector512<float> right) => Avx512F.Max(left, right);
    }
}
