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

    byte[] data = [];
    float[] planes = [];
    delegate*<ReadOnlySpan<float>, int, int, Span<byte>, int, void> kernel;
#if NET9_0_OR_GREATER
    delegate*<ReadOnlySpan<float>, int, int, Span<byte>, int, void> registers;
    delegate*<ReadOnlySpan<float>, int, int, Span<byte>, int, void> structures;
    delegate*<float*, nint, int, byte*, int, void> pointers;
#endif

    [Params("S16", "S24", "S32", "F32")]
    public string Format { get; set; } = "S16";

    [Params(1, 2, 3, 4, 6, 8)]
    public int Channels { get; set; }

    int Bytes => Format switch { "S16" => 2, "S24" => 3, _ => 4 };

    [GlobalSetup]
    public void Setup()
    {
        data = new byte[Frames * Channels * Bytes];
        planes = new float[Channels * Stride];
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
        Library();
        var expected = data.ToArray();
        Array.Clear(data);
        Pointers();
        if (!data.AsSpan().SequenceEqual(expected)) throw new InvalidOperationException($"{Format} {Channels}: the pointer kernel's output differs");
        Check(registers, expected, "register");
        Check(structures, expected, "structure");
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

    void Check(delegate*<ReadOnlySpan<float>, int, int, Span<byte>, int, void> variant, byte[] expected, string kind)
    {
        Array.Clear(data);
        Run(variant);
        if (!data.AsSpan().SequenceEqual(expected)) throw new InvalidOperationException($"{Format} {Channels}: the {kind} variant's output differs");
    }

    [Benchmark]
    public void Registers() => Run(registers);

    [Benchmark]
    public void Structures() => Run(structures);
#endif

    [Benchmark(Baseline = true)]
    public void Library() => Run(kernel);

    void Run(delegate*<ReadOnlySpan<float>, int, int, Span<byte>, int, void> kernel)
    {
        var frameBytes = Channels * Bytes;
        for (var f = 0; f < Frames; f += Packet)
        {
            var count = Math.Min(Packet, Frames - f);
            kernel(planes, Stride, Channels, data.AsSpan(f * frameBytes, count * frameBytes), count);
        }
    }

#if NET9_0_OR_GREATER
    [Benchmark]
    public void Pointers()
    {
        var frameBytes = Channels * Bytes;
        fixed (byte* raw = data)
        fixed (float* samples = planes)
            for (var f = 0; f < Frames; f += Packet) pointers(samples, Stride, Channels, raw + f * frameBytes, Math.Min(Packet, Frames - f));
    }
#endif
}
