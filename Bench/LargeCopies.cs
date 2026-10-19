#if NET9_0_OR_GREATER
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.Arm;
using System.Runtime.Intrinsics.X86;
using BenchmarkDotNet.Attributes;
using GFXKit;

namespace Bench;

public unsafe class LargeCopies
{
    const nuint Fresh = 512 << 20;

    byte* source;
    byte* target;
    nuint window;
    nuint at;
    delegate*<byte*, byte*, nuint, void> copy;

    [Params(64 << 10, 256 << 10, 1 << 20, 2 << 20, 4 << 20, 8 << 20, 16 << 20, 32 << 20, 64 << 20, 128 << 20)]
    public int Size { get; set; }

    [Params("Fresh", "Reused")]
    public string Destination { get; set; } = "Fresh";

    [ParamsSource(nameof(Copies))]
    public string Copy { get; set; } = "MemoryCopy";

    public static IEnumerable<string> Copies()
    {
        yield return "BufferMemoryCopy";
        yield return "MemoryCopy";
        yield return "Streaming";
        if (AdvSimd.Arm64.IsSupported) yield return "Pairs";
    }

    [GlobalSetup]
    public void Setup()
    {
        var size = (nuint)Size;
        window = Destination == "Fresh" ? Math.Max(Fresh, 4 * size) : size;
        source = (byte*)NativeMemory.AlignedAlloc(size, 4096);
        target = (byte*)NativeMemory.AlignedAlloc(window, 4096);
        new Random(1).NextBytes(new Span<byte>(source, Size));
        for (nuint i = 0; i < window; i += 4096) target[i] = 1;
        copy = Copy switch
        {
            "BufferMemoryCopy" => &ViaBufferMemoryCopy,
            "MemoryCopy" => &ViaMemoryCopy,
            "Streaming" => &Streaming,
            _ => &Pairs
        };
        copy(source, target, size);
        if (!new ReadOnlySpan<byte>(target, Size).SequenceEqual(new ReadOnlySpan<byte>(source, Size))) throw new InvalidOperationException($"{Copy} copied wrongly");
    }

    [GlobalCleanup]
    public void Cleanup()
    {
        NativeMemory.AlignedFree(source);
        NativeMemory.AlignedFree(target);
    }

    [Benchmark]
    public void Run()
    {
        var size = (nuint)Size;
        copy(source, target + at, size);
        at += size;
        if (at + size > window) at = 0;
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    static void ViaBufferMemoryCopy(byte* from, byte* to, nuint length) => Buffer.MemoryCopy(from, to, length, length);

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    static void ViaMemoryCopy(byte* from, byte* to, nuint length) => MemoryCopy.CopyUnsafe((nint)from, (nint)to, length);

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    static void Streaming(byte* from, byte* to, nuint length)
    {
        if (Vector256.IsHardwareAccelerated)
        {
            for (nuint i = 0; i < length; i += 128)
            {
                var a = Vector256.Load(from + i);
                var b = Vector256.Load(from + i + 32);
                var c = Vector256.Load(from + i + 64);
                var d = Vector256.Load(from + i + 96);
                Vector256.StoreAlignedNonTemporal(a, to + i);
                Vector256.StoreAlignedNonTemporal(b, to + i + 32);
                Vector256.StoreAlignedNonTemporal(c, to + i + 64);
                Vector256.StoreAlignedNonTemporal(d, to + i + 96);
            }
            if (Sse.IsSupported) Sse.StoreFence();
            return;
        }
        for (nuint i = 0; i < length; i += 64)
        {
            var a = Vector128.Load(from + i);
            var b = Vector128.Load(from + i + 16);
            var c = Vector128.Load(from + i + 32);
            var d = Vector128.Load(from + i + 48);
            Vector128.StoreAlignedNonTemporal(a, to + i);
            Vector128.StoreAlignedNonTemporal(b, to + i + 16);
            Vector128.StoreAlignedNonTemporal(c, to + i + 32);
            Vector128.StoreAlignedNonTemporal(d, to + i + 48);
        }
        if (Sse.IsSupported) Sse.StoreFence();
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    static void Pairs(byte* from, byte* to, nuint length)
    {
        for (nuint i = 0; i < length; i += 64)
        {
            var (a, b) = AdvSimd.Arm64.LoadPairVector128(from + i);
            var (c, d) = AdvSimd.Arm64.LoadPairVector128(from + i + 32);
            AdvSimd.Arm64.StorePairNonTemporal(to + i, a, b);
            AdvSimd.Arm64.StorePairNonTemporal(to + i + 32, c, d);
        }
    }
}
#endif
