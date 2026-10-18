using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics;

namespace GFXKit.Media.Codecs;

readonly ref struct Planes
{
    readonly Span<float> data;
    readonly int stride;
    readonly int channels;
    readonly int frames;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Planes(Span<float> data, int stride, int channels, int frames)
    {
        Check(data.Length, stride, channels, frames);
        this.data = data;
        (this.stride, this.channels, this.frames) = (stride, channels, frames);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Planes(ReadOnlySpan<float> data, int stride, int channels, int frames) : this(MemoryMarshal.CreateSpan(ref MemoryMarshal.GetReference(data), data.Length), stride, channels, frames) { }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Check(int length, int stride, int channels, int frames)
    {
        if (channels <= 0 || frames < 0 || stride < frames || (long)(channels - 1) * stride + frames > length) Throws.Planes(length, stride, channels, frames);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Block At(int frame, int width)
    {
        if (frame < 0 || frame > frames - width) Throws.Frames(frame, width, frames);
        return new Block(ref Unsafe.Add(ref MemoryMarshal.GetReference(data), frame), stride, channels, width);
    }

    public readonly ref struct Block
    {
        readonly Span<float> start;
        readonly int stride;
        readonly int channels;
        readonly int width;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Block(ref float start, int stride, int channels, int width)
        {
            this.start = MemoryMarshal.CreateSpan(ref start, 1);
            (this.stride, this.channels, this.width) = (stride, channels, width);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref float At(int channel, int frame, int count)
        {
            if ((uint)channel >= (uint)channels || (uint)frame + (uint)count > (uint)width) Throws.Channel(channel, frame, count, channels, width);
            return ref Unsafe.Add(ref MemoryMarshal.GetReference(start), channel * stride + frame);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Store(int channel, int frame, Vector256<float> values) => Unsafe.WriteUnaligned(ref Unsafe.As<float, byte>(ref At(channel, frame, 8)), values);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Store(int channel, int frame, Vector128<float> values) => Unsafe.WriteUnaligned(ref Unsafe.As<float, byte>(ref At(channel, frame, 4)), values);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Vector256<float> Load8(int channel, int frame) => Unsafe.ReadUnaligned<Vector256<float>>(ref Unsafe.As<float, byte>(ref At(channel, frame, 8)));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Vector128<float> Load4(int channel, int frame) => Unsafe.ReadUnaligned<Vector128<float>>(ref Unsafe.As<float, byte>(ref At(channel, frame, 4)));
    }
}

static class Packed
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ref byte At(ReadOnlySpan<byte> block, int offset, int count)
    {
        if ((ulong)(uint)offset + (uint)count > (uint)block.Length) Throws.Bytes(offset, count, block.Length);
        return ref Unsafe.Add(ref MemoryMarshal.GetReference(block), offset);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ref byte At(Span<byte> block, int offset, int count)
    {
        if ((ulong)(uint)offset + (uint)count > (uint)block.Length) Throws.Bytes(offset, count, block.Length);
        return ref Unsafe.Add(ref MemoryMarshal.GetReference(block), offset);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector256<byte> Read32(scoped ReadOnlySpan<byte> block, int offset) => Unsafe.ReadUnaligned<Vector256<byte>>(ref At(block, offset, 32));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector128<byte> Read16(scoped ReadOnlySpan<byte> block, int offset) => Unsafe.ReadUnaligned<Vector128<byte>>(ref At(block, offset, 16));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector128<byte> Read8(scoped ReadOnlySpan<byte> block, int offset) => Vector128.CreateScalar(Unsafe.ReadUnaligned<long>(ref At(block, offset, 8))).AsByte();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int Read4(scoped ReadOnlySpan<byte> block, int offset) => Unsafe.ReadUnaligned<int>(ref At(block, offset, 4));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Write32(scoped Span<byte> block, int offset, Vector256<byte> values) => Unsafe.WriteUnaligned(ref At(block, offset, 32), values);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Write16(scoped Span<byte> block, int offset, Vector128<byte> values) => Unsafe.WriteUnaligned(ref At(block, offset, 16), values);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Write8(scoped Span<byte> block, int offset, Vector128<byte> values) => Unsafe.WriteUnaligned(ref At(block, offset, 8), values.AsInt64().ToScalar());

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Write4(scoped Span<byte> block, int offset, int value) => Unsafe.WriteUnaligned(ref At(block, offset, 4), value);
}

readonly ref struct Interleaved
{
    readonly ReadOnlySpan<byte> data;
    readonly int frameBytes;
    readonly int frames;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Check(int length, int channels, int bytes, int frames)
    {
        if (channels <= 0 || frames < 0 || frames > length / ((long)channels * bytes)) Throws.Fit(frames, channels * bytes, length);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Interleaved(ReadOnlySpan<byte> data, int frameBytes, int frames)
    {
        if (frames < 0 || (long)frames * frameBytes > data.Length) Throws.Fit(frames, frameBytes, data.Length);
        this.data = data;
        (this.frameBytes, this.frames) = (frameBytes, frames);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ReadOnlySpan<byte> At(int frame, int width)
    {
        if (frame < 0 || frame > frames - width) Throws.Frames(frame, width, frames);
        return MemoryMarshal.CreateReadOnlySpan(ref Unsafe.Add(ref MemoryMarshal.GetReference(data), frame * frameBytes), width * frameBytes);
    }
}

readonly ref struct InterleavedTarget
{
    readonly Span<byte> data;
    readonly int frameBytes;
    readonly int frames;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public InterleavedTarget(Span<byte> data, int frameBytes, int frames)
    {
        if (frames < 0 || (long)frames * frameBytes > data.Length) Throws.Fit(frames, frameBytes, data.Length);
        this.data = data;
        (this.frameBytes, this.frames) = (frameBytes, frames);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Span<byte> At(int frame, int width)
    {
        if (frame < 0 || frame > frames - width) Throws.Frames(frame, width, frames);
        return MemoryMarshal.CreateSpan(ref Unsafe.Add(ref MemoryMarshal.GetReference(data), frame * frameBytes), width * frameBytes);
    }
}

unsafe readonly ref struct Pinned
{
    readonly byte* data;
    readonly int frameBytes;
    readonly int frames;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Pinned(byte* data, int length, int frameBytes, int frames)
    {
        if (frames < 0 || (long)frames * frameBytes > length) Throws.Fit(frames, frameBytes, length);
        this.data = data;
        (this.frameBytes, this.frames) = (frameBytes, frames);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public byte* At(int frame, int width)
    {
        if (frame < 0 || frame > frames - width) Throws.Frames(frame, width, frames);
        return data + frame * frameBytes;
    }
}

static class Throws
{
    [DoesNotReturn]
    public static void Planes(int length, int stride, int channels, int frames) =>
        throw new ArgumentException($"{channels} channels of {frames} frames {stride} apart don't fit in {length} samples");

    [DoesNotReturn]
    public static void Fit(int frames, int frameBytes, int length) =>
        throw new ArgumentException($"{frames} frames of {frameBytes} bytes don't fit in {length}");

    [DoesNotReturn]
    public static void Frames(int frame, int width, int frames) =>
        throw new ArgumentOutOfRangeException(nameof(frame), frame, $"frames {frame} to {frame + width} are outside the {frames}");

    [DoesNotReturn]
    public static void Channel(int channel, int frame, int count, int channels, int width) =>
        throw new ArgumentOutOfRangeException(nameof(channel), $"channel {channel}, frames {frame} to {frame + count} are outside the block's {channels} channels of {width}");

    [DoesNotReturn]
    public static void Bytes(int offset, int count, int length) =>
        throw new ArgumentOutOfRangeException(nameof(offset), offset, $"bytes {offset} to {offset + count} are outside the block's {length}");
}
