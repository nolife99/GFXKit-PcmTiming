#if NET9_0_OR_GREATER
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics;
using BenchmarkDotNet.Attributes;
using GFXKit;

namespace Bench;

public unsafe class CopyScenarios
{
    const nuint Fresh = 512 << 20;
    const nuint Hot = 512 << 10;

    byte* source;
    byte* target;
    byte* hot;
    nuint window;
    nuint at;
    ulong sum;

    [Params(8 << 10, 16 << 10, 32 << 10, 64 << 10, 256 << 10, 1 << 20, 4 << 20, 16 << 20, 64 << 20)]
    public int Size { get; set; }

    [Params("Fresh", "Reused", "Consumed", "WorkingSet")]
    public string Scenario { get; set; } = "Fresh";

    [Params(CopyHint.Auto, CopyHint.Cached, CopyHint.Streaming)]
    public CopyHint Hint { get; set; }

    public ulong Sum => sum;

    [GlobalSetup]
    public void Setup()
    {
        var size = (nuint)Size;
        window = Scenario is "Fresh" or "WorkingSet" ? Math.Max(Fresh, 4 * size) : size;
        source = (byte*)NativeMemory.AlignedAlloc(size, 4096);
        target = (byte*)NativeMemory.AlignedAlloc(window, 4096);
        hot = (byte*)NativeMemory.AlignedAlloc(Hot, 4096);
        new Random(1).NextBytes(new Span<byte>(source, Size));
        new Random(2).NextBytes(new Span<byte>(hot, (int)Hot));
        for (nuint i = 0; i < window; i += 4096) target[i] = 1;
        Console.WriteLine($"// streaming from {MemoryCopy.StreamingFrom} bytes, or {MemoryCopy.StreamingFloor} when asked");
    }

    [GlobalCleanup]
    public void Cleanup()
    {
        NativeMemory.AlignedFree(source);
        NativeMemory.AlignedFree(target);
        NativeMemory.AlignedFree(hot);
    }

    [Benchmark]
    public void Run()
    {
        var size = (nuint)Size;
        MemoryCopy.CopyUnsafe((nint)source, (nint)(target + at), size, Hint);
        if (Scenario == "Consumed") sum += Total(target + at, size);
        else if (Scenario == "WorkingSet") sum += Total(hot, Hot);
        at += size;
        if (at + size > window) at = 0;
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    static ulong Total(byte* bytes, nuint length)
    {
        var total = Vector128<ulong>.Zero;
        for (nuint i = 0; i < length; i += 64)
        {
            var a = Vector128.Load((ulong*)(bytes + i)) + Vector128.Load((ulong*)(bytes + i + 16));
            var b = Vector128.Load((ulong*)(bytes + i + 32)) + Vector128.Load((ulong*)(bytes + i + 48));
            total += a + b;
        }
        return Vector128.Sum(total);
    }
}
#endif
