using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.Arm;

static class Net8Intrinsics
{
    extension(AdvSimd.Arm64)
    {
        public static Vector128<byte> VectorTableLookup((Vector128<byte>, Vector128<byte>, Vector128<byte>) table, Vector128<byte> byteIndexes) =>
            AdvSimd.Arm64.VectorTableLookupExtension(AdvSimd.Arm64.VectorTableLookupExtension(AdvSimd.Arm64.VectorTableLookup(table.Item1, byteIndexes), table.Item2,
                AdvSimd.Subtract(byteIndexes, Vector128.Create((byte)16))), table.Item3, AdvSimd.Subtract(byteIndexes, Vector128.Create((byte)32)));
    }
}
