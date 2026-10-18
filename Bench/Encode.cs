using System.Runtime.InteropServices;
using BenchmarkDotNet.Attributes;
using GFXKit.Media.Codecs;
#if NET9_0_OR_GREATER
using Ptr = GFXKit.Benchmarks.Pointers;
#endif

namespace Bench;

public unsafe class Encode
{
    const int Frames = 48000;
    const int Packet = 1024;
    const int Stride = 1088;
    const int Layouts = 8;
    const int LayoutStep = 128;

    byte* bytes;
    float* samples;
    int length;
    int count;
    delegate*<ReadOnlySpan<float>, int, int, Span<byte>, int, void> kernel;
#if NET9_0_OR_GREATER
    delegate*<ReadOnlySpan<float>, int, int, Span<byte>, int, void> registers;
    delegate*<ReadOnlySpan<float>, int, int, Span<byte>, int, void> structures;
    delegate*<ReadOnlySpan<float>, int, int, Span<byte>, int, void> pairs;
    delegate*<float*, nint, int, byte*, int, void> pointers;
#endif

    [Params("S16", "S24", "S32", "F32")]
    public string Format { get; set; } = "S16";

    [Params(1, 2, 3, 4, 6, 8)]
    public int Channels { get; set; }

    Span<byte> Data => new(bytes, length);

    Span<float> Planes => new(samples, count);

    int Bytes => Format switch { "S16" => 2, "S24" => 3, _ => 4 };

    [GlobalSetup]
    public void Setup()
    {
        length = Frames * Channels * Bytes;
        bytes = (byte*)NativeMemory.AlignedAlloc((nuint)length, 4096);
        var data = Data;
        data.Clear();
        count = Channels * Stride + Layouts * LayoutStep;
        samples = (float*)NativeMemory.AlignedAlloc((nuint)count * 4, 4096);
        var planes = Planes;
        planes.Clear();
        for (var i = 0; i < planes.Length; i++) planes[i] = (i * 7919 % 4001 - 2000) / 1900f;
        kernel = Format switch
        {
            "S16" => Interleave.For<S16>(Channels),
            "S24" => Interleave.For<S24>(Channels),
            "S32" => Interleave.For<S32>(Channels),
            _ => Interleave.For<F32>(Channels)
        };
#if NET9_0_OR_GREATER
        pointers = Format switch
        {
            "S16" => Ptr.Interleave.For<Ptr.S16>(Channels),
            "S24" => Ptr.Interleave.For<Ptr.S24>(Channels),
            "S32" => Ptr.Interleave.For<Ptr.S32>(Channels),
            _ => Ptr.Interleave.For<Ptr.F32>(Channels)
        };
        registers = Variant("registers");
        structures = Variant("structures");
        pairs = Variant("pairs");
        Library();
        var expected = Data.ToArray();
        Data.Clear();
        Pointers();
        if (!Data.SequenceEqual(expected)) throw new InvalidOperationException($"{Format} {Channels}: the pointer kernel's output differs");
        Check(registers, expected, "register");
        Check(structures, expected, "structure");
        Check(pairs, expected, "paired");
#endif
    }

#if NET9_0_OR_GREATER
    delegate*<ReadOnlySpan<float>, int, int, Span<byte>, int, void> Variant(string kind)
    {
        if (!System.Runtime.Intrinsics.Arm.AdvSimd.Arm64.IsSupported) return kernel;
        var table = GFXKit.Benchmarks.NeonVariants.Kernels;
        for (var i = 0; i < table.Length; i++)
            if (table[i].Direction == "encode" && table[i].Format == Format && table[i].Channels == Channels && table[i].Kind == kind) return (delegate*<ReadOnlySpan<float>, int, int, Span<byte>, int, void>)table[i].Kernel;
        return kernel;
    }

    void Check(delegate*<ReadOnlySpan<float>, int, int, Span<byte>, int, void> variant, Span<byte> expected, string kind)
    {
        Data.Clear();
        Run(variant);
        if (!Data.SequenceEqual(expected)) throw new InvalidOperationException($"{Format} {Channels}: the {kind} variant's output differs");
    }

    [Benchmark]
    public void Registers() => Run(registers);

    [Benchmark]
    public void Structures() => Run(structures);

    [Benchmark]
    public void Pairs() => Run(pairs);
#endif

    [GlobalCleanup]
    public void Cleanup()
    {
        NativeMemory.AlignedFree(bytes);
        NativeMemory.AlignedFree(samples);
    }

    [Benchmark(Baseline = true)]
    public void Library() => Run(kernel);

    void Run(delegate*<ReadOnlySpan<float>, int, int, Span<byte>, int, void> kernel)
    {
        var frameBytes = Channels * Bytes;
        for (var f = 0; f < Frames; f += Packet)
        {
            var frames = Math.Min(Packet, Frames - f);
            kernel(Planes[(f / Packet % Layouts * LayoutStep)..], Stride, Channels, Data.Slice(f * frameBytes, frames * frameBytes), frames);
        }
    }

#if NET9_0_OR_GREATER
    [Benchmark]
    public void Pointers()
    {
        var frameBytes = Channels * Bytes;
        for (var f = 0; f < Frames; f += Packet) pointers(samples + f / Packet % Layouts * LayoutStep, Stride, Channels, bytes + f * frameBytes, Math.Min(Packet, Frames - f));
    }
#endif
}
