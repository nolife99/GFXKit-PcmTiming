using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics;

namespace GFXKit;

/// <summary>Copies memory with vector loads and stores the JIT compiles inline, at every size. <see cref="Span{T}.CopyTo"/> hands copies
/// over 2 KiB on x64 to the C runtime's <c>memmove</c>, whose <c>rep movsb</c> is slow on AMD's Zen 3 and Zen 4, most of all where the
/// destination lies just past the source modulo 4096; these copies never call out.</summary>
/// <remarks>Each copy is correct when the source and destination overlap, as <see cref="Buffer.MemoryCopy(void*, void*, long, long)"/>
/// is. A copy of elements holding references goes through <see cref="Span{T}.CopyTo"/>, which tells the garbage collector about each.</remarks>
public static unsafe class MemoryCopy
{
    const MethodImplOptions Inline = MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization;

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
        if (length <= 16) Short(ref to, ref from, length);
        else if (length <= 32) Pair<Vector128<byte>>(ref to, ref from, length);
        else if (Vector256.IsHardwareAccelerated)
        {
            if (length <= 64) Pair<Vector256<byte>>(ref to, ref from, length);
            else if (length <= 96) Three<Vector256<byte>>(ref to, ref from, length);
            else if (length <= 128) Four<Vector256<byte>>(ref to, ref from, length);
            else if (Overlap(ref to, ref from, length)) Memmove(ref to, ref from, length);
            else Loop<Vector256<byte>>(ref to, ref from, length);
        }
        else if (length <= 48) Three<Vector128<byte>>(ref to, ref from, length);
        else if (length <= 64) Four<Vector128<byte>>(ref to, ref from, length);
        else if (length <= 128) Eight<Vector128<byte>>(ref to, ref from, length);
        else if (length < 256 || Overlap(ref to, ref from, length)) Memmove(ref to, ref from, length);
        else Loop<Vector128<byte>>(ref to, ref from, length);
    }

    [MethodImpl(Inline)]
    static void Short(ref byte to, ref byte from, nuint length)
    {
        if (length >= 8) Pair<ulong>(ref to, ref from, length);
        else if (length >= 4) Pair<uint>(ref to, ref from, length);
        else if (length >= 2) Pair<ushort>(ref to, ref from, length);
        else if (length != 0) to = from;
    }

    [MethodImpl(Inline)]
    static void Pair<T>(ref byte to, ref byte from, nuint length) where T : unmanaged
    {
        var width = (nuint)sizeof(T);
        var a = Unsafe.ReadUnaligned<T>(ref from);
        var b = Unsafe.ReadUnaligned<T>(ref Unsafe.Add(ref from, length - width));
        Unsafe.WriteUnaligned(ref to, a);
        Unsafe.WriteUnaligned(ref Unsafe.Add(ref to, length - width), b);
    }

    [MethodImpl(Inline)]
    static void Three<T>(ref byte to, ref byte from, nuint length) where T : unmanaged
    {
        var width = (nuint)sizeof(T);
        var a = Unsafe.ReadUnaligned<T>(ref from);
        var b = Unsafe.ReadUnaligned<T>(ref Unsafe.Add(ref from, width));
        var c = Unsafe.ReadUnaligned<T>(ref Unsafe.Add(ref from, length - width));
        Unsafe.WriteUnaligned(ref to, a);
        Unsafe.WriteUnaligned(ref Unsafe.Add(ref to, width), b);
        Unsafe.WriteUnaligned(ref Unsafe.Add(ref to, length - width), c);
    }

    [MethodImpl(Inline)]
    static void Four<T>(ref byte to, ref byte from, nuint length) where T : unmanaged
    {
        var width = (nuint)sizeof(T);
        var a = Unsafe.ReadUnaligned<T>(ref from);
        var b = Unsafe.ReadUnaligned<T>(ref Unsafe.Add(ref from, width));
        var c = Unsafe.ReadUnaligned<T>(ref Unsafe.Add(ref from, length - 2 * width));
        var d = Unsafe.ReadUnaligned<T>(ref Unsafe.Add(ref from, length - width));
        Unsafe.WriteUnaligned(ref to, a);
        Unsafe.WriteUnaligned(ref Unsafe.Add(ref to, width), b);
        Unsafe.WriteUnaligned(ref Unsafe.Add(ref to, length - 2 * width), c);
        Unsafe.WriteUnaligned(ref Unsafe.Add(ref to, length - width), d);
    }

    [MethodImpl(Inline)]
    static void Eight<T>(ref byte to, ref byte from, nuint length) where T : unmanaged
    {
        var width = (nuint)sizeof(T);
        var a = Unsafe.ReadUnaligned<T>(ref from);
        var b = Unsafe.ReadUnaligned<T>(ref Unsafe.Add(ref from, width));
        var c = Unsafe.ReadUnaligned<T>(ref Unsafe.Add(ref from, 2 * width));
        var d = Unsafe.ReadUnaligned<T>(ref Unsafe.Add(ref from, 3 * width));
        var e = Unsafe.ReadUnaligned<T>(ref Unsafe.Add(ref from, length - 4 * width));
        var f = Unsafe.ReadUnaligned<T>(ref Unsafe.Add(ref from, length - 3 * width));
        var g = Unsafe.ReadUnaligned<T>(ref Unsafe.Add(ref from, length - 2 * width));
        var h = Unsafe.ReadUnaligned<T>(ref Unsafe.Add(ref from, length - width));
        Unsafe.WriteUnaligned(ref to, a);
        Unsafe.WriteUnaligned(ref Unsafe.Add(ref to, width), b);
        Unsafe.WriteUnaligned(ref Unsafe.Add(ref to, 2 * width), c);
        Unsafe.WriteUnaligned(ref Unsafe.Add(ref to, 3 * width), d);
        Unsafe.WriteUnaligned(ref Unsafe.Add(ref to, length - 4 * width), e);
        Unsafe.WriteUnaligned(ref Unsafe.Add(ref to, length - 3 * width), f);
        Unsafe.WriteUnaligned(ref Unsafe.Add(ref to, length - 2 * width), g);
        Unsafe.WriteUnaligned(ref Unsafe.Add(ref to, length - width), h);
    }

    [MethodImpl(Inline)]
    static bool Overlap(ref byte to, ref byte from, nuint length) =>
        (nuint)(nint)Unsafe.ByteOffset(ref from, ref to) < length || (nuint)(nint)Unsafe.ByteOffset(ref to, ref from) < length;

    [MethodImpl(MethodImplOptions.NoInlining)]
    static void Memmove(ref byte to, ref byte from, nuint length)
    {
        fixed (byte* target = &to, source = &from) Buffer.MemoryCopy(source, target, length, length);
    }

    [MethodImpl(Inline)]
    static void Loop<T>(ref byte to, ref byte from, nuint length) where T : unmanaged
    {
        var width = (nuint)sizeof(T);
        Unsafe.WriteUnaligned(ref to, Unsafe.ReadUnaligned<T>(ref from));
        for (var at = width - ((nuint)Unsafe.AsPointer(ref to) & (width - 1)); at + 4 * width <= length; at += 4 * width)
        {
            var a = Unsafe.ReadUnaligned<T>(ref Unsafe.Add(ref from, at));
            var b = Unsafe.ReadUnaligned<T>(ref Unsafe.Add(ref from, at + width));
            var c = Unsafe.ReadUnaligned<T>(ref Unsafe.Add(ref from, at + 2 * width));
            var d = Unsafe.ReadUnaligned<T>(ref Unsafe.Add(ref from, at + 3 * width));
            Unsafe.WriteUnaligned(ref Unsafe.Add(ref to, at), a);
            Unsafe.WriteUnaligned(ref Unsafe.Add(ref to, at + width), b);
            Unsafe.WriteUnaligned(ref Unsafe.Add(ref to, at + 2 * width), c);
            Unsafe.WriteUnaligned(ref Unsafe.Add(ref to, at + 3 * width), d);
        }
        Four<T>(ref Unsafe.Add(ref to, length - 4 * width), ref Unsafe.Add(ref from, length - 4 * width), 4 * width);
    }
}
