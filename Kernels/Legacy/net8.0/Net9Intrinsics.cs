using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.Arm;

static unsafe class Net9Intrinsics
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static (Vector128<byte>, Vector128<byte>, Vector128<byte>) Load3(byte* at) => (AdvSimd.LoadVector128(at), AdvSimd.LoadVector128(at + 16), AdvSimd.LoadVector128(at + 32));

    extension(AdvSimd.Arm64)
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static (Vector128<ushort>, Vector128<ushort>) Load2xVector128AndUnzip(ushort* address)
        {
            var (a, b) = (AdvSimd.LoadVector128(address), AdvSimd.LoadVector128(address + 8));
            return (AdvSimd.Arm64.UnzipEven(a, b), AdvSimd.Arm64.UnzipOdd(a, b));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static (Vector128<uint>, Vector128<uint>) Load2xVector128AndUnzip(uint* address)
        {
            var (a, b) = (AdvSimd.LoadVector128(address), AdvSimd.LoadVector128(address + 4));
            return (AdvSimd.Arm64.UnzipEven(a, b), AdvSimd.Arm64.UnzipOdd(a, b));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static (Vector128<byte>, Vector128<byte>, Vector128<byte>) Load3xVector128AndUnzip(byte* address)
        {
            var table = Load3(address);
            return (AdvSimd.Arm64.VectorTableLookup(table, Vector128.Create((byte)0, 3, 6, 9, 12, 15, 18, 21, 24, 27, 30, 33, 36, 39, 42, 45)), AdvSimd.Arm64.VectorTableLookup(table, Vector128.Create((byte)1, 4, 7, 10, 13, 16, 19, 22, 25, 28, 31, 34, 37, 40, 43, 46)),
                AdvSimd.Arm64.VectorTableLookup(table, Vector128.Create((byte)2, 5, 8, 11, 14, 17, 20, 23, 26, 29, 32, 35, 38, 41, 44, 47)));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static (Vector128<ushort>, Vector128<ushort>, Vector128<ushort>) Load3xVector128AndUnzip(ushort* address)
        {
            var table = Load3((byte*)address);
            return (AdvSimd.Arm64.VectorTableLookup(table, Vector128.Create((byte)0, 1, 6, 7, 12, 13, 18, 19, 24, 25, 30, 31, 36, 37, 42, 43)).AsUInt16(), AdvSimd.Arm64.VectorTableLookup(table, Vector128.Create((byte)2, 3, 8, 9, 14, 15, 20, 21, 26, 27, 32, 33, 38, 39, 44, 45)).AsUInt16(),
                AdvSimd.Arm64.VectorTableLookup(table, Vector128.Create((byte)4, 5, 10, 11, 16, 17, 22, 23, 28, 29, 34, 35, 40, 41, 46, 47)).AsUInt16());
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static (Vector128<uint>, Vector128<uint>, Vector128<uint>) Load3xVector128AndUnzip(uint* address)
        {
            var table = Load3((byte*)address);
            return (AdvSimd.Arm64.VectorTableLookup(table, Vector128.Create((byte)0, 1, 2, 3, 12, 13, 14, 15, 24, 25, 26, 27, 36, 37, 38, 39)).AsUInt32(), AdvSimd.Arm64.VectorTableLookup(table, Vector128.Create((byte)4, 5, 6, 7, 16, 17, 18, 19, 28, 29, 30, 31, 40, 41, 42, 43)).AsUInt32(),
                AdvSimd.Arm64.VectorTableLookup(table, Vector128.Create((byte)8, 9, 10, 11, 20, 21, 22, 23, 32, 33, 34, 35, 44, 45, 46, 47)).AsUInt32());
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static (Vector128<ulong>, Vector128<ulong>, Vector128<ulong>) Load3xVector128AndUnzip(ulong* address)
        {
            var table = Load3((byte*)address);
            return (AdvSimd.Arm64.VectorTableLookup(table, Vector128.Create((byte)0, 1, 2, 3, 4, 5, 6, 7, 24, 25, 26, 27, 28, 29, 30, 31)).AsUInt64(), AdvSimd.Arm64.VectorTableLookup(table, Vector128.Create((byte)8, 9, 10, 11, 12, 13, 14, 15, 32, 33, 34, 35, 36, 37, 38, 39)).AsUInt64(),
                AdvSimd.Arm64.VectorTableLookup(table, Vector128.Create((byte)16, 17, 18, 19, 20, 21, 22, 23, 40, 41, 42, 43, 44, 45, 46, 47)).AsUInt64());
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void StoreVectorAndZip(ushort* address, (Vector128<ushort>, Vector128<ushort>, Vector128<ushort>) value)
        {
            var table = (value.Item1.AsByte(), value.Item2.AsByte(), value.Item3.AsByte());
            AdvSimd.Store((byte*)address, AdvSimd.Arm64.VectorTableLookup(table, Vector128.Create((byte)0, 1, 16, 17, 32, 33, 2, 3, 18, 19, 34, 35, 4, 5, 20, 21)));
            AdvSimd.Store((byte*)address + 16, AdvSimd.Arm64.VectorTableLookup(table, Vector128.Create((byte)36, 37, 6, 7, 22, 23, 38, 39, 8, 9, 24, 25, 40, 41, 10, 11)));
            AdvSimd.Store((byte*)address + 32, AdvSimd.Arm64.VectorTableLookup(table, Vector128.Create((byte)26, 27, 42, 43, 12, 13, 28, 29, 44, 45, 14, 15, 30, 31, 46, 47)));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void StoreVectorAndZip(uint* address, (Vector128<uint>, Vector128<uint>, Vector128<uint>) value)
        {
            var table = (value.Item1.AsByte(), value.Item2.AsByte(), value.Item3.AsByte());
            AdvSimd.Store((byte*)address, AdvSimd.Arm64.VectorTableLookup(table, Vector128.Create((byte)0, 1, 2, 3, 16, 17, 18, 19, 32, 33, 34, 35, 4, 5, 6, 7)));
            AdvSimd.Store((byte*)address + 16, AdvSimd.Arm64.VectorTableLookup(table, Vector128.Create((byte)20, 21, 22, 23, 36, 37, 38, 39, 8, 9, 10, 11, 24, 25, 26, 27)));
            AdvSimd.Store((byte*)address + 32, AdvSimd.Arm64.VectorTableLookup(table, Vector128.Create((byte)40, 41, 42, 43, 12, 13, 14, 15, 28, 29, 30, 31, 44, 45, 46, 47)));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void StoreVectorAndZip(ulong* address, (Vector128<ulong>, Vector128<ulong>, Vector128<ulong>) value)
        {
            var table = (value.Item1.AsByte(), value.Item2.AsByte(), value.Item3.AsByte());
            AdvSimd.Store((byte*)address, AdvSimd.Arm64.VectorTableLookup(table, Vector128.Create((byte)0, 1, 2, 3, 4, 5, 6, 7, 16, 17, 18, 19, 20, 21, 22, 23)));
            AdvSimd.Store((byte*)address + 16, AdvSimd.Arm64.VectorTableLookup(table, Vector128.Create((byte)32, 33, 34, 35, 36, 37, 38, 39, 8, 9, 10, 11, 12, 13, 14, 15)));
            AdvSimd.Store((byte*)address + 32, AdvSimd.Arm64.VectorTableLookup(table, Vector128.Create((byte)24, 25, 26, 27, 28, 29, 30, 31, 40, 41, 42, 43, 44, 45, 46, 47)));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void StoreVectorAndZip(uint* address, (Vector128<uint>, Vector128<uint>, Vector128<uint>, Vector128<uint>) value)
        {
            var (l0, h0) = (AdvSimd.Arm64.ZipLow(value.Item1, value.Item3), AdvSimd.Arm64.ZipHigh(value.Item1, value.Item3));
            var (l1, h1) = (AdvSimd.Arm64.ZipLow(value.Item2, value.Item4), AdvSimd.Arm64.ZipHigh(value.Item2, value.Item4));
            AdvSimd.Store(address, AdvSimd.Arm64.ZipLow(l0, l1));
            AdvSimd.Store(address + 4, AdvSimd.Arm64.ZipHigh(l0, l1));
            AdvSimd.Store(address + 8, AdvSimd.Arm64.ZipLow(h0, h1));
            AdvSimd.Store(address + 12, AdvSimd.Arm64.ZipHigh(h0, h1));
        }
    }
}
