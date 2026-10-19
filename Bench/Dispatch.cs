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
