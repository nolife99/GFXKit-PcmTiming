namespace GFXKit;

public static partial class MemoryCopy
{
    static partial class Thresholds
    {
        public static readonly bool Wide = false;
    }

    static void Wide(ref byte to, ref byte from, nuint length) => throw new PlatformNotSupportedException();
}
