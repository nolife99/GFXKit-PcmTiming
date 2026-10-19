using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;

namespace GFXKit;

public static unsafe partial class MemoryCopy
{
    static bool Wide => Vector512.IsHardwareAccelerated;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static void Short512(ref byte to, ref byte from, nuint length)
    {
        if (length <= 128)
        {
            var a = Vector512.LoadUnsafe(ref from);
            var b = Vector512.LoadUnsafe(ref from, length - 64);
            a.StoreUnsafe(ref to);
            b.StoreUnsafe(ref to, length - 64);
            return;
        }
        var c = Vector512.LoadUnsafe(ref from);
        var d = Vector512.LoadUnsafe(ref from, 64);
        var e = Vector512.LoadUnsafe(ref from, length - 128);
        var f = Vector512.LoadUnsafe(ref from, length - 64);
        c.StoreUnsafe(ref to);
        d.StoreUnsafe(ref to, 64);
        e.StoreUnsafe(ref to, length - 128);
        f.StoreUnsafe(ref to, length - 64);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static void Loop512(ref byte to, ref byte from, nuint length)
    {
        Vector512.LoadUnsafe(ref from).StoreUnsafe(ref to);
        var at = Skip(ref to, 64);
        for (; at + 256 <= length; at += 256)
        {
            var a = Vector512.LoadUnsafe(ref from, at);
            var b = Vector512.LoadUnsafe(ref from, at + 64);
            var c = Vector512.LoadUnsafe(ref from, at + 128);
            var d = Vector512.LoadUnsafe(ref from, at + 192);
            a.StoreUnsafe(ref to, at);
            b.StoreUnsafe(ref to, at + 64);
            c.StoreUnsafe(ref to, at + 128);
            d.StoreUnsafe(ref to, at + 192);
        }
        var e = Vector512.LoadUnsafe(ref from, length - 256);
        var f = Vector512.LoadUnsafe(ref from, length - 192);
        var g = Vector512.LoadUnsafe(ref from, length - 128);
        var h = Vector512.LoadUnsafe(ref from, length - 64);
        e.StoreUnsafe(ref to, length - 256);
        f.StoreUnsafe(ref to, length - 192);
        g.StoreUnsafe(ref to, length - 128);
        h.StoreUnsafe(ref to, length - 64);
    }
}
