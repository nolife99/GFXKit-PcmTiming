#if NET9_0_OR_GREATER
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics;
using BenchmarkDotNet.Attributes;
using GFXKit;

namespace Bench;

interface IShape
{
    static abstract bool Narrow { get; }
    static abstract bool Thirds { get; }
    static abstract bool Octo { get; }
    static abstract bool Unaligned { get; }
}

struct BaseShape : IShape
{
    public static bool Narrow => false;
    public static bool Thirds => false;
    public static bool Octo => false;
    public static bool Unaligned => false;
}

struct NarrowShape : IShape
{
    public static bool Narrow => true;
    public static bool Thirds => false;
    public static bool Octo => false;
    public static bool Unaligned => false;
}

struct ThirdsShape : IShape
{
    public static bool Narrow => false;
    public static bool Thirds => true;
    public static bool Octo => false;
    public static bool Unaligned => false;
}

struct OctoShape : IShape
{
    public static bool Narrow => false;
    public static bool Thirds => false;
    public static bool Octo => true;
    public static bool Unaligned => false;
}

struct UnalignedShape : IShape
{
    public static bool Narrow => false;
    public static bool Thirds => false;
    public static bool Octo => false;
    public static bool Unaligned => true;
}

struct CombinedShape : IShape
{
    public static bool Narrow => false;
    public static bool Thirds => true;
    public static bool Octo => true;
    public static bool Unaligned => false;
}

public unsafe class CopyShapes
{
    const int Calls = 4096;
    const int Table = 65536;
    const int Spread = 4096;
    const int Largest = 4096;

    byte* source;
    byte* target;
    int[] sizes = [];
    int[] from = [];
    int[] to = [];
    int cursor;
    delegate*<byte*, byte*, nuint, void> copy;

    [Params("33", "48", "64", "65", "80", "96", "128", "129", "160", "192", "256", "257", "320", "384", "512", "768", "1024", "2048", "4096", "1-256", "65-512", "1-2048")]
    public string Size { get; set; } = "33";

    [ParamsSource(nameof(Methods))]
    public string Method { get; set; } = "MemoryCopy";

    public static IEnumerable<string> Methods()
    {
        yield return "MemoryCopy";
        yield return "Vectors";
        yield return "Base";
        if (Vector512.IsHardwareAccelerated) yield return "Narrow";
        yield return "Thirds";
        yield return "Octo";
        yield return "Unaligned";
        yield return "Combined";
    }

    [GlobalSetup]
    public void Setup()
    {
        source = (byte*)NativeMemory.AlignedAlloc(Spread + Largest, 64);
        target = (byte*)NativeMemory.AlignedAlloc(Spread + Largest, 64);
        var random = new Random(1);
        random.NextBytes(new Span<byte>(source, Spread + Largest));
        var range = Size.Split('-');
        var low = int.Parse(range[0]);
        var high = int.Parse(range[^1]);
        sizes = new int[Table];
        from = new int[Table];
        to = new int[Table];
        for (var i = 0; i < Table; i++)
        {
            sizes[i] = random.Next(low, high + 1);
            from[i] = random.Next(Spread);
            to[i] = random.Next(Spread);
        }
        copy = Method switch
        {
            "MemoryCopy" => &ViaMemoryCopy,
            "Vectors" => &ViaVectors,
            "Base" => &Shaped<BaseShape>,
            "Narrow" => &Shaped<NarrowShape>,
            "Thirds" => &Shaped<ThirdsShape>,
            "Octo" => &Shaped<OctoShape>,
            "Unaligned" => &Shaped<UnalignedShape>,
            _ => &Shaped<CombinedShape>
        };
        var expected = new byte[Spread + Largest];
        for (var length = 0; length <= Largest; length += length < 600 ? 1 : 61)
        {
            new Span<byte>(target, Spread + Largest).Clear();
            expected.AsSpan().Clear();
            var at = length % 67;
            fixed (byte* e = expected) ViaMemoryCopy(source + length % 61, e + at, (nuint)length);
            copy(source + length % 61, target + at, (nuint)length);
            if (!new Span<byte>(target, Spread + Largest).SequenceEqual(expected)) throw new InvalidOperationException($"{Method} copied {length} bytes wrongly");
        }
    }

    [GlobalCleanup]
    public void Cleanup()
    {
        NativeMemory.AlignedFree(source);
        NativeMemory.AlignedFree(target);
    }

    [Benchmark(OperationsPerInvoke = Calls)]
    public void Run()
    {
        var at = cursor;
        for (var i = 0; i < Calls; i++, at = (at + 1) & (Table - 1)) copy(source + from[at], target + to[at], (nuint)sizes[at]);
        cursor = at;
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    static void ViaMemoryCopy(byte* from, byte* to, nuint length) => Buffer.MemoryCopy(from, to, length, length);

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    static void ViaVectors(byte* from, byte* to, nuint length) => MemoryCopy.CopyUnsafe((nint)from, (nint)to, length);

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    static void Shaped<TShape>(byte* from, byte* to, nuint length) where TShape : struct, IShape => Move<TShape>(ref *to, ref *from, length);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static void Move<TShape>(ref byte to, ref byte from, nuint length) where TShape : struct, IShape
    {
        if (length <= 16)
        {
            Short(ref to, ref from, length);
            return;
        }
        if (length <= 32)
        {
            Pair<Vector128<byte>>(ref to, ref from, length);
            return;
        }
        var wide = Vector512.IsHardwareAccelerated && !TShape.Narrow;
        if (wide)
        {
            if (length <= 64) Pair<Vector256<byte>>(ref to, ref from, length);
            else if (length <= 128) Pair<Vector512<byte>>(ref to, ref from, length);
            else if (TShape.Thirds && length <= 192) Three<Vector512<byte>>(ref to, ref from, length);
            else if (length <= 256) Quad<Vector512<byte>>(ref to, ref from, length);
            else if (TShape.Octo && length <= 512) Eight<Vector512<byte>>(ref to, ref from, length);
            else if (Overlap(ref to, ref from, length)) Overlapping(ref to, ref from, length);
            else Loop<TShape, Vector512<byte>>(ref to, ref from, length);
        }
        else if (Vector256.IsHardwareAccelerated)
        {
            if (length <= 64) Pair<Vector256<byte>>(ref to, ref from, length);
            else if (TShape.Thirds && length <= 96) Three<Vector256<byte>>(ref to, ref from, length);
            else if (length <= 128) Quad<Vector256<byte>>(ref to, ref from, length);
            else if (TShape.Octo && length <= 256) Eight<Vector256<byte>>(ref to, ref from, length);
            else if (Overlap(ref to, ref from, length)) Overlapping(ref to, ref from, length);
            else Loop<TShape, Vector256<byte>>(ref to, ref from, length);
        }
        else
        {
            if (TShape.Thirds && length <= 48) Three<Vector128<byte>>(ref to, ref from, length);
            else if (length <= 64) Quad<Vector128<byte>>(ref to, ref from, length);
            else if (TShape.Octo && length <= 128) Eight<Vector128<byte>>(ref to, ref from, length);
            else if (Overlap(ref to, ref from, length)) Overlapping(ref to, ref from, length);
            else Loop<TShape, Vector128<byte>>(ref to, ref from, length);
        }
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
    static void Pair<TVector>(ref byte to, ref byte from, nuint length) where TVector : unmanaged
    {
        var width = (nuint)Unsafe.SizeOf<TVector>();
        var a = Unsafe.ReadUnaligned<TVector>(ref from);
        var b = Unsafe.ReadUnaligned<TVector>(ref Unsafe.Add(ref from, length - width));
        Unsafe.WriteUnaligned(ref to, a);
        Unsafe.WriteUnaligned(ref Unsafe.Add(ref to, length - width), b);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static void Three<TVector>(ref byte to, ref byte from, nuint length) where TVector : unmanaged
    {
        var width = (nuint)Unsafe.SizeOf<TVector>();
        var a = Unsafe.ReadUnaligned<TVector>(ref from);
        var b = Unsafe.ReadUnaligned<TVector>(ref Unsafe.Add(ref from, width));
        var c = Unsafe.ReadUnaligned<TVector>(ref Unsafe.Add(ref from, length - width));
        Unsafe.WriteUnaligned(ref to, a);
        Unsafe.WriteUnaligned(ref Unsafe.Add(ref to, width), b);
        Unsafe.WriteUnaligned(ref Unsafe.Add(ref to, length - width), c);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static void Quad<TVector>(ref byte to, ref byte from, nuint length) where TVector : unmanaged
    {
        var width = (nuint)Unsafe.SizeOf<TVector>();
        var a = Unsafe.ReadUnaligned<TVector>(ref from);
        var b = Unsafe.ReadUnaligned<TVector>(ref Unsafe.Add(ref from, width));
        var c = Unsafe.ReadUnaligned<TVector>(ref Unsafe.Add(ref from, length - 2 * width));
        var d = Unsafe.ReadUnaligned<TVector>(ref Unsafe.Add(ref from, length - width));
        Unsafe.WriteUnaligned(ref to, a);
        Unsafe.WriteUnaligned(ref Unsafe.Add(ref to, width), b);
        Unsafe.WriteUnaligned(ref Unsafe.Add(ref to, length - 2 * width), c);
        Unsafe.WriteUnaligned(ref Unsafe.Add(ref to, length - width), d);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static void Eight<TVector>(ref byte to, ref byte from, nuint length) where TVector : unmanaged
    {
        var width = (nuint)Unsafe.SizeOf<TVector>();
        var a = Unsafe.ReadUnaligned<TVector>(ref from);
        var b = Unsafe.ReadUnaligned<TVector>(ref Unsafe.Add(ref from, width));
        var c = Unsafe.ReadUnaligned<TVector>(ref Unsafe.Add(ref from, 2 * width));
        var d = Unsafe.ReadUnaligned<TVector>(ref Unsafe.Add(ref from, 3 * width));
        var e = Unsafe.ReadUnaligned<TVector>(ref Unsafe.Add(ref from, length - 4 * width));
        var f = Unsafe.ReadUnaligned<TVector>(ref Unsafe.Add(ref from, length - 3 * width));
        var g = Unsafe.ReadUnaligned<TVector>(ref Unsafe.Add(ref from, length - 2 * width));
        var h = Unsafe.ReadUnaligned<TVector>(ref Unsafe.Add(ref from, length - width));
        Unsafe.WriteUnaligned(ref to, a);
        Unsafe.WriteUnaligned(ref Unsafe.Add(ref to, width), b);
        Unsafe.WriteUnaligned(ref Unsafe.Add(ref to, 2 * width), c);
        Unsafe.WriteUnaligned(ref Unsafe.Add(ref to, 3 * width), d);
        Unsafe.WriteUnaligned(ref Unsafe.Add(ref to, length - 4 * width), e);
        Unsafe.WriteUnaligned(ref Unsafe.Add(ref to, length - 3 * width), f);
        Unsafe.WriteUnaligned(ref Unsafe.Add(ref to, length - 2 * width), g);
        Unsafe.WriteUnaligned(ref Unsafe.Add(ref to, length - width), h);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static bool Overlap(ref byte to, ref byte from, nuint length) =>
        (nuint)Unsafe.ByteOffset(ref from, ref to) < length || (nuint)Unsafe.ByteOffset(ref to, ref from) < length;

    static void Overlapping(ref byte to, ref byte from, nuint length)
    {
        fixed (byte* target = &to, source = &from) Buffer.MemoryCopy(source, target, length, length);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static void Loop<TShape, TVector>(ref byte to, ref byte from, nuint length) where TShape : struct, IShape where TVector : unmanaged
    {
        var width = (nuint)Unsafe.SizeOf<TVector>();
        Unsafe.WriteUnaligned(ref to, Unsafe.ReadUnaligned<TVector>(ref from));
        var at = TShape.Unaligned ? width : width - ((nuint)Unsafe.AsPointer(ref to) & (width - 1));
        for (; at + 4 * width <= length; at += 4 * width)
        {
            var a = Unsafe.ReadUnaligned<TVector>(ref Unsafe.Add(ref from, at));
            var b = Unsafe.ReadUnaligned<TVector>(ref Unsafe.Add(ref from, at + width));
            var c = Unsafe.ReadUnaligned<TVector>(ref Unsafe.Add(ref from, at + 2 * width));
            var d = Unsafe.ReadUnaligned<TVector>(ref Unsafe.Add(ref from, at + 3 * width));
            Unsafe.WriteUnaligned(ref Unsafe.Add(ref to, at), a);
            Unsafe.WriteUnaligned(ref Unsafe.Add(ref to, at + width), b);
            Unsafe.WriteUnaligned(ref Unsafe.Add(ref to, at + 2 * width), c);
            Unsafe.WriteUnaligned(ref Unsafe.Add(ref to, at + 3 * width), d);
        }
        var e = Unsafe.ReadUnaligned<TVector>(ref Unsafe.Add(ref from, length - 4 * width));
        var f = Unsafe.ReadUnaligned<TVector>(ref Unsafe.Add(ref from, length - 3 * width));
        var g = Unsafe.ReadUnaligned<TVector>(ref Unsafe.Add(ref from, length - 2 * width));
        var h = Unsafe.ReadUnaligned<TVector>(ref Unsafe.Add(ref from, length - width));
        Unsafe.WriteUnaligned(ref Unsafe.Add(ref to, length - 4 * width), e);
        Unsafe.WriteUnaligned(ref Unsafe.Add(ref to, length - 3 * width), f);
        Unsafe.WriteUnaligned(ref Unsafe.Add(ref to, length - 2 * width), g);
        Unsafe.WriteUnaligned(ref Unsafe.Add(ref to, length - width), h);
    }
}
#endif
