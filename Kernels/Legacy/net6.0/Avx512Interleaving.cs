namespace GFXKit.Media.Codecs;

static unsafe class Avx512Deinterleave
{
    public static delegate*<ReadOnlySpan<byte>, int, Span<float>, int, int, void> For<T>(int channels) where T : struct, IPcmSample => null;
}

static unsafe class Avx512Interleave
{
    public static delegate*<ReadOnlySpan<float>, int, int, Span<byte>, int, void> For<T>(int channels) where T : struct, IPcmTarget => null;
}
