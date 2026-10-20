using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;

namespace GFXKit;

public static partial class MemoryCopy
{
    static partial class Thresholds
    {
        public static readonly bool Wide = Vector512.IsHardwareAccelerated && Zen5();

        static bool Zen5()
        {
            var (_, b, c, d) = X86Base.CpuId(0, 0);
            if (!Caches.AuthenticAmd(b, c, d)) return false;
            var signature = (uint)X86Base.CpuId(1, 0).Eax;
            return (signature >> 8 & 0xF) + (signature >> 20 & 0xFF) >= 0x1A;
        }
    }

    [MethodImpl(Inline)]
    static void Wide(ref byte to, ref byte from, nuint length)
    {
        if (length <= 192) Three<Vector512<byte>>(ref to, ref from, length);
        else if (length <= 256) Four<Vector512<byte>>(ref to, ref from, length);
        else if (Behind(ref to, ref from, length)) Backward<Vector512<byte>>(ref to, ref from, length);
        else Forward<Vector512<byte>>(ref to, ref from, length);
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    internal static void MoveWide(ref byte to, ref byte from, nuint length)
    {
        if (length <= 128) Move(ref to, ref from, length, CopyHint.Cached);
        else Wide(ref to, ref from, length);
    }
}
