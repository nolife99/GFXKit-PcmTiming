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
        Library();
        var expected = data.ToArray();
        Array.Clear(data);
        Pointers();
        if (!data.AsSpan().SequenceEqual(expected)) throw new InvalidOperationException($"{Format} {Channels}: the pointer kernel's output differs");
#endif
    }

    [Benchmark(Baseline = true)]
    public void Library()
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
