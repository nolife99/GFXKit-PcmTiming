#if NET9_0_OR_GREATER
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.Arm;
using System.Runtime.Intrinsics.X86;
using BenchmarkDotNet.Attributes;

namespace Bench;

public unsafe class Copies
{
    const int Stream = 48000 * 4;
    const int Layouts = 8;
    const int LayoutStep = 512;

    byte* bytes;
    byte* plane;
    bool decode;
    delegate*<byte*, byte*, nuint, void> copy;

    [Params("decode", "encode")]
    public string Direction { get; set; } = "decode";

    [Params(4096, 65536)]
    public int Size { get; set; }

    [ParamsSource(nameof(Methods))]
    public string Method { get; set; } = "CopyTo";

    public static IEnumerable<string> Methods()
    {
        yield return "CopyTo";
        if (Vector512.IsHardwareAccelerated) yield return "Tuned512";
        if (Avx.IsSupported)
        {
            yield return "Tuned256";
            yield return "Tuned256NT";
        }
        if (Vector512.IsHardwareAccelerated) yield return "Tuned512NT";
        if (AdvSimd.Arm64.IsSupported)
        {
            yield return "Tuned128";
            yield return "Pairs";
            yield return "PairsNT";
            yield return "Multi";
        }
    }

    [GlobalSetup]
    public void Setup()
    {
        bytes = (byte*)NativeMemory.AlignedAlloc(Stream, 4096);
        plane = (byte*)NativeMemory.AlignedAlloc((nuint)(Size + Layouts * LayoutStep), 4096);
        new Random(1).NextBytes(new Span<byte>(bytes, Stream));
        new Random(2).NextBytes(new Span<byte>(plane, Size + Layouts * LayoutStep));
        decode = Direction == "decode";
        copy = Method switch
        {
            "CopyTo" => &CopyTo,
            "Tuned512" => &Tuned512Cached,
            "Tuned512NT" => &Tuned512Streaming,
            "Tuned256" => &Tuned256Cached,
            "Tuned256NT" => &Tuned256Streaming,
            "Tuned128" => &Tuned128,
            "Pairs" => &PairsCached,
            "PairsNT" => &PairsStreaming,
            _ => &Multi
        };
        var expected = new byte[Size];
        var actual = new byte[Size];
        fixed (byte* e = expected, a = actual)
            for (var offset = 0; offset < 64; offset += 13)
            {
                var length = (nuint)(Size - offset);
                CopyTo(bytes + offset, e + offset % 7, length);
                copy(bytes + offset, a + offset % 7, length);
                if (!expected.AsSpan().SequenceEqual(actual)) throw new InvalidOperationException($"{Method} copied wrongly at offset {offset}");
            }
    }

    [GlobalCleanup]
    public void Cleanup()
    {
        NativeMemory.AlignedFree(bytes);
        NativeMemory.AlignedFree(plane);
    }

    [Benchmark]
    public void Run()
    {
        var size = Size;
        for (int at = 0, packet = 0; at + size <= Stream; at += size, packet++)
        {
            var slot = plane + packet % Layouts * LayoutStep;
            if (decode) copy(bytes + at, slot, (nuint)size);
            else copy(slot, bytes + at, (nuint)size);
        }
    }

    static void CopyTo(byte* from, byte* to, nuint length) => new ReadOnlySpan<byte>(from, (int)length).CopyTo(new Span<byte>(to, (int)length));

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    static void Tuned512Cached(byte* from, byte* to, nuint length) => Tuned512(from, to, length, false);

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    static void Tuned512Streaming(byte* from, byte* to, nuint length) => Tuned512(from, to, length, true);

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    static void Tuned256Cached(byte* from, byte* to, nuint length) => Tuned256(from, to, length, false);

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    static void Tuned256Streaming(byte* from, byte* to, nuint length) => Tuned256(from, to, length, true);

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    static void PairsCached(byte* from, byte* to, nuint length) => Pairs(from, to, length, false);

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    static void PairsStreaming(byte* from, byte* to, nuint length) => Pairs(from, to, length, true);

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    static void Tuned512(byte* from, byte* to, nuint length, bool streaming)
    {
        var end = to + length;
        var last = from + length;
        Vector512.Store(Vector512.Load(from), to);
        var skip = 64 - ((nuint)to & 63);
        from += skip;
        to += skip;
        for (; to + 256 <= end; from += 256, to += 256)
        {
            var a = Vector512.Load(from);
            var b = Vector512.Load(from + 64);
            var c = Vector512.Load(from + 128);
            var d = Vector512.Load(from + 192);
            if (streaming)
            {
                Vector512.StoreAlignedNonTemporal(a, to);
                Vector512.StoreAlignedNonTemporal(b, to + 64);
                Vector512.StoreAlignedNonTemporal(c, to + 128);
                Vector512.StoreAlignedNonTemporal(d, to + 192);
            }
            else
            {
                Vector512.StoreAligned(a, to);
                Vector512.StoreAligned(b, to + 64);
                Vector512.StoreAligned(c, to + 128);
                Vector512.StoreAligned(d, to + 192);
            }
        }
        if (streaming) Sse.StoreFence();
        Vector512.Store(Vector512.Load(last - 256), end - 256);
        Vector512.Store(Vector512.Load(last - 192), end - 192);
        Vector512.Store(Vector512.Load(last - 128), end - 128);
        Vector512.Store(Vector512.Load(last - 64), end - 64);
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    static void Tuned256(byte* from, byte* to, nuint length, bool streaming)
    {
        var end = to + length;
        var last = from + length;
        Vector256.Store(Vector256.Load(from), to);
        var skip = 32 - ((nuint)to & 31);
        from += skip;
        to += skip;
        for (; to + 128 <= end; from += 128, to += 128)
        {
            var a = Vector256.Load(from);
            var b = Vector256.Load(from + 32);
            var c = Vector256.Load(from + 64);
            var d = Vector256.Load(from + 96);
            if (streaming)
            {
                Vector256.StoreAlignedNonTemporal(a, to);
                Vector256.StoreAlignedNonTemporal(b, to + 32);
                Vector256.StoreAlignedNonTemporal(c, to + 64);
                Vector256.StoreAlignedNonTemporal(d, to + 96);
            }
            else
            {
                Vector256.StoreAligned(a, to);
                Vector256.StoreAligned(b, to + 32);
                Vector256.StoreAligned(c, to + 64);
                Vector256.StoreAligned(d, to + 96);
            }
        }
        if (streaming) Sse.StoreFence();
        Vector256.Store(Vector256.Load(last - 128), end - 128);
        Vector256.Store(Vector256.Load(last - 96), end - 96);
        Vector256.Store(Vector256.Load(last - 64), end - 64);
        Vector256.Store(Vector256.Load(last - 32), end - 32);
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    static void Tuned128(byte* from, byte* to, nuint length)
    {
        var end = to + length;
        var last = from + length;
        Vector128.Store(Vector128.Load(from), to);
        var skip = 16 - ((nuint)to & 15);
        from += skip;
        to += skip;
        for (; to + 64 <= end; from += 64, to += 64)
        {
            var a = Vector128.Load(from);
            var b = Vector128.Load(from + 16);
            var c = Vector128.Load(from + 32);
            var d = Vector128.Load(from + 48);
            Vector128.StoreAligned(a, to);
            Vector128.StoreAligned(b, to + 16);
            Vector128.StoreAligned(c, to + 32);
            Vector128.StoreAligned(d, to + 48);
        }
        Vector128.Store(Vector128.Load(last - 64), end - 64);
        Vector128.Store(Vector128.Load(last - 48), end - 48);
        Vector128.Store(Vector128.Load(last - 32), end - 32);
        Vector128.Store(Vector128.Load(last - 16), end - 16);
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    static void Pairs(byte* from, byte* to, nuint length, bool streaming)
    {
        var end = to + length;
        var last = from + length;
        Vector128.Store(Vector128.Load(from), to);
        var skip = 16 - ((nuint)to & 15);
        from += skip;
        to += skip;
        for (; to + 64 <= end; from += 64, to += 64)
        {
            if (streaming)
            {
                var (a, b) = AdvSimd.Arm64.LoadPairVector128NonTemporal(from);
                var (c, d) = AdvSimd.Arm64.LoadPairVector128NonTemporal(from + 32);
                AdvSimd.Arm64.StorePairNonTemporal(to, a, b);
                AdvSimd.Arm64.StorePairNonTemporal(to + 32, c, d);
            }
            else
            {
                var (a, b) = AdvSimd.Arm64.LoadPairVector128(from);
                var (c, d) = AdvSimd.Arm64.LoadPairVector128(from + 32);
                AdvSimd.Arm64.StorePair(to, a, b);
                AdvSimd.Arm64.StorePair(to + 32, c, d);
            }
        }
        var (e, f) = AdvSimd.Arm64.LoadPairVector128(last - 64);
        var (g, h) = AdvSimd.Arm64.LoadPairVector128(last - 32);
        AdvSimd.Arm64.StorePair(end - 64, e, f);
        AdvSimd.Arm64.StorePair(end - 32, g, h);
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    static void Multi(byte* from, byte* to, nuint length)
    {
        var end = to + length;
        var last = from + length;
        Vector128.Store(Vector128.Load(from), to);
        var skip = 16 - ((nuint)to & 15);
        from += skip;
        to += skip;
        for (; to + 64 <= end; from += 64, to += 64) AdvSimd.Arm64.Store(to, AdvSimd.Arm64.Load4xVector128(from));
        AdvSimd.Arm64.Store(end - 64, AdvSimd.Arm64.Load4xVector128(last - 64));
    }
}
#endif
