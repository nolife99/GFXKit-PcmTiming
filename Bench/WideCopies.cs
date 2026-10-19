#if NET9_0_OR_GREATER
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics;
using BenchmarkDotNet.Attributes;
using GFXKit;

namespace Bench;

public unsafe class WideCopies
{
    const int Calls = 1024;
    const int Table = 16384;
    const int Spread = 4096;
    const int Largest = 65536;

    byte* source;
    byte* target;
    int[] sizes = [];
    int[] from = [];
    int[] to = [];
    int cursor;
    delegate*<byte*, byte*, nuint, void> copy;

    [Params("129", "160", "192", "255", "256", "257", "320", "384", "448", "511", "512", "768", "1024", "2048", "4096", "16384", "65536", "129-512", "129-4096", "1-2048")]
    public string Size { get; set; } = "129";

    [ParamsSource(nameof(Methods))]
    public string Method { get; set; } = "MemoryCopy";

    public static IEnumerable<string> Methods()
    {
        yield return "BufferMemoryCopy";
        yield return "MemoryCopy";
        if (!Vector256.IsHardwareAccelerated) yield break;
        yield return "Narrow";
        yield return "AlignedStores";
        if (!Vector512.IsHardwareAccelerated) yield break;
        yield return "Wide";
        yield return "WideLate";
    }

    [GlobalSetup]
    public void Setup()
    {
        Console.WriteLine($"// Vector512 {Vector512.IsHardwareAccelerated}, family 0x{Family():x}");
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
            "BufferMemoryCopy" => &ViaBufferMemoryCopy,
            "MemoryCopy" => &ViaMemoryCopy,
            "Narrow" => &Shaped<NarrowShape>,
            "AlignedStores" => &Shaped<AlignedShape>,
            "Wide" => &Shaped<WideShape>,
            _ => &Shaped<WideLateShape>
        };
        var expected = new byte[Spread + Largest];
        for (var i = 0; i < 1024; i++)
        {
            new Span<byte>(target, Spread + Largest).Clear();
            expected.AsSpan().Clear();
            new ReadOnlySpan<byte>(source + from[i], sizes[i]).CopyTo(expected.AsSpan(to[i]));
            copy(source + from[i], target + to[i], (nuint)sizes[i]);
            if (!new Span<byte>(target, Spread + Largest).SequenceEqual(expected)) throw new InvalidOperationException($"{Method} copied {sizes[i]} bytes wrongly");
        }
    }

    static int Family()
    {
        if (!System.Runtime.Intrinsics.X86.X86Base.IsSupported) return 0;
        var eax = System.Runtime.Intrinsics.X86.X86Base.CpuId(1, 0).Eax;
        return (eax >> 8 & 0xF) + (eax >> 20 & 0xFF);
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
    static void ViaBufferMemoryCopy(byte* from, byte* to, nuint length) => Buffer.MemoryCopy(from, to, length, length);

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    static void ViaMemoryCopy(byte* from, byte* to, nuint length) => MemoryCopy.CopyUnsafe((nint)from, (nint)to, length);

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    static void Shaped<TShape>(byte* from, byte* to, nuint length) where TShape : struct, IWideShape
    {
        if (length <= 128)
        {
            MemoryCopy.CopyUnsafe((nint)from, (nint)to, length);
            return;
        }
        if (TShape.Wide && (!TShape.Late || length <= 256 || length >= 512))
        {
            if (length <= 192) Three<Vector512<byte>>(to, from, length);
            else if (length <= 256) Four<Vector512<byte>>(to, from, length);
            else Forward<Vector512<byte>>(to, from, length, false);
        }
        else Forward<Vector256<byte>>(to, from, length, TShape.Aligned);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static void Three<T>(byte* to, byte* from, nuint length) where T : unmanaged
    {
        var a = Unsafe.ReadUnaligned<T>(from);
        var b = Unsafe.ReadUnaligned<T>(from + sizeof(T));
        var c = Unsafe.ReadUnaligned<T>(from + length - (nuint)sizeof(T));
        Unsafe.WriteUnaligned(to, a);
        Unsafe.WriteUnaligned(to + sizeof(T), b);
        Unsafe.WriteUnaligned(to + length - (nuint)sizeof(T), c);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
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

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static void Forward<T>(byte* to, byte* from, nuint length, bool aligned) where T : unmanaged
    {
        var last = from + length - 4 * (nuint)sizeof(T);
        var end = to + length - 4 * (nuint)sizeof(T);
        var head = Unsafe.ReadUnaligned<T>(from);
        var e = Unsafe.ReadUnaligned<T>(last);
        var f = Unsafe.ReadUnaligned<T>(last + sizeof(T));
        var g = Unsafe.ReadUnaligned<T>(last + 2 * sizeof(T));
        var h = Unsafe.ReadUnaligned<T>(last + 3 * sizeof(T));
        var skip = (nuint)sizeof(T) - ((nuint)to & (nuint)(sizeof(T) - 1));
        var source = from + skip;
        for (var target = to + skip; target <= end; source += 4 * sizeof(T), target += 4 * sizeof(T))
        {
            var a = Unsafe.ReadUnaligned<T>(source);
            var b = Unsafe.ReadUnaligned<T>(source + sizeof(T));
            var c = Unsafe.ReadUnaligned<T>(source + 2 * sizeof(T));
            var d = Unsafe.ReadUnaligned<T>(source + 3 * sizeof(T));
            if (aligned && sizeof(T) == 32)
            {
                Vector256.StoreAligned(Unsafe.As<T, Vector256<byte>>(ref a), target);
                Vector256.StoreAligned(Unsafe.As<T, Vector256<byte>>(ref b), target + 32);
                Vector256.StoreAligned(Unsafe.As<T, Vector256<byte>>(ref c), target + 64);
                Vector256.StoreAligned(Unsafe.As<T, Vector256<byte>>(ref d), target + 96);
            }
            else
            {
                Unsafe.WriteUnaligned(target, a);
                Unsafe.WriteUnaligned(target + sizeof(T), b);
                Unsafe.WriteUnaligned(target + 2 * sizeof(T), c);
                Unsafe.WriteUnaligned(target + 3 * sizeof(T), d);
            }
        }
        Unsafe.WriteUnaligned(to, head);
        Unsafe.WriteUnaligned(end, e);
        Unsafe.WriteUnaligned(end + sizeof(T), f);
        Unsafe.WriteUnaligned(end + 2 * sizeof(T), g);
        Unsafe.WriteUnaligned(end + 3 * sizeof(T), h);
    }
}

interface IWideShape
{
    static abstract bool Wide { get; }
    static abstract bool Late { get; }
    static abstract bool Aligned { get; }
}

struct NarrowShape : IWideShape
{
    public static bool Wide => false;
    public static bool Late => false;
    public static bool Aligned => false;
}

struct AlignedShape : IWideShape
{
    public static bool Wide => false;
    public static bool Late => false;
    public static bool Aligned => true;
}

struct WideShape : IWideShape
{
    public static bool Wide => true;
    public static bool Late => false;
    public static bool Aligned => false;
}

struct WideLateShape : IWideShape
{
    public static bool Wide => true;
    public static bool Late => true;
    public static bool Aligned => false;
}
#endif
