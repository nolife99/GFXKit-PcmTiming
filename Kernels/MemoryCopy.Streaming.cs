using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;

namespace GFXKit;

/// <summary>Whether a <see cref="MemoryCopy"/> copy may write with non-temporal stores, which send each line to memory without reading it into
/// the cache first. They suit a destination the processor won't read again soon, such as a large buffer another thread or the GPU consumes
/// later: the copy doesn't read the destination's lines, and it leaves the rest of the cache, the source among it, in place. A destination the
/// processor reads soon after, or one that fits in its cache, is faster written through the cache.</summary>
/// <remarks>Only x86 has them; on ARM64 every hint copies through the cache, as non-temporal stores timed no faster there. Memory mapped
/// write-combined, such as a buffer mapped for <see cref="BufferUsage.MapWrite"/>, never caches its lines, so the hint makes no difference to
/// it.</remarks>
public enum CopyHint
{
    /// <summary>Non-temporal from the size past which the destination can't stay in the cache the processor reads fastest from: the L2 on
    /// Intel and other vendors, half the L3 a core's complex shares on AMD and Hygon, as their L3s keep lines nearly as fast and the source takes
    /// the other half.</summary>
    Auto,

    /// <summary>Through the cache at every size.</summary>
    Cached,

    /// <summary>Non-temporal from the size of the L1 data cache, below which the store fence each such copy ends with costs more than it
    /// saves.</summary>
    Streaming,
}

public static unsafe partial class MemoryCopy
{
    internal static nuint StreamingFrom => Thresholds.From;

    internal static nuint StreamingFloor => Thresholds.Floor;

    [MethodImpl(Inline)]
    static bool Streams(nuint length, CopyHint hint) => hint switch
    {
        CopyHint.Auto => length >= Thresholds.From,
        CopyHint.Streaming => length >= Thresholds.Floor,
        _ => false
    };

    [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.AggressiveOptimization)]
    static void Stream(ref byte to, ref byte from, nuint length)
    {
        fixed (byte* target = &to)
        {
            if (Vector256.IsHardwareAccelerated) Stream256(target, ref from, length);
            else Stream128(target, ref from, length);
        }
    }

    [MethodImpl(Inline)]
    static void Stream256(byte* to, ref byte from, nuint length)
    {
        Unsafe.WriteUnaligned(to, Unsafe.ReadUnaligned<Vector256<byte>>(ref from));
        var skip = 32 - ((nuint)to & 31);
        ref var source = ref Unsafe.Add(ref from, skip);
        var end = to + length - 128;
        for (var target = to + skip; target <= end; source = ref Unsafe.Add(ref source, 128), target += 128)
        {
            var a = Unsafe.ReadUnaligned<Vector256<byte>>(ref source);
            var b = Unsafe.ReadUnaligned<Vector256<byte>>(ref Unsafe.Add(ref source, 32));
            var c = Unsafe.ReadUnaligned<Vector256<byte>>(ref Unsafe.Add(ref source, 64));
            var d = Unsafe.ReadUnaligned<Vector256<byte>>(ref Unsafe.Add(ref source, 96));
            Vector256.StoreAlignedNonTemporal(a, target);
            Vector256.StoreAlignedNonTemporal(b, target + 32);
            Vector256.StoreAlignedNonTemporal(c, target + 64);
            Vector256.StoreAlignedNonTemporal(d, target + 96);
        }
        Sse.StoreFence();
        Four<Vector256<byte>>(ref Unsafe.AsRef<byte>(end), ref Unsafe.Add(ref from, length - 128), 128);
    }

    [MethodImpl(Inline)]
    static void Stream128(byte* to, ref byte from, nuint length)
    {
        Unsafe.WriteUnaligned(to, Unsafe.ReadUnaligned<Vector128<byte>>(ref from));
        var skip = 16 - ((nuint)to & 15);
        ref var source = ref Unsafe.Add(ref from, skip);
        var end = to + length - 64;
        for (var target = to + skip; target <= end; source = ref Unsafe.Add(ref source, 64), target += 64)
        {
            var a = Unsafe.ReadUnaligned<Vector128<byte>>(ref source);
            var b = Unsafe.ReadUnaligned<Vector128<byte>>(ref Unsafe.Add(ref source, 16));
            var c = Unsafe.ReadUnaligned<Vector128<byte>>(ref Unsafe.Add(ref source, 32));
            var d = Unsafe.ReadUnaligned<Vector128<byte>>(ref Unsafe.Add(ref source, 48));
            Vector128.StoreAlignedNonTemporal(a, target);
            Vector128.StoreAlignedNonTemporal(b, target + 16);
            Vector128.StoreAlignedNonTemporal(c, target + 32);
            Vector128.StoreAlignedNonTemporal(d, target + 48);
        }
        Sse.StoreFence();
        Four<Vector128<byte>>(ref Unsafe.AsRef<byte>(end), ref Unsafe.Add(ref from, length - 64), 64);
    }

    static class Thresholds
    {
        public static readonly nuint From;
        public static readonly nuint Floor;

        static Thresholds()
        {
            From = Floor = nuint.MaxValue;
            if (!X86Base.IsSupported) return;
            var (highest, vendorB, vendorC, vendorD) = X86Base.CpuId(0, 0);
            var amd = vendorB == 0x68747541 && vendorD == 0x69746E65 && vendorC == 0x444D4163 || vendorB == 0x6F677948 && vendorD == 0x6E65476E && vendorC == 0x656E6975;
            nuint l1 = 0, l2 = 0, l3 = 0;
            if (amd)
            {
                var extended = (uint)X86Base.CpuId(unchecked((int)0x80000000), 0).Eax;
                if (extended >= 0x8000001D && (X86Base.CpuId(unchecked((int)0x80000001), 0).Ecx & (1 << 22)) != 0) Levels(unchecked((int)0x8000001D), ref l1, ref l2, ref l3);
                else if (extended >= 0x80000006)
                {
                    l1 = (nuint)((uint)X86Base.CpuId(unchecked((int)0x80000005), 0).Ecx >> 24) << 10;
                    var (_, _, ecx, edx) = X86Base.CpuId(unchecked((int)0x80000006), 0);
                    l2 = (nuint)((uint)ecx >> 16) << 10;
                    l3 = (nuint)((uint)edx >> 18) << 19;
                }
            }
            else if (highest >= 4) Levels(4, ref l1, ref l2, ref l3);
            if (l2 == 0) return;
            From = amd && l3 > 0 ? l3 / 2 : l2;
            Floor = l1 > 0 ? Math.Min(l1, From) : From;
        }

        static void Levels(int leaf, ref nuint l1, ref nuint l2, ref nuint l3)
        {
            for (var index = 0; index < 32; index++)
            {
                var (eax, ebx, ecx, _) = X86Base.CpuId(leaf, index);
                var type = eax & 31;
                if (type == 0) break;
                if (type == 2) continue;
                var size = (nuint)(((uint)ebx >> 22) + 1) * (((uint)ebx >> 12 & 0x3FF) + 1) * (((uint)ebx & 0xFFF) + 1) * ((uint)ecx + 1);
                switch (eax >> 5 & 7)
                {
                    case 1: l1 = size; break;
                    case 2: l2 = size; break;
                    case 3: l3 = size; break;
                }
            }
        }
    }
}
