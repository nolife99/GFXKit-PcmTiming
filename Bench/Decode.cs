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

    byte[] data = [];
    float[] planes = [];
    delegate*<ReadOnlySpan<byte>, int, Span<float>, int, int, void> kernel;
#if NET9_0_OR_GREATER
    delegate*<byte*, int, float*, nint, int, void> pointers;
#endif

    [Params("U8", "S16", "S24", "S32", "F32", "F64")]
    public string Format { get; set; } = "S16";

    [Params(1, 2, 3, 4, 6, 8)]
    public int Channels { get; set; }

    int Bytes => Format switch { "U8" => 1, "S16" => 2, "S24" => 3, "F64" => 8, _ => 4 };

    [GlobalSetup]
    public void Setup()
    {
        data = new byte[Frames * Channels * Bytes];
        new Random(Channels).NextBytes(data);
        if (Format == "F32") for (var i = 0; i < data.Length; i += 4) BitConverter.TryWriteBytes(data.AsSpan(i), i % 2001 / 1000f - 1);
        if (Format == "F64") for (var i = 0; i < data.Length; i += 8) BitConverter.TryWriteBytes(data.AsSpan(i), i % 2001 / 1000.0 - 1);
        planes = new float[Channels * Stride];
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
        Library();
        var expected = planes.ToArray();
        Array.Clear(planes);
        Pointers();
        if (!planes.AsSpan().SequenceEqual(expected)) throw new InvalidOperationException($"{Format} {Channels}: the pointer kernel's output differs");
#endif
    }

    [Benchmark(Baseline = true)]
    public void Library()
    {
        var frameBytes = Channels * Bytes;
        for (var f = 0; f < Frames; f += Packet)
        {
            var count = Math.Min(Packet, Frames - f);
            kernel(data.AsSpan(f * frameBytes, count * frameBytes), Channels, planes, Stride, count);
        }
    }

#if NET9_0_OR_GREATER
    [Benchmark]
    public void Pointers()
    {
        var frameBytes = Channels * Bytes;
        fixed (byte* raw = data)
        fixed (float* samples = planes)
            for (var f = 0; f < Frames; f += Packet) pointers(raw + f * frameBytes, Channels, samples, Stride, Math.Min(Packet, Frames - f));
    }
#endif
}
