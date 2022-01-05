using System;
using System.Windows.Media;

namespace SpriteEditor.Services
{
    public static class PixelFormatEx
    {
        public static System.Drawing.Imaging.PixelFormat ToDrawingImaging(this PixelFormat pixelFormat)
        {
            return pixelFormat switch
            {
                PixelFormat.Argb8888 => System.Drawing.Imaging.PixelFormat.Format32bppArgb,
                PixelFormat.Rgb888 => System.Drawing.Imaging.PixelFormat.Format24bppRgb,
                PixelFormat.Bgr888 => throw new NotSupportedException(),
                PixelFormat.Argb4444 => throw new NotSupportedException(),
                _ => throw new NotImplementedException(),
            };
        }

        public static int GetSizeInBytes(this PixelFormat pixelFormat)
        {
            return pixelFormat switch
            {
                PixelFormat.Argb8888 => 4,
                PixelFormat.Rgb888 => 3,
                PixelFormat.Bgr888 => 3,
                PixelFormat.Argb4444 => 2,
                _ => throw new NotImplementedException(),
            };
        }
    }
}