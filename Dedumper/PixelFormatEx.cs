using System;

namespace Dedumper
{
    public static class PixelFormatEx
    {
        public static SpriteEditor.Services.PixelFormat ToImageFormat(this PixelFormat pixelFormat)
        {
            return pixelFormat switch
            {
                PixelFormat.Argb8888 => SpriteEditor.Services.PixelFormats.Argb8888,
                PixelFormat.Argb4444 => SpriteEditor.Services.PixelFormats.Argb4444,
                PixelFormat.Rgb888 => SpriteEditor.Services.PixelFormats.Rgb888,
                PixelFormat.Rgb565 => SpriteEditor.Services.PixelFormats.Rgb565,
                PixelFormat.Bgr888 => SpriteEditor.Services.PixelFormats.Bgr888,
                _ => throw new NotImplementedException(),
            };
        }
    }
}
