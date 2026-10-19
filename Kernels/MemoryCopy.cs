using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics;

namespace GFXKit;

/// <summary>Copies memory with vector loads and stores the JIT compiles inline, at every size. <see cref="Span{T}.CopyTo"/> hands copies
/// over 2 KiB on x64 to the C runtime's <c>memmove</c>, whose <c>rep movsb</c> is slow on AMD's Zen 3 and Zen 4, most of all where the
/// destination lies just past the source modulo 4096; these copies never call out.</summary>
/// <remarks>Each copy is correct when the source and destination overlap, as <see cref="Buffer.MemoryCopy(void*, void*, long, long)"/>
/// is. A copy of elements holding references goes through <see cref="Span{T}.CopyTo"/>, which tells the garbage collector about each.</remarks>
public static unsafe partial class MemoryCopy
{
    /// <summary>Copies a span's elements to the start of another span.</summary>
    /// <param name="source">The elements to copy.</param>
    /// <param name="destination">The span to copy them to, at least as long as <paramref name="source"/>.</param>
    /// <typeparam name="T">The type of the elements.</typeparam>
    /// <exception cref="ArgumentException"><paramref name="destination"/> is shorter than <paramref name="source"/>.</exception>
    public static void Copy<T>(scoped ReadOnlySpan<T> source, scoped Span<T> destination)
    {
        if (!TryCopy(source, destination)) throw new ArgumentException("The destination is shorter than the source.", nameof(destination));
    }

    /// <summary>Copies a span's elements to the start of another span if they fit.</summary>
    /// <param name="source">The elements to copy.</param>
    /// <param name="destination">The span to copy them to.</param>
    /// <typeparam name="T">The type of the elements.</typeparam>
    /// <returns><see langword="true"/> if the elements were copied; <see langword="false"/>, copying nothing, if <paramref name="destination"/>
    /// is shorter than <paramref name="source"/>.</returns>
    public static bool TryCopy<T>(scoped ReadOnlySpan<T> source, scoped Span<T> destination)
    {
        if ((uint)source.Length > (uint)destination.Length) return false;
        if (RuntimeHelpers.IsReferenceOrContainsReferences<T>()) source.CopyTo(destination);
        else Move(ref Unsafe.As<T, byte>(ref MemoryMarshal.GetReference(destination)), ref Unsafe.As<T, byte>(ref MemoryMarshal.GetReference(source)),
            (nuint)source.Length * (nuint)Unsafe.SizeOf<T>());
        return true;
    }

    /// <summary>Copies bytes between two addresses, with no checks.</summary>
    /// <param name="source">The address of the bytes to copy.</param>
    /// <param name="destination">The address to copy them to.</param>
    /// <param name="byteCount">The number of bytes to copy.</param>
    /// <remarks>Both ranges must be memory the caller may read and write for as long as the copy runs, which pinned or native memory is.</remarks>
    public static void CopyUnsafe(nint source, nint destination, nuint byteCount) =>
        Move(ref Unsafe.AsRef<byte>((void*)destination), ref Unsafe.AsRef<byte>((void*)source), byteCount);

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    internal static void Move(ref byte to, ref byte from, nuint length)
    {
        if (length <= 16)
        {
            Short(ref to, ref from, length);
            return;
        }
        if (length <= 32)
        {
            var a = Vector128.LoadUnsafe(ref from);
            var b = Vector128.LoadUnsafe(ref from, length - 16);
            a.StoreUnsafe(ref to);
            b.StoreUnsafe(ref to, length - 16);
            return;
        }
        if (length <= 64)
        {
            if (Vector256.IsHardwareAccelerated)
            {
                var a = Vector256.LoadUnsafe(ref from);
                var b = Vector256.LoadUnsafe(ref from, length - 32);
                a.StoreUnsafe(ref to);
                b.StoreUnsafe(ref to, length - 32);
            }
            else Quad128(ref to, ref from, length);
            return;
        }
        if (Wide)
        {
            if (length <= 256)
            {
                Short512(ref to, ref from, length);
                return;
            }
        }
        else if (Vector256.IsHardwareAccelerated && length <= 128)
        {
            Quad256(ref to, ref from, length);
            return;
        }
        if ((nuint)(nint)Unsafe.ByteOffset(ref from, ref to) < length || (nuint)(nint)Unsafe.ByteOffset(ref to, ref from) < length) Overlapping(ref to, ref from, length);
        else if (Wide) Loop512(ref to, ref from, length);
        else if (Vector256.IsHardwareAccelerated) Loop256(ref to, ref from, length);
        else Loop128(ref to, ref from, length);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static void Short(ref byte to, ref byte from, nuint length)
    {
        if (length >= 8)
        {
            var a = Unsafe.ReadUnaligned<ulong>(ref from);
            var b = Unsafe.ReadUnaligned<ulong>(ref Unsafe.Add(ref from, length - 8));
            Unsafe.WriteUnaligned(ref to, a);
            Unsafe.WriteUnaligned(ref Unsafe.Add(ref to, length - 8), b);
        }
        else if (length >= 4)
        {
            var a = Unsafe.ReadUnaligned<uint>(ref from);
            var b = Unsafe.ReadUnaligned<uint>(ref Unsafe.Add(ref from, length - 4));
            Unsafe.WriteUnaligned(ref to, a);
            Unsafe.WriteUnaligned(ref Unsafe.Add(ref to, length - 4), b);
        }
        else if (length >= 2)
        {
            var a = Unsafe.ReadUnaligned<ushort>(ref from);
            var b = Unsafe.ReadUnaligned<ushort>(ref Unsafe.Add(ref from, length - 2));
            Unsafe.WriteUnaligned(ref to, a);
            Unsafe.WriteUnaligned(ref Unsafe.Add(ref to, length - 2), b);
        }
        else if (length != 0) to = from;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static void Quad128(ref byte to, ref byte from, nuint length)
    {
        var a = Vector128.LoadUnsafe(ref from);
        var b = Vector128.LoadUnsafe(ref from, 16);
        var c = Vector128.LoadUnsafe(ref from, length - 32);
        var d = Vector128.LoadUnsafe(ref from, length - 16);
        a.StoreUnsafe(ref to);
        b.StoreUnsafe(ref to, 16);
        c.StoreUnsafe(ref to, length - 32);
        d.StoreUnsafe(ref to, length - 16);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static void Quad256(ref byte to, ref byte from, nuint length)
    {
        var a = Vector256.LoadUnsafe(ref from);
        var b = Vector256.LoadUnsafe(ref from, 32);
        var c = Vector256.LoadUnsafe(ref from, length - 64);
        var d = Vector256.LoadUnsafe(ref from, length - 32);
        a.StoreUnsafe(ref to);
        b.StoreUnsafe(ref to, 32);
        c.StoreUnsafe(ref to, length - 64);
        d.StoreUnsafe(ref to, length - 32);
    }

    static void Overlapping(ref byte to, ref byte from, nuint length)
    {
        fixed (byte* target = &to, source = &from) Buffer.MemoryCopy(source, target, length, length);
    }

    static nuint Skip(ref byte to, nuint width) => width - ((nuint)Unsafe.AsPointer(ref to) & (width - 1));

    static void Loop128(ref byte to, ref byte from, nuint length)
    {
        Vector128.LoadUnsafe(ref from).StoreUnsafe(ref to);
        var at = Skip(ref to, 16);
        for (; at + 64 <= length; at += 64)
        {
            var a = Vector128.LoadUnsafe(ref from, at);
            var b = Vector128.LoadUnsafe(ref from, at + 16);
            var c = Vector128.LoadUnsafe(ref from, at + 32);
            var d = Vector128.LoadUnsafe(ref from, at + 48);
            a.StoreUnsafe(ref to, at);
            b.StoreUnsafe(ref to, at + 16);
            c.StoreUnsafe(ref to, at + 32);
            d.StoreUnsafe(ref to, at + 48);
        }
        var e = Vector128.LoadUnsafe(ref from, length - 64);
        var f = Vector128.LoadUnsafe(ref from, length - 48);
        var g = Vector128.LoadUnsafe(ref from, length - 32);
        var h = Vector128.LoadUnsafe(ref from, length - 16);
        e.StoreUnsafe(ref to, length - 64);
        f.StoreUnsafe(ref to, length - 48);
        g.StoreUnsafe(ref to, length - 32);
        h.StoreUnsafe(ref to, length - 16);
    }

    static void Loop256(ref byte to, ref byte from, nuint length)
    {
        Vector256.LoadUnsafe(ref from).StoreUnsafe(ref to);
        var at = Skip(ref to, 32);
        for (; at + 128 <= length; at += 128)
        {
            var a = Vector256.LoadUnsafe(ref from, at);
            var b = Vector256.LoadUnsafe(ref from, at + 32);
            var c = Vector256.LoadUnsafe(ref from, at + 64);
            var d = Vector256.LoadUnsafe(ref from, at + 96);
            a.StoreUnsafe(ref to, at);
            b.StoreUnsafe(ref to, at + 32);
            c.StoreUnsafe(ref to, at + 64);
            d.StoreUnsafe(ref to, at + 96);
        }
        var e = Vector256.LoadUnsafe(ref from, length - 128);
        var f = Vector256.LoadUnsafe(ref from, length - 96);
        var g = Vector256.LoadUnsafe(ref from, length - 64);
        var h = Vector256.LoadUnsafe(ref from, length - 32);
        e.StoreUnsafe(ref to, length - 128);
        f.StoreUnsafe(ref to, length - 96);
        g.StoreUnsafe(ref to, length - 64);
        h.StoreUnsafe(ref to, length - 32);
    }
}
