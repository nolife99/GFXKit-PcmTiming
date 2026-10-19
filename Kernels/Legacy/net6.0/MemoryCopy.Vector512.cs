namespace GFXKit;

public static partial class MemoryCopy
{
    static bool Wide => false;

    static void Short512(ref byte to, ref byte from, nuint length) => throw new PlatformNotSupportedException();

    static void Loop512(ref byte to, ref byte from, nuint length) => throw new PlatformNotSupportedException();
}
