namespace GFXKit.Media;

/// <summary>
/// Planar float audio: channel <c>c</c> holds <see cref="Frames"/> samples from <c>c * Stride</c>, so a buffer of several channels is one span and a
/// view of fewer frames keeps its stride.
/// </summary>
public readonly ref struct PlanarSpan
{
    readonly Span<float> data;

    /// <summary>The frames each channel holds.</summary>
    public int Frames { get; }

    /// <summary>The number of channels.</summary>
    public int Channels { get; }

    /// <summary>The distance from one channel's first sample to the next channel's, at least <see cref="Frames"/>.</summary>
    public int Stride { get; }

    /// <exception cref="ArgumentOutOfRangeException">A count is negative, the stride is less than the frames, or the span is shorter than the channels need.</exception>
    public PlanarSpan(Span<float> data, int channels, int frames, int stride)
    {
        Check(data.Length, channels, frames, stride);
        this.data = data;
        Channels = channels;
        Frames = frames;
        Stride = stride;
    }

    /// <summary>A buffer whose channels follow each other with no gap.</summary>
    public PlanarSpan(Span<float> data, int channels, int frames) : this(data, channels, frames, frames) { }

    internal Span<float> Storage => data;

    /// <summary>The samples of one channel.</summary>
    public Span<float> this[int channel]
    {
        get
        {
            if ((uint)channel >= (uint)Channels) throw new ArgumentOutOfRangeException(nameof(channel), channel, $"the span holds {Channels} channels");
            return data.Slice(channel * Stride, Frames);
        }
    }

    /// <summary>The frames from <paramref name="start"/> on, keeping the stride.</summary>
    public PlanarSpan Slice(int start) => Slice(start, Frames - start);

    /// <summary><paramref name="length"/> frames from <paramref name="start"/>, keeping the stride.</summary>
    public PlanarSpan Slice(int start, int length)
    {
        if ((uint)start > (uint)Frames || (uint)length > (uint)(Frames - start))
            throw new ArgumentOutOfRangeException(nameof(start), $"frames {start} to {start + length} are outside the span's {Frames}");
        return Channels == 0 ? new PlanarSpan(data, 0, length, Stride) : new PlanarSpan(data[start..], Channels, length, Stride);
    }

    /// <summary>Sets every sample of every channel to zero.</summary>
    public void Clear()
    {
        for (var c = 0; c < Channels; c++) this[c].Clear();
    }

    internal static void Check(int length, int channels, int frames, int stride)
    {
        if (channels < 0) throw new ArgumentOutOfRangeException(nameof(channels), channels, "the channel count is negative");
        if (frames < 0) throw new ArgumentOutOfRangeException(nameof(frames), frames, "the frame count is negative");
        if (stride < frames) throw new ArgumentOutOfRangeException(nameof(stride), stride, $"the stride is less than the {frames} frames");
        if (channels > 0 && (long)(channels - 1) * stride + frames > length)
            throw new ArgumentOutOfRangeException(nameof(length), $"{channels} channels of {frames} frames {stride} apart need {(long)(channels - 1) * stride + frames} samples, and the buffer holds {length}");
    }
}

/// <summary>The storage of a <see cref="PlanarSpan"/> that can be kept, for asynchronous reads.</summary>
public readonly struct PlanarMemory
{
    readonly Memory<float> data;

    public int Frames { get; }

    public int Channels { get; }

    public int Stride { get; }

    /// <exception cref="ArgumentOutOfRangeException">A count is negative, the stride is less than the frames, or the memory is shorter than the channels need.</exception>
    public PlanarMemory(Memory<float> data, int channels, int frames, int stride)
    {
        PlanarSpan.Check(data.Length, channels, frames, stride);
        this.data = data;
        Channels = channels;
        Frames = frames;
        Stride = stride;
    }

    public PlanarMemory(Memory<float> data, int channels, int frames) : this(data, channels, frames, frames) { }

    public PlanarSpan Span => new(data.Span, Channels, Frames, Stride);

    /// <summary><paramref name="length"/> frames from <paramref name="start"/>, keeping the stride.</summary>
    public PlanarMemory Slice(int start, int length)
    {
        if ((uint)start > (uint)Frames || (uint)length > (uint)(Frames - start))
            throw new ArgumentOutOfRangeException(nameof(start), $"frames {start} to {start + length} are outside the memory's {Frames}");
        return Channels == 0 ? new PlanarMemory(data, 0, length, Stride) : new PlanarMemory(data[start..], Channels, length, Stride);
    }
}
