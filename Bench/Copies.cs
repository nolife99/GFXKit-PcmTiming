#if NET9_0_OR_GREATER
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics;
using BenchmarkDotNet.Attributes;

namespace Bench;

public unsafe class Copies
{
    const int Size = 4096;
    const int Stream = 48 * Size;
    const int Page = 4096;

    byte* bytes;
    byte* plane;
    byte* to;
    int source;
    delegate*<byte*, byte*, nuint, void> copy;

    [ParamsSource(nameof(Methods))]
    public string Method { get; set; } = "CopyTo";

    [Params("0/0", "4/0", "0/4", "20/52")]
    public string Offsets { get; set; } = "0/0";

    [Params(0, 1, 4, 16, 62)]
    public int Skew { get; set; }

    public static IEnumerable<string> Methods()
    {
        yield return "CopyTo";
        var width = Vector512.IsHardwareAccelerated ? "512" : Vector256.IsHardwareAccelerated ? "256" : null;
        if (width is null) yield break;
        yield return "Dest" + width;
        yield return "Source" + width;
        yield return "Back" + width;
    }

    [GlobalSetup]
    public void Setup()
    {
        var parts = Offsets.Split('/');
        source = int.Parse(parts[0]);
        bytes = (byte*)NativeMemory.AlignedAlloc(Stream + Page, Page);
        plane = (byte*)NativeMemory.AlignedAlloc(Size + 2 * Page, Page);
        new Random(1).NextBytes(new Span<byte>(bytes, Stream + Page));
        new Random(2).NextBytes(new Span<byte>(plane, Size + 2 * Page));
        to = plane + Skew * 64 + int.Parse(parts[1]);
        copy = Method switch
        {
            "CopyTo" => &CopyTo,
            "Dest512" => &Dest512,
            "Source512" => &Source512,
            "Back512" => &Back512,
            "Dest256" => &Dest256,
            "Source256" => &Source256,
            _ => &Back256
        };
        var expected = new byte[Size + 64];
        var actual = new byte[Size + 64];
        ReadOnlySpan<int> lengths = [0, 1, 17, 63, 64, 127, 128, 255, 256, 257, 511, 1000, Size];
        fixed (byte* e = expected, a = actual)
            for (var offset = 0; offset < 64; offset += 9)
                for (var i = 0; i < lengths.Length; i++)
                {
                    expected.AsSpan().Clear();
                    actual.AsSpan().Clear();
                    CopyTo(bytes + offset, e + offset % 7, (nuint)lengths[i]);
                    copy(bytes + offset, a + offset % 7, (nuint)lengths[i]);
                    if (!expected.AsSpan().SequenceEqual(actual)) throw new InvalidOperationException($"{Method} copied {lengths[i]} bytes from offset {offset} wrongly");
                }
    }

    [GlobalCleanup]
    public void Cleanup()
    {
        NativeMemory.AlignedFree(bytes);
        NativeMemory.AlignedFree(plane);
    }

    [Benchmark]
    public void Run()
    {
        for (var at = source; at + Size <= Stream; at += Size) copy(bytes + at, to, Size);
    }

    static void CopyTo(byte* from, byte* to, nuint length) => new ReadOnlySpan<byte>(from, (int)length).CopyTo(new Span<byte>(to, (int)length));

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    static void Dest512(byte* from, byte* to, nuint length)
    {
        if (length < 256)
        {
            CopyTo(from, to, length);
            return;
        }
        var end = to + length;
        var last = from + length;
        Vector512.Store(Vector512.Load(from), to);
        var skip = 64 - ((nuint)to & 63);
        from += skip;
        to += skip;
        for (; to + 256 <= end; from += 256, to += 256)
        {
            var a = Vector512.Load(from);
            var b = Vector512.Load(from + 64);
            var c = Vector512.Load(from + 128);
            var d = Vector512.Load(from + 192);
            Vector512.StoreAligned(a, to);
            Vector512.StoreAligned(b, to + 64);
            Vector512.StoreAligned(c, to + 128);
            Vector512.StoreAligned(d, to + 192);
        }
        Tail512(last, end);
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    static void Source512(byte* from, byte* to, nuint length)
    {
        if (length < 256)
        {
            CopyTo(from, to, length);
            return;
        }
        var end = to + length;
        var last = from + length;
        Vector512.Store(Vector512.Load(from), to);
        var skip = 64 - ((nuint)from & 63);
        from += skip;
        to += skip;
        for (; to + 256 <= end; from += 256, to += 256)
        {
            var a = Vector512.LoadAligned(from);
            var b = Vector512.LoadAligned(from + 64);
            var c = Vector512.LoadAligned(from + 128);
            var d = Vector512.LoadAligned(from + 192);
            Vector512.Store(a, to);
            Vector512.Store(b, to + 64);
            Vector512.Store(c, to + 128);
            Vector512.Store(d, to + 192);
        }
        Tail512(last, end);
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    static void Back512(byte* from, byte* to, nuint length)
    {
        if (length < 256)
        {
            CopyTo(from, to, length);
            return;
        }
        var start = to;
        var first = from;
        var end = to + length;
        var last = from + length;
        Vector512.Store(Vector512.Load(last - 64), end - 64);
        var skip = (nuint)end & 63;
        last -= skip;
        end -= skip;
        for (; end - 256 >= start; last -= 256, end -= 256)
        {
            var a = Vector512.Load(last - 64);
            var b = Vector512.Load(last - 128);
            var c = Vector512.Load(last - 192);
            var d = Vector512.Load(last - 256);
            Vector512.StoreAligned(a, end - 64);
            Vector512.StoreAligned(b, end - 128);
            Vector512.StoreAligned(c, end - 192);
            Vector512.StoreAligned(d, end - 256);
        }
        var e = Vector512.Load(first + 192);
        var f = Vector512.Load(first + 128);
        var g = Vector512.Load(first + 64);
        var h = Vector512.Load(first);
        Vector512.Store(e, start + 192);
        Vector512.Store(f, start + 128);
        Vector512.Store(g, start + 64);
        Vector512.Store(h, start);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static void Tail512(byte* last, byte* end)
    {
        var a = Vector512.Load(last - 256);
        var b = Vector512.Load(last - 192);
        var c = Vector512.Load(last - 128);
        var d = Vector512.Load(last - 64);
        Vector512.Store(a, end - 256);
        Vector512.Store(b, end - 192);
        Vector512.Store(c, end - 128);
        Vector512.Store(d, end - 64);
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    static void Dest256(byte* from, byte* to, nuint length)
    {
        if (length < 128)
        {
            CopyTo(from, to, length);
            return;
        }
        var end = to + length;
        var last = from + length;
        Vector256.Store(Vector256.Load(from), to);
        var skip = 32 - ((nuint)to & 31);
        from += skip;
        to += skip;
        for (; to + 128 <= end; from += 128, to += 128)
        {
            var a = Vector256.Load(from);
            var b = Vector256.Load(from + 32);
            var c = Vector256.Load(from + 64);
            var d = Vector256.Load(from + 96);
            Vector256.StoreAligned(a, to);
            Vector256.StoreAligned(b, to + 32);
            Vector256.StoreAligned(c, to + 64);
            Vector256.StoreAligned(d, to + 96);
        }
        Tail256(last, end);
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    static void Source256(byte* from, byte* to, nuint length)
    {
        if (length < 128)
        {
            CopyTo(from, to, length);
            return;
        }
        var end = to + length;
        var last = from + length;
        Vector256.Store(Vector256.Load(from), to);
        var skip = 32 - ((nuint)from & 31);
        from += skip;
        to += skip;
        for (; to + 128 <= end; from += 128, to += 128)
        {
            var a = Vector256.LoadAligned(from);
            var b = Vector256.LoadAligned(from + 32);
            var c = Vector256.LoadAligned(from + 64);
            var d = Vector256.LoadAligned(from + 96);
            Vector256.Store(a, to);
            Vector256.Store(b, to + 32);
            Vector256.Store(c, to + 64);
            Vector256.Store(d, to + 96);
        }
        Tail256(last, end);
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    static void Back256(byte* from, byte* to, nuint length)
    {
        if (length < 128)
        {
            CopyTo(from, to, length);
            return;
        }
        var start = to;
        var first = from;
        var end = to + length;
        var last = from + length;
        Vector256.Store(Vector256.Load(last - 32), end - 32);
        var skip = (nuint)end & 31;
        last -= skip;
        end -= skip;
        for (; end - 128 >= start; last -= 128, end -= 128)
        {
            var a = Vector256.Load(last - 32);
            var b = Vector256.Load(last - 64);
            var c = Vector256.Load(last - 96);
            var d = Vector256.Load(last - 128);
            Vector256.StoreAligned(a, end - 32);
            Vector256.StoreAligned(b, end - 64);
            Vector256.StoreAligned(c, end - 96);
            Vector256.StoreAligned(d, end - 128);
        }
        var e = Vector256.Load(first + 96);
        var f = Vector256.Load(first + 64);
        var g = Vector256.Load(first + 32);
        var h = Vector256.Load(first);
        Vector256.Store(e, start + 96);
        Vector256.Store(f, start + 64);
        Vector256.Store(g, start + 32);
        Vector256.Store(h, start);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static void Tail256(byte* last, byte* end)
    {
        var a = Vector256.Load(last - 128);
        var b = Vector256.Load(last - 96);
        var c = Vector256.Load(last - 64);
        var d = Vector256.Load(last - 32);
        Vector256.Store(a, end - 128);
        Vector256.Store(b, end - 96);
        Vector256.Store(c, end - 64);
        Vector256.Store(d, end - 32);
    }
}
#endif
