using System.Diagnostics;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.Arm;
using System.Runtime.Intrinsics.X86;

static unsafe class Net7Methods
{
    public static int IndexOfAnyExcept<T>(this ReadOnlySpan<T> span, T value) where T : IEquatable<T>
    {
        for (var i = 0; i < span.Length; i++)
            if (!span[i].Equals(value)) return i;
        return -1;
    }

    extension(ObjectDisposedException)
    {
        public static void ThrowIf(bool condition, object instance)
        {
            if (condition) throw new ObjectDisposedException(instance.GetType().FullName);
        }

        public static void ThrowIf(bool condition, Type type)
        {
            if (condition) throw new ObjectDisposedException(type.FullName);
        }
    }

    extension(Stopwatch)
    {
        public static TimeSpan GetElapsedTime(long startingTimestamp) => GetElapsedTime(startingTimestamp, Stopwatch.GetTimestamp());

        public static TimeSpan GetElapsedTime(long startingTimestamp, long endingTimestamp) =>
            new((long)((endingTimestamp - startingTimestamp) * ((double)TimeSpan.TicksPerSecond / Stopwatch.Frequency)));
    }

    extension(NativeMemory)
    {
        public static void Clear(void* ptr, nuint byteCount)
        {
            for (var at = (byte*)ptr; byteCount > 0; at += uint.MaxValue, byteCount -= Math.Min(byteCount, uint.MaxValue))
                Unsafe.InitBlockUnaligned(at, 0, (uint)Math.Min(byteCount, uint.MaxValue));
        }
    }

    extension(TimeSpan span)
    {
        public double TotalMicroseconds => span.Ticks / 10.0;

        public double TotalNanoseconds => span.Ticks * 100.0;
    }

    extension(int)
    {
        public static int LeadingZeroCount(int value) => BitOperations.LeadingZeroCount((uint)value);
    }

    extension(Vector)
    {
        public static Vector<int> ShiftLeft(Vector<int> value, int shiftCount) => value * new Vector<int>(1 << shiftCount);

        public static Vector<int> ShiftRightArithmetic(Vector<int> value, int shiftCount)
        {
            if (Vector<int>.Count == 8 && Avx2.IsSupported) return Avx2.ShiftRightArithmetic(value.AsVector256(), (byte)shiftCount).AsVector();
            if (Vector<int>.Count == 4 && Sse2.IsSupported) return Sse2.ShiftRightArithmetic(value.AsVector128(), (byte)shiftCount).AsVector();
            if (Vector<int>.Count == 4 && AdvSimd.IsSupported) return AdvSimd.ShiftArithmetic(value.AsVector128(), Vector128.Create(-shiftCount)).AsVector();
            Span<int> lanes = stackalloc int[Vector<int>.Count];
            value.CopyTo(lanes);
            for (var i = 0; i < lanes.Length; i++) lanes[i] >>= shiftCount;
            return new Vector<int>(lanes);
        }
    }

    extension(Vector128)
    {
        public static Vector128<T> LoadUnsafe<T>(ref T source, nuint elementOffset) where T : struct =>
            Unsafe.ReadUnaligned<Vector128<T>>(ref Unsafe.As<T, byte>(ref Unsafe.Add(ref source, (nint)elementOffset)));
    }

    public static Vector128<byte> Equals(Vector128<byte> left, Vector128<byte> right)
    {
        if (Sse2.IsSupported) return Sse2.CompareEqual(left, right);
        if (AdvSimd.IsSupported) return AdvSimd.CompareEqual(left, right);
        var lanes = Vector128<byte>.Zero;
        for (var i = 0; i < Vector128<byte>.Count; i++) lanes = lanes.WithElement(i, left.GetElement(i) == right.GetElement(i) ? byte.MaxValue : (byte)0);
        return lanes;
    }

    extension(Vector128<byte> vector)
    {
        public uint ExtractMostSignificantBits()
        {
            if (Sse2.IsSupported) return (uint)Sse2.MoveMask(vector);
            if (AdvSimd.Arm64.IsSupported)
            {
                var weights = Vector128.Create(1, 2, 4, 8, 16, 32, 64, 128, 1, 2, 4, 8, 16, 32, 64, (byte)128);
                var bits = AdvSimd.And(AdvSimd.ShiftRightArithmetic(vector.AsSByte(), 7).AsByte(), weights);
                return AdvSimd.Arm64.AddAcross(bits.GetLower()).ToScalar() | (uint)AdvSimd.Arm64.AddAcross(bits.GetUpper()).ToScalar() << 8;
            }
            var mask = 0u;
            for (var i = 0; i < Vector128<byte>.Count; i++) mask |= (uint)(vector.GetElement(i) >> 7) << i;
            return mask;
        }
    }

    public static void CopyTo<T>(this Vector128<T> vector, Span<T> destination) where T : struct
    {
        if (destination.Length < Vector128<T>.Count) throw new ArgumentException("the destination is shorter than a vector", nameof(destination));
        Unsafe.WriteUnaligned(ref Unsafe.As<T, byte>(ref MemoryMarshal.GetReference(destination)), vector);
    }

    public static void CopyTo<T>(this Vector256<T> vector, Span<T> destination) where T : struct
    {
        if (destination.Length < Vector256<T>.Count) throw new ArgumentException("the destination is shorter than a vector", nameof(destination));
        Unsafe.WriteUnaligned(ref Unsafe.As<T, byte>(ref MemoryMarshal.GetReference(destination)), vector);
    }

    extension(Vector256)
    {
        public static Vector256<T> Load<T>(T* source) where T : unmanaged => Unsafe.ReadUnaligned<Vector256<T>>(source);

        public static void Store<T>(Vector256<T> source, T* destination) where T : unmanaged => Unsafe.WriteUnaligned(destination, source);
    }

    extension(Vector128)
    {
        public static Vector128<float> ConvertToSingle(Vector128<int> value) =>
            Sse2.IsSupported ? Sse2.ConvertToVector128Single(value) : AdvSimd.ConvertToSingle(value);

        public static Vector128<float> Narrow(Vector128<double> lower, Vector128<double> upper) =>
            Sse2.IsSupported ? Sse.MoveLowToHigh(Sse2.ConvertToVector128Single(lower), Sse2.ConvertToVector128Single(upper))
                : AdvSimd.Arm64.ConvertToSingleUpper(AdvSimd.Arm64.ConvertToSingleLower(lower), upper);
    }

    extension(Vector256)
    {
        public static Vector256<float> ConvertToSingle(Vector256<int> value) => Avx.ConvertToVector256Single(value);

        public static Vector256<float> Narrow(Vector256<double> lower, Vector256<double> upper) =>
            Vector256.Create(Avx.ConvertToVector128Single(lower), Avx.ConvertToVector128Single(upper));
    }
}
