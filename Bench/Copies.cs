#if NET8_0_OR_GREATER
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics;
using BenchmarkDotNet.Attributes;

namespace Bench;

public unsafe class Copies
{
    const int Frames = 48000;
    const int Packet = 1024;
    const int Layouts = 8;
    const int LayoutStep = 128;

    byte* bytes;
    float* samples;

    [Params("decode", "encode")]
    public string Direction { get; set; } = "decode";

    [Params("CopyTo", "CopyBlockUnaligned", "Vectors")]
    public string Method { get; set; } = "CopyTo";

    [GlobalSetup]
    public void Setup()
    {
        bytes = (byte*)NativeMemory.AlignedAlloc(Frames * 4, 4096);
        samples = (float*)NativeMemory.AlignedAlloc((nuint)(Packet + Layouts * LayoutStep) * 4, 4096);
        new Random(1).NextBytes(new Span<byte>(bytes, Frames * 4));
        new Span<float>(samples, Packet + Layouts * LayoutStep).Clear();
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
        var method = Method;
        for (var f = 0; f < Frames; f += Packet)
        {
            var frames = Math.Min(Packet, Frames - f);
            var data = new Span<byte>(bytes + f * 4, frames * 4);
            var plane = new Span<byte>(samples + f / Packet % Layouts * LayoutStep, frames * 4);
            if (Direction == "decode") Copy(method, data, plane);
            else Copy(method, plane, data);
        }
    }

    static void Copy(string method, scoped ReadOnlySpan<byte> from, scoped Span<byte> to)
    {
        if (method == "CopyTo") from.CopyTo(to);
        else if (method == "CopyBlockUnaligned") Unsafe.CopyBlockUnaligned(ref MemoryMarshal.GetReference(to), ref MemoryMarshal.GetReference(from), (uint)from.Length);
        else Vectors(from, to);
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    static void Vectors(scoped ReadOnlySpan<byte> from, scoped Span<byte> to)
    {
        ref var source = ref MemoryMarshal.GetReference(from);
        ref var target = ref MemoryMarshal.GetReference(to);
        var length = (nuint)from.Length;
        nuint at = 0;
        if (Vector512.IsHardwareAccelerated)
            for (; at + 64 <= length; at += 64) Vector512.LoadUnsafe(ref source, at).StoreUnsafe(ref target, at);
        for (; at + 32 <= length; at += 32) Vector256.LoadUnsafe(ref source, at).StoreUnsafe(ref target, at);
        for (; at < length; at++) Unsafe.Add(ref target, at) = Unsafe.Add(ref source, at);
    }
}
#endif
