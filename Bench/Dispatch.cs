#if NET9_0_OR_GREATER
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;
using GFXKit;

namespace Bench;

static unsafe class Dispatch
{
    const MethodImplOptions Inline = MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization;

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public static void Chain(byte* from, byte* to, nuint length)
    {
        if (length <= 16) Short(to, from, length);
        else if (length <= 32) Pair<Vector128<byte>>(to, from, length);
        else if (length <= 64) Pair<Vector256<byte>>(to, from, length);
        else if (length <= 96) Three<Vector256<byte>>(to, from, length);
        else if (length <= 128) Four<Vector256<byte>>(to, from, length);
        else Large(from, to, length);
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public static void Table(byte* from, byte* to, nuint length)
    {
        switch ((uint)(Math.Min(length - 1, 128) >> 4))
        {
            case 0:
                Short(to, from, length);
                break;
            case 1:
                Pair<Vector128<byte>>(to, from, length);
                break;
            case 2:
            case 3:
                Pair<Vector256<byte>>(to, from, length);
                break;
            case 4:
            case 5:
                Three<Vector256<byte>>(to, from, length);
                break;
            case 6:
            case 7:
                Four<Vector256<byte>>(to, from, length);
                break;
            default:
                if (length != 0) Large(from, to, length);
                break;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public static void Narrow(byte* from, byte* to, nuint length)
    {
        if (length <= 16) Short(to, from, length);
        else if (length <= 32) Pair<Vector128<byte>>(to, from, length);
        else if (length <= 48) Three<Vector128<byte>>(to, from, length);
        else if (length <= 64) Four<Vector128<byte>>(to, from, length);
        else if (length <= 96) Ends<Vector128<byte>>(to, from, length, 3);
        else if (length <= 128) Ends<Vector128<byte>>(to, from, length, 4);
        else Large(from, to, length);
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public static void NarrowLong(byte* from, byte* to, nuint length)
    {
        if (length <= 16) Short(to, from, length);
        else if (length <= 32) Pair<Vector128<byte>>(to, from, length);
        else if (length <= 48) Three<Vector128<byte>>(to, from, length);
        else if (length <= 64) Four<Vector128<byte>>(to, from, length);
        else if (length <= 96) Ends<Vector128<byte>>(to, from, length, 3);
        else if (length <= 128) Ends<Vector128<byte>>(to, from, length, 4);
        else if (length <= 192) Ends<Vector128<byte>>(to, from, length, 6);
        else if (length <= 256) Ends<Vector128<byte>>(to, from, length, 8);
        else Large(from, to, length);
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public static void NarrowFine(byte* from, byte* to, nuint length)
    {
        if (length <= 16) Short(to, from, length);
        else if (length <= 32) Pair<Vector128<byte>>(to, from, length);
        else if (length <= 48) Three<Vector128<byte>>(to, from, length);
        else if (length <= 64) Four<Vector128<byte>>(to, from, length);
        else if (length <= 96) Six<Vector128<byte>>(to, from, length);
        else if (length <= 128) Ends<Vector128<byte>>(to, from, length, 4);
        else if (length <= 160) Ends<Vector128<byte>>(to, from, length, 5);
        else if (length <= 192) Ends<Vector128<byte>>(to, from, length, 6);
        else if (length <= 224) Ends<Vector128<byte>>(to, from, length, 7);
        else if (length <= 256) Ends<Vector128<byte>>(to, from, length, 8);
        else Large(from, to, length);
    }

    [MethodImpl(Inline)]
    static void Six<T>(byte* to, byte* from, nuint length) where T : unmanaged
    {
        var last = from + length - 2 * (nuint)sizeof(T);
        var end = to + length - 2 * (nuint)sizeof(T);
        var a = Unsafe.ReadUnaligned<T>(from);
        var b = Unsafe.ReadUnaligned<T>(from + sizeof(T));
        var c = Unsafe.ReadUnaligned<T>(from + 2 * sizeof(T));
        var d = Unsafe.ReadUnaligned<T>(from + 3 * sizeof(T));
        var e = Unsafe.ReadUnaligned<T>(last);
        var f = Unsafe.ReadUnaligned<T>(last + sizeof(T));
        Unsafe.WriteUnaligned(to, a);
        Unsafe.WriteUnaligned(to + sizeof(T), b);
        Unsafe.WriteUnaligned(to + 2 * sizeof(T), c);
        Unsafe.WriteUnaligned(to + 3 * sizeof(T), d);
        Unsafe.WriteUnaligned(end, e);
        Unsafe.WriteUnaligned(end + sizeof(T), f);
    }

    [MethodImpl(Inline)]
    static void Ends<T>(byte* to, byte* from, nuint length, int half) where T : unmanaged
    {
        var last = from + length - (nuint)(half * sizeof(T));
        var end = to + length - (nuint)(half * sizeof(T));
        if (half == 3)
        {
            var a = Unsafe.ReadUnaligned<T>(from);
            var b = Unsafe.ReadUnaligned<T>(from + sizeof(T));
            var c = Unsafe.ReadUnaligned<T>(from + 2 * sizeof(T));
            var d = Unsafe.ReadUnaligned<T>(last);
            var e = Unsafe.ReadUnaligned<T>(last + sizeof(T));
            var f = Unsafe.ReadUnaligned<T>(last + 2 * sizeof(T));
            Unsafe.WriteUnaligned(to, a);
            Unsafe.WriteUnaligned(to + sizeof(T), b);
            Unsafe.WriteUnaligned(to + 2 * sizeof(T), c);
            Unsafe.WriteUnaligned(end, d);
            Unsafe.WriteUnaligned(end + sizeof(T), e);
            Unsafe.WriteUnaligned(end + 2 * sizeof(T), f);
        }
        else if (half == 4)
        {
            var a = Unsafe.ReadUnaligned<T>(from);
            var b = Unsafe.ReadUnaligned<T>(from + sizeof(T));
            var c = Unsafe.ReadUnaligned<T>(from + 2 * sizeof(T));
            var d = Unsafe.ReadUnaligned<T>(from + 3 * sizeof(T));
            var e = Unsafe.ReadUnaligned<T>(last);
            var f = Unsafe.ReadUnaligned<T>(last + sizeof(T));
            var g = Unsafe.ReadUnaligned<T>(last + 2 * sizeof(T));
            var h = Unsafe.ReadUnaligned<T>(last + 3 * sizeof(T));
            Unsafe.WriteUnaligned(to, a);
            Unsafe.WriteUnaligned(to + sizeof(T), b);
            Unsafe.WriteUnaligned(to + 2 * sizeof(T), c);
            Unsafe.WriteUnaligned(to + 3 * sizeof(T), d);
            Unsafe.WriteUnaligned(end, e);
            Unsafe.WriteUnaligned(end + sizeof(T), f);
            Unsafe.WriteUnaligned(end + 2 * sizeof(T), g);
            Unsafe.WriteUnaligned(end + 3 * sizeof(T), h);
        }
        else if (half == 5)
        {
            var a = Unsafe.ReadUnaligned<T>(from);
            var b = Unsafe.ReadUnaligned<T>(from + sizeof(T));
            var c = Unsafe.ReadUnaligned<T>(from + 2 * sizeof(T));
            var d = Unsafe.ReadUnaligned<T>(from + 3 * sizeof(T));
            var e = Unsafe.ReadUnaligned<T>(from + 4 * sizeof(T));
            var f = Unsafe.ReadUnaligned<T>(last);
            var g = Unsafe.ReadUnaligned<T>(last + sizeof(T));
            var h = Unsafe.ReadUnaligned<T>(last + 2 * sizeof(T));
            var i = Unsafe.ReadUnaligned<T>(last + 3 * sizeof(T));
            var j = Unsafe.ReadUnaligned<T>(last + 4 * sizeof(T));
            Unsafe.WriteUnaligned(to, a);
            Unsafe.WriteUnaligned(to + sizeof(T), b);
            Unsafe.WriteUnaligned(to + 2 * sizeof(T), c);
            Unsafe.WriteUnaligned(to + 3 * sizeof(T), d);
            Unsafe.WriteUnaligned(to + 4 * sizeof(T), e);
            Unsafe.WriteUnaligned(end, f);
            Unsafe.WriteUnaligned(end + sizeof(T), g);
            Unsafe.WriteUnaligned(end + 2 * sizeof(T), h);
            Unsafe.WriteUnaligned(end + 3 * sizeof(T), i);
            Unsafe.WriteUnaligned(end + 4 * sizeof(T), j);
        }
        else if (half == 7)
        {
            var a = Unsafe.ReadUnaligned<T>(from);
            var b = Unsafe.ReadUnaligned<T>(from + sizeof(T));
            var c = Unsafe.ReadUnaligned<T>(from + 2 * sizeof(T));
            var d = Unsafe.ReadUnaligned<T>(from + 3 * sizeof(T));
            var e = Unsafe.ReadUnaligned<T>(from + 4 * sizeof(T));
            var f = Unsafe.ReadUnaligned<T>(from + 5 * sizeof(T));
            var g = Unsafe.ReadUnaligned<T>(from + 6 * sizeof(T));
            var h = Unsafe.ReadUnaligned<T>(last);
            var i = Unsafe.ReadUnaligned<T>(last + sizeof(T));
            var j = Unsafe.ReadUnaligned<T>(last + 2 * sizeof(T));
            var k = Unsafe.ReadUnaligned<T>(last + 3 * sizeof(T));
            var l = Unsafe.ReadUnaligned<T>(last + 4 * sizeof(T));
            var m = Unsafe.ReadUnaligned<T>(last + 5 * sizeof(T));
            var n = Unsafe.ReadUnaligned<T>(last + 6 * sizeof(T));
            Unsafe.WriteUnaligned(to, a);
            Unsafe.WriteUnaligned(to + sizeof(T), b);
            Unsafe.WriteUnaligned(to + 2 * sizeof(T), c);
            Unsafe.WriteUnaligned(to + 3 * sizeof(T), d);
            Unsafe.WriteUnaligned(to + 4 * sizeof(T), e);
            Unsafe.WriteUnaligned(to + 5 * sizeof(T), f);
            Unsafe.WriteUnaligned(to + 6 * sizeof(T), g);
            Unsafe.WriteUnaligned(end, h);
            Unsafe.WriteUnaligned(end + sizeof(T), i);
            Unsafe.WriteUnaligned(end + 2 * sizeof(T), j);
            Unsafe.WriteUnaligned(end + 3 * sizeof(T), k);
            Unsafe.WriteUnaligned(end + 4 * sizeof(T), l);
            Unsafe.WriteUnaligned(end + 5 * sizeof(T), m);
            Unsafe.WriteUnaligned(end + 6 * sizeof(T), n);
        }
        else if (half == 6)
        {
            var a = Unsafe.ReadUnaligned<T>(from);
            var b = Unsafe.ReadUnaligned<T>(from + sizeof(T));
            var c = Unsafe.ReadUnaligned<T>(from + 2 * sizeof(T));
            var d = Unsafe.ReadUnaligned<T>(from + 3 * sizeof(T));
            var e = Unsafe.ReadUnaligned<T>(from + 4 * sizeof(T));
            var f = Unsafe.ReadUnaligned<T>(from + 5 * sizeof(T));
            var g = Unsafe.ReadUnaligned<T>(last);
            var h = Unsafe.ReadUnaligned<T>(last + sizeof(T));
            var i = Unsafe.ReadUnaligned<T>(last + 2 * sizeof(T));
            var j = Unsafe.ReadUnaligned<T>(last + 3 * sizeof(T));
            var k = Unsafe.ReadUnaligned<T>(last + 4 * sizeof(T));
            var l = Unsafe.ReadUnaligned<T>(last + 5 * sizeof(T));
            Unsafe.WriteUnaligned(to, a);
            Unsafe.WriteUnaligned(to + sizeof(T), b);
            Unsafe.WriteUnaligned(to + 2 * sizeof(T), c);
            Unsafe.WriteUnaligned(to + 3 * sizeof(T), d);
            Unsafe.WriteUnaligned(to + 4 * sizeof(T), e);
            Unsafe.WriteUnaligned(to + 5 * sizeof(T), f);
            Unsafe.WriteUnaligned(end, g);
            Unsafe.WriteUnaligned(end + sizeof(T), h);
            Unsafe.WriteUnaligned(end + 2 * sizeof(T), i);
            Unsafe.WriteUnaligned(end + 3 * sizeof(T), j);
            Unsafe.WriteUnaligned(end + 4 * sizeof(T), k);
            Unsafe.WriteUnaligned(end + 5 * sizeof(T), l);
        }
        else
        {
            var a = Unsafe.ReadUnaligned<T>(from);
            var b = Unsafe.ReadUnaligned<T>(from + sizeof(T));
            var c = Unsafe.ReadUnaligned<T>(from + 2 * sizeof(T));
            var d = Unsafe.ReadUnaligned<T>(from + 3 * sizeof(T));
            var e = Unsafe.ReadUnaligned<T>(from + 4 * sizeof(T));
            var f = Unsafe.ReadUnaligned<T>(from + 5 * sizeof(T));
            var g = Unsafe.ReadUnaligned<T>(from + 6 * sizeof(T));
            var h = Unsafe.ReadUnaligned<T>(from + 7 * sizeof(T));
            var i = Unsafe.ReadUnaligned<T>(last);
            var j = Unsafe.ReadUnaligned<T>(last + sizeof(T));
            var k = Unsafe.ReadUnaligned<T>(last + 2 * sizeof(T));
            var l = Unsafe.ReadUnaligned<T>(last + 3 * sizeof(T));
            var m = Unsafe.ReadUnaligned<T>(last + 4 * sizeof(T));
            var n = Unsafe.ReadUnaligned<T>(last + 5 * sizeof(T));
            var o = Unsafe.ReadUnaligned<T>(last + 6 * sizeof(T));
            var p = Unsafe.ReadUnaligned<T>(last + 7 * sizeof(T));
            Unsafe.WriteUnaligned(to, a);
            Unsafe.WriteUnaligned(to + sizeof(T), b);
            Unsafe.WriteUnaligned(to + 2 * sizeof(T), c);
            Unsafe.WriteUnaligned(to + 3 * sizeof(T), d);
            Unsafe.WriteUnaligned(to + 4 * sizeof(T), e);
            Unsafe.WriteUnaligned(to + 5 * sizeof(T), f);
            Unsafe.WriteUnaligned(to + 6 * sizeof(T), g);
            Unsafe.WriteUnaligned(to + 7 * sizeof(T), h);
            Unsafe.WriteUnaligned(end, i);
            Unsafe.WriteUnaligned(end + sizeof(T), j);
            Unsafe.WriteUnaligned(end + 2 * sizeof(T), k);
            Unsafe.WriteUnaligned(end + 3 * sizeof(T), l);
            Unsafe.WriteUnaligned(end + 4 * sizeof(T), m);
            Unsafe.WriteUnaligned(end + 5 * sizeof(T), n);
            Unsafe.WriteUnaligned(end + 6 * sizeof(T), o);
            Unsafe.WriteUnaligned(end + 7 * sizeof(T), p);
        }
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    static void Large(byte* from, byte* to, nuint length) => MemoryCopy.CopyUnsafe((nint)from, (nint)to, length);

    [MethodImpl(Inline)]
    static void Short(byte* to, byte* from, nuint length)
    {
        if (length >= 8) Pair<ulong>(to, from, length);
        else if (length >= 4) Pair<uint>(to, from, length);
        else if (length >= 2) Pair<ushort>(to, from, length);
        else if (length != 0) *to = *from;
    }

    [MethodImpl(Inline)]
    static void Pair<T>(byte* to, byte* from, nuint length) where T : unmanaged
    {
        var a = Unsafe.ReadUnaligned<T>(from);
        var b = Unsafe.ReadUnaligned<T>(from + length - (nuint)sizeof(T));
        Unsafe.WriteUnaligned(to, a);
        Unsafe.WriteUnaligned(to + length - (nuint)sizeof(T), b);
    }

    [MethodImpl(Inline)]
    static void Three<T>(byte* to, byte* from, nuint length) where T : unmanaged
    {
        var a = Unsafe.ReadUnaligned<T>(from);
        var b = Unsafe.ReadUnaligned<T>(from + sizeof(T));
        var c = Unsafe.ReadUnaligned<T>(from + length - (nuint)sizeof(T));
        Unsafe.WriteUnaligned(to, a);
        Unsafe.WriteUnaligned(to + sizeof(T), b);
        Unsafe.WriteUnaligned(to + length - (nuint)sizeof(T), c);
    }

    [MethodImpl(Inline)]
    static void Four<T>(byte* to, byte* from, nuint length) where T : unmanaged
    {
        var last = from + length - 2 * (nuint)sizeof(T);
        var end = to + length - 2 * (nuint)sizeof(T);
        var a = Unsafe.ReadUnaligned<T>(from);
        var b = Unsafe.ReadUnaligned<T>(from + sizeof(T));
        var c = Unsafe.ReadUnaligned<T>(last);
        var d = Unsafe.ReadUnaligned<T>(last + sizeof(T));
        Unsafe.WriteUnaligned(to, a);
        Unsafe.WriteUnaligned(to + sizeof(T), b);
        Unsafe.WriteUnaligned(end, c);
        Unsafe.WriteUnaligned(end + sizeof(T), d);
    }
}
#endif
