#if NET9_0_OR_GREATER
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics.X86;
using BenchmarkDotNet.Attributes;
using GFXKit.Benchmarks;
using GFXKit.Media.Codecs;

namespace Bench;

static class X86Cases
{
    public static IEnumerable<string> Of((string Format, int Channels, string Kind, bool Vbmi, nint Kernel)[] table)
    {
        if (!Avx2.IsSupported) yield break;
        var avx512 = Avx512Deinterleave.Supported;
        var only = Environment.GetEnvironmentVariable("BENCH_LAYOUT");
        var previous = "";
        for (var i = 0; i < table.Length; i++)
        {
            var layout = $"{table[i].Format} {table[i].Channels}";
            if (!string.IsNullOrEmpty(only) && layout != only) continue;
            if (layout != previous)
            {
                yield return $"{layout} avx2";
                yield return $"{layout} sse";
                previous = layout;
            }
            if (avx512 && (!table[i].Vbmi || Avx512Vbmi.IsSupported)) yield return $"{table[i].Format} {table[i].Channels} {table[i].Kind}";
        }
    }

    public static nint Variant((string Format, int Channels, string Kind, bool Vbmi, nint Kernel)[] table, string format, int channels, string kind)
    {
        for (var i = 0; i < table.Length; i++)
            if (table[i].Format == format && table[i].Channels == channels && table[i].Kind == kind) return table[i].Kernel;
        throw new ArgumentException($"no {kind} variant for {format} {channels}");
    }
}

public unsafe class X86Decode
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
    int channels;
    int width;
    delegate*<ReadOnlySpan<byte>, int, Span<float>, int, int, void> kernel;

    public static IEnumerable<string> Cases() => X86Cases.Of(Avx512Variants.Decoders);

    [ParamsSource(nameof(Cases))]
    public string Case { get; set; } = "";

    Span<byte> Data => new(bytes, length);

    Span<float> Planes => new(samples, count);

    [GlobalSetup]
    public void Setup()
    {
        var parts = Case.Split(' ');
        var format = parts[0];
        channels = int.Parse(parts[1]);
        width = format switch { "U8" => 1, "S16" => 2, "S24" => 3, "F64" => 8, _ => 4 };
        length = Frames * channels * width;
        bytes = (byte*)NativeMemory.AlignedAlloc((nuint)length, 4096);
        var data = Data;
        data.Clear();
        new Random(channels).NextBytes(data);
        if (format == "F32") for (var i = 0; i < data.Length; i += 4) BitConverter.TryWriteBytes(data[i..], i % 2001 / 1000f - 1);
        if (format == "F64") for (var i = 0; i < data.Length; i += 8) BitConverter.TryWriteBytes(data[i..], i % 2001 / 1000.0 - 1);
        count = channels * Stride + Layouts * LayoutStep;
        samples = (float*)NativeMemory.AlignedAlloc((nuint)count * 4, 4096);
        kernel = format switch
        {
            "U8" => &Deinterleave.Scalar<U8>,
            "S16" => &Deinterleave.Scalar<S16>,
            "S24" => &Deinterleave.Scalar<S24>,
            "S32" => &Deinterleave.Scalar<S32>,
            "F32" => &Deinterleave.Scalar<F32>,
            _ => &Deinterleave.Scalar<F64>
        };
        Planes.Clear();
        Run();
        var expected = Planes.ToArray();
        kernel = parts[2] switch
        {
            "avx2" => format switch
            {
                "U8" => Deinterleave.Avx2Kernel<U8>(channels),
                "S16" => Deinterleave.Avx2Kernel<S16>(channels),
                "S24" => Deinterleave.Avx2Kernel<S24>(channels),
                "S32" => Deinterleave.Avx2Kernel<S32>(channels),
                "F32" => Deinterleave.Avx2Kernel<F32>(channels),
                _ => Deinterleave.Avx2Kernel<F64>(channels)
            },
            "sse" => format switch
            {
                "U8" => SseDeinterleave.For<U8>(channels),
                "S16" => SseDeinterleave.For<S16>(channels),
                "S24" => SseDeinterleave.For<S24>(channels),
                "S32" => SseDeinterleave.For<S32>(channels),
                "F32" => SseDeinterleave.For<F32>(channels),
                _ => SseDeinterleave.For<F64>(channels)
            },
            _ => (delegate*<ReadOnlySpan<byte>, int, Span<float>, int, int, void>)X86Cases.Variant(Avx512Variants.Decoders, format, channels, parts[2])
        };
        Planes.Clear();
        Run();
        if (!Planes.SequenceEqual(expected)) throw new InvalidOperationException($"decode {Case}: the output differs from the scalar loop's");
    }

    [GlobalCleanup]
    public void Cleanup()
    {
        NativeMemory.AlignedFree(bytes);
        NativeMemory.AlignedFree(samples);
    }

    [Benchmark]
    public void Run()
    {
        var frameBytes = channels * width;
        for (var f = 0; f < Frames; f += Packet)
        {
            var frames = Math.Min(Packet, Frames - f);
            kernel(Data.Slice(f * frameBytes, frames * frameBytes), channels, Planes[(f / Packet % Layouts * LayoutStep)..], Stride, frames);
        }
    }
}

public unsafe class X86Encode
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
    int channels;
    int width;
    delegate*<ReadOnlySpan<float>, int, int, Span<byte>, int, void> kernel;

    public static IEnumerable<string> Cases() => X86Cases.Of(Avx512Variants.Encoders);

    [ParamsSource(nameof(Cases))]
    public string Case { get; set; } = "";

    Span<byte> Data => new(bytes, length);

    Span<float> Planes => new(samples, count);

    [GlobalSetup]
    public void Setup()
    {
        var parts = Case.Split(' ');
        var format = parts[0];
        channels = int.Parse(parts[1]);
        width = format switch { "S16" => 2, "S24" => 3, _ => 4 };
        length = Frames * channels * width;
        bytes = (byte*)NativeMemory.AlignedAlloc((nuint)length, 4096);
        count = channels * Stride + Layouts * LayoutStep;
        samples = (float*)NativeMemory.AlignedAlloc((nuint)count * 4, 4096);
        var planes = Planes;
        for (var i = 0; i < planes.Length; i++) planes[i] = (i * 7919 % 4001 - 2000) / 1900f;
        kernel = format switch
        {
            "S16" => &Interleave.Scalar<S16>,
            "S24" => &Interleave.Scalar<S24>,
            "S32" => &Interleave.Scalar<S32>,
            _ => &Interleave.Scalar<F32>
        };
        Data.Clear();
        Run();
        var expected = Data.ToArray();
        kernel = parts[2] switch
        {
            "avx2" => format switch
            {
                "S16" => Interleave.Avx2Kernel<S16>(channels),
                "S24" => Interleave.Avx2Kernel<S24>(channels),
                "S32" => Interleave.Avx2Kernel<S32>(channels),
                _ => Interleave.Avx2Kernel<F32>(channels)
            },
            "sse" => format switch
            {
                "S16" => SseInterleave.For<S16>(channels),
                "S24" => SseInterleave.For<S24>(channels),
                "S32" => SseInterleave.For<S32>(channels),
                _ => SseInterleave.For<F32>(channels)
            },
            _ => (delegate*<ReadOnlySpan<float>, int, int, Span<byte>, int, void>)X86Cases.Variant(Avx512Variants.Encoders, format, channels, parts[2])
        };
        Data.Clear();
        Run();
        if (!Data.SequenceEqual(expected)) throw new InvalidOperationException($"encode {Case}: the output differs from the scalar loop's");
    }

    [GlobalCleanup]
    public void Cleanup()
    {
        NativeMemory.AlignedFree(bytes);
        NativeMemory.AlignedFree(samples);
    }

    [Benchmark]
    public void Run()
    {
        var frameBytes = channels * width;
        for (var f = 0; f < Frames; f += Packet)
        {
            var frames = Math.Min(Packet, Frames - f);
            kernel(Planes[(f / Packet % Layouts * LayoutStep)..], Stride, channels, Data.Slice(f * frameBytes, frames * frameBytes), frames);
        }
    }
}
#endif
