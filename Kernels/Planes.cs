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
    public ref float At(int frame, int width)
    {
        if (frame < 0 || frame > frames - width) Throws.Frames(frame, width, frames);
        return ref Unsafe.Add(ref MemoryMarshal.GetReference(data), frame);
    }
}

static class Blocks
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Store(this ref float block, int stride, int channel, int frame, Vector256<float> values) =>
        Unsafe.WriteUnaligned(ref Unsafe.As<float, byte>(ref Unsafe.Add(ref block, channel * stride + frame)), values);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Store(this ref float block, int stride, int channel, int frame, Vector128<float> values) =>
        Unsafe.WriteUnaligned(ref Unsafe.As<float, byte>(ref Unsafe.Add(ref block, channel * stride + frame)), values);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector256<float> Load8(this ref float block, int stride, int channel, int frame) =>
        Unsafe.ReadUnaligned<Vector256<float>>(ref Unsafe.As<float, byte>(ref Unsafe.Add(ref block, channel * stride + frame)));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector128<float> Load4(this ref float block, int stride, int channel, int frame) =>
        Unsafe.ReadUnaligned<Vector128<float>>(ref Unsafe.As<float, byte>(ref Unsafe.Add(ref block, channel * stride + frame)));
}

static class Packed
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector256<byte> Read32(ref byte block, int offset) => Unsafe.ReadUnaligned<Vector256<byte>>(ref Unsafe.Add(ref block, offset));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector128<byte> Read16(ref byte block, int offset) => Unsafe.ReadUnaligned<Vector128<byte>>(ref Unsafe.Add(ref block, offset));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector128<byte> Read8(ref byte block, int offset) => Vector128.CreateScalar(Unsafe.ReadUnaligned<long>(ref Unsafe.Add(ref block, offset))).AsByte();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int Read4(ref byte block, int offset) => Unsafe.ReadUnaligned<int>(ref Unsafe.Add(ref block, offset));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Write32(ref byte block, int offset, Vector256<byte> values) => Unsafe.WriteUnaligned(ref Unsafe.Add(ref block, offset), values);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Write16(ref byte block, int offset, Vector128<byte> values) => Unsafe.WriteUnaligned(ref Unsafe.Add(ref block, offset), values);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Write8(ref byte block, int offset, Vector128<byte> values) => Unsafe.WriteUnaligned(ref Unsafe.Add(ref block, offset), values.AsInt64().ToScalar());

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Write4(ref byte block, int offset, int value) => Unsafe.WriteUnaligned(ref Unsafe.Add(ref block, offset), value);
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
    public ref byte At(int frame, int width)
    {
        if (frame < 0 || frame > frames - width) Throws.Frames(frame, width, frames);
        return ref Unsafe.Add(ref MemoryMarshal.GetReference(data), frame * frameBytes);
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
    public ref byte At(int frame, int width)
    {
        if (frame < 0 || frame > frames - width) Throws.Frames(frame, width, frames);
        return ref Unsafe.Add(ref MemoryMarshal.GetReference(data), frame * frameBytes);
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


}
