#if NET9_0_OR_GREATER
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using BenchmarkDotNet.Attributes;
using GFXKit;

namespace Bench;

public unsafe class SmallCopies
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

    [Params("1", "2", "3", "4", "7", "8", "12", "16", "17", "24", "32", "33", "48", "64", "65", "96", "128", "129", "192", "256", "257", "384", "512", "1024", "2048", "2049", "4096", "1-16", "1-64", "1-256", "1-2048")]
    public string Size { get; set; } = "1";

    [Params("MemoryCopy", "CopyTo", "Vectors")]
    public string Method { get; set; } = "MemoryCopy";

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
            "CopyTo" => &ViaCopyTo,
            _ => &ViaVectors
        };
        var expected = new byte[Spread + Largest];
        for (var i = 0; i < 4096; i++)
        {
            new Span<byte>(target, Spread + Largest).Clear();
            expected.AsSpan().Clear();
            fixed (byte* e = expected) ViaCopyTo(source + from[i], e + to[i], (nuint)sizes[i]);
            copy(source + from[i], target + to[i], (nuint)sizes[i]);
            if (!new Span<byte>(target, Spread + Largest).SequenceEqual(expected)) throw new InvalidOperationException($"{Method} copied {sizes[i]} bytes wrongly");
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
    static void ViaCopyTo(byte* from, byte* to, nuint length) => new ReadOnlySpan<byte>(from, (int)length).CopyTo(new Span<byte>(to, (int)length));

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    static void ViaVectors(byte* from, byte* to, nuint length) => MemoryCopy.CopyUnsafe((nint)from, (nint)to, length);
}
#endif
