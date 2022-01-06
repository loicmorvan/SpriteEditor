namespace SpriteEditor.Services
{
    public static class PixelFormats
    {
        public static PixelFormat Argb8888 { get; } = new PixelFormat(
            4,
            new(0b_00000000_11111111_00000000_00000000, 16),
            new(0b_00000000_00000000_11111111_00000000, 8),
            new(0b_00000000_00000000_00000000_11111111, 0),
            new(0b_11111111_00000000_00000000_00000000, 24));

        public static PixelFormat Argb4444 { get; } = new PixelFormat(
            2,
            new(0b_00001111_00000000, 4),
            new(0b_00000000_11110000, 0),
            new(0b_00000000_00001111, -4),
            new(0b_11110000_00000000, 8));

        public static PixelFormat Rgb888 { get; } = new PixelFormat(
            3,
            new(0b_11111111_00000000_00000000, 16),
            new(0b_00000000_11111111_00000000, 8),
            new(0b_00000000_00000000_11111111, 0));

        public static PixelFormat Rgb565 { get; } = new PixelFormat(
            2,
            new(0b_11111000_00000000, 8),
            new(0b_00000111_11100000, 3),
            new(0b_00000000_00011111, -3));

        public static PixelFormat Bgr888 { get; } = new PixelFormat(
            3,
            new(0b_00000000_00000000_11111111, 0),
            new(0b_00000000_11111111_00000000, 8),
            new(0b_11111111_00000000_00000000, 16));
    }
}