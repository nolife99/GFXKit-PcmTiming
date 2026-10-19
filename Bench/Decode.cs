using System.Runtime.InteropServices;
using BenchmarkDotNet.Attributes;
using GFXKit.Media.Codecs;
#if NET9_0_OR_GREATER
using Ptr = GFXKit.Benchmarks.Pointers;
#endif

namespace Bench;

public unsafe class Decode
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
    delegate*<ReadOnlySpan<byte>, int, Span<float>, int, int, void> kernel;
#if NET9_0_OR_GREATER
    delegate*<ReadOnlySpan<byte>, int, Span<float>, int, int, void> registers;
    delegate*<ReadOnlySpan<byte>, int, Span<float>, int, int, void> structures;
    delegate*<ReadOnlySpan<byte>, int, Span<float>, int, int, void> pairs;
    delegate*<ReadOnlySpan<byte>, int, Span<float>, int, int, void> subsets;
    delegate*<ReadOnlySpan<byte>, int, Span<float>, int, int, void> scalar;
    delegate*<byte*, int, float*, nint, int, void> pointers;
#endif

    [Params("U8", "S16", "S24", "S32", "F32", "F64")]
    public string Format { get; set; } = "S16";

    [Params(1, 2, 3, 4, 5, 6, 7, 8)]
    public int Channels { get; set; }

    Span<byte> Data => new(bytes, length);

    Span<float> Planes => new(samples, count);

    int Bytes => Format switch { "U8" => 1, "S16" => 2, "S24" => 3, "F64" => 8, _ => 4 };

    [GlobalSetup]
    public void Setup()
    {
        length = Frames * Channels * Bytes;
        bytes = (byte*)NativeMemory.AlignedAlloc((nuint)length, 4096);
        var data = Data;
        data.Clear();
        new Random(Channels).NextBytes(data);
        if (Format == "F32") for (var i = 0; i < data.Length; i += 4) BitConverter.TryWriteBytes(data[i..], i % 2001 / 1000f - 1);
        if (Format == "F64") for (var i = 0; i < data.Length; i += 8) BitConverter.TryWriteBytes(data[i..], i % 2001 / 1000.0 - 1);
        count = Channels * Stride + Layouts * LayoutStep;
        samples = (float*)NativeMemory.AlignedAlloc((nuint)count * 4, 4096);
        var planes = Planes;
        planes.Clear();
        kernel = Format switch
        {
            "U8" => Deinterleave.For<U8>(Channels),
            "S16" => Deinterleave.For<S16>(Channels),
            "S24" => Deinterleave.For<S24>(Channels),
            "S32" => Deinterleave.For<S32>(Channels),
            "F32" => Deinterleave.For<F32>(Channels),
            _ => Deinterleave.For<F64>(Channels)
        };
#if NET9_0_OR_GREATER
        pointers = Format switch
        {
            "U8" => Ptr.Deinterleave.For<Ptr.U8>(Channels),
            "S16" => Ptr.Deinterleave.For<Ptr.S16>(Channels),
            "S24" => Ptr.Deinterleave.For<Ptr.S24>(Channels),
            "S32" => Ptr.Deinterleave.For<Ptr.S32>(Channels),
            "F32" => Ptr.Deinterleave.For<Ptr.F32>(Channels),
            _ => Ptr.Deinterleave.For<Ptr.F64>(Channels)
        };
        registers = Variant("registers");
        structures = Variant("structures");
        pairs = Variant("pairs");
        subsets = Variant("subsets");
        scalar = Format switch
        {
            "U8" => &Deinterleave.Scalar<U8>,
            "S16" => &Deinterleave.Scalar<S16>,
            "S24" => &Deinterleave.Scalar<S24>,
            "S32" => &Deinterleave.Scalar<S32>,
            "F32" => &Deinterleave.Scalar<F32>,
            _ => &Deinterleave.Scalar<F64>
        };
        Library();
        var expected = Planes.ToArray();
        Planes.Clear();
        Pointers();
        if (!Planes.SequenceEqual(expected)) throw new InvalidOperationException($"{Format} {Channels}: the pointer kernel's output differs");
        Check(registers, expected, "register");
        Check(structures, expected, "structure");
        Check(pairs, expected, "paired");
        Check(subsets, expected, "subset");
#endif
    }

#if NET9_0_OR_GREATER
    delegate*<ReadOnlySpan<byte>, int, Span<float>, int, int, void> Variant(string kind)
    {
        if (!System.Runtime.Intrinsics.Arm.AdvSimd.Arm64.IsSupported) return kernel;
        var table = GFXKit.Benchmarks.NeonVariants.Kernels;
        for (var i = 0; i < table.Length; i++)
            if (table[i].Direction == "decode" && table[i].Format == Format && table[i].Channels == Channels && table[i].Kind == kind) return (delegate*<ReadOnlySpan<byte>, int, Span<float>, int, int, void>)table[i].Kernel;
        return kernel;
    }

    void Check(delegate*<ReadOnlySpan<byte>, int, Span<float>, int, int, void> variant, Span<float> expected, string kind)
    {
        Planes.Clear();
        Run(variant);
        if (!Planes.SequenceEqual(expected)) throw new InvalidOperationException($"{Format} {Channels}: the {kind} variant's output differs");
    }

    [Benchmark]
    public void Registers() => Run(registers);

    [Benchmark]
    public void Structures() => Run(structures);

    [Benchmark]
    public void Pairs() => Run(pairs);

    [Benchmark]
    public void Subsets() => Run(subsets);

    [Benchmark]
    public void Scalar() => Run(scalar);
#endif

    [GlobalCleanup]
    public void Cleanup()
    {
        NativeMemory.AlignedFree(bytes);
        NativeMemory.AlignedFree(samples);
    }

    [Benchmark(Baseline = true)]
    public void Library() => Run(kernel);

    void Run(delegate*<ReadOnlySpan<byte>, int, Span<float>, int, int, void> kernel)
    {
        var frameBytes = Channels * Bytes;
        for (var f = 0; f < Frames; f += Packet)
        {
            var frames = Math.Min(Packet, Frames - f);
            kernel(Data.Slice(f * frameBytes, frames * frameBytes), Channels, Planes[(f / Packet % Layouts * LayoutStep)..], Stride, frames);
        }
    }

#if NET9_0_OR_GREATER
    [Benchmark]
    public void Pointers()
    {
        var frameBytes = Channels * Bytes;
        for (var f = 0; f < Frames; f += Packet) pointers(bytes + f * frameBytes, Channels, samples + f / Packet % Layouts * LayoutStep, Stride, Math.Min(Packet, Frames - f));
    }
#endif
}
