using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using static SpriteEditor.Foundation.MathHelper;

namespace SpriteEditor.Services
{
    public class Image : IEquatable<Image>
    {
        private readonly PixelFormat pixelFormat;
        private readonly byte[] pixels;

        public unsafe Image(string filepath)
        {
            using var bmp = new Bitmap(filepath);
            Width = bmp.Width;
            Height = bmp.Height;

            var data = bmp.LockBits(
                new Rectangle(0, 0, Width, Height),
                ImageLockMode.ReadOnly,
                System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            var sizeInBytes = 4 * Width * Height;
            pixels = new byte[sizeInBytes];
            fixed (byte* destPtr = &pixels[0])
            {
                Buffer.MemoryCopy((void*)data.Scan0, destPtr, sizeInBytes, sizeInBytes);
            }

            pixelFormat = PixelFormat.Argb32;
        }

        public Image(byte[] pixels, int width, int height, PixelFormat pixelFormat = PixelFormat.Argb32)
        {
            this.pixels = pixels;
            Width = width;
            Height = height;
            this.pixelFormat = pixelFormat;
        }

        public static Image CreateDefault()
        {
            return new Image(new byte[] {
                0xFF, 0xFF, 0x00, 0x00,
                0xFF, 0x00, 0xFF, 0x00,
                0xFF, 0x00, 0x00, 0xFF,
                0xFF, 0xFF, 0xFF, 0xFF
            }, 2, 2);
        }

        public int Width { get; }

        public int Height { get; }

        public bool Equals(Image? other)
        {
            return
                other != null &&
                other.Width == Width &&
                other.Height == Height &&
                other.pixels.SequenceEqual(pixels);
        }

        public override bool Equals(object? obj)
        {
            return obj is Image other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Width, Height, pixels.GetHashCode());
        }

        public unsafe void Save(string path)
        {
            using var bitmap = new Bitmap(Width, Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            var data = bitmap.LockBits(
                new Rectangle(0, 0, Width, Height),
                ImageLockMode.WriteOnly,
                ToDrawingImaging(pixelFormat));

            var sizeInBytes = GetSizeInBytes(pixelFormat) * Width * Height;
            fixed (byte* sourcePtr = &pixels[0])
            {
                Buffer.MemoryCopy(sourcePtr, (void*)data.Scan0, sizeInBytes, sizeInBytes);
            }

            bitmap.UnlockBits(data);

            bitmap.Save(path);
        }

        public Image MovePixels(Vector displacement)
        {
            throw new NotImplementedException();

            var newPixels = new byte[pixels.Length];

            for (int x = 0; x < Width; ++x)
            {
                for (int y = 0; y < Height; ++y)
                {
                    int i = x + y * Width;
                    int i2 = Mod(x - displacement.X, Width) + Mod(y - displacement.Y, Height) * Width;

                    newPixels[i] = pixels[i2];
                }
            }

            return new(newPixels, Width, Height, pixelFormat);
        }

        public IEnumerable<Image> Split(int columns, int rows)
        {
            var size = new Vector(Width / columns, Height / rows);
            for (var y = 0; y < rows; ++y)
            {
                for (var x = 0; x < columns; ++x)
                {
                    yield return Cut(new Vector(size.X * x, size.Y * y), size);
                }
            }
        }

        public Image Cut(Vector topLeft, Vector size)
        {
            throw new NotImplementedException();

            var newPixels = new byte[size.X * size.Y];

            for (int x = 0; x < size.X; ++x)
            {
                for (int y = 0; y < size.Y; ++y)
                {
                    int newIndex = x + y * size.X;
                    int sourceIndex = x + topLeft.X + (y + topLeft.Y) * Width;

                    newPixels[newIndex] = pixels[sourceIndex];
                }
            }

            return new(newPixels, size.X, size.Y);
        }

        public ImageSource CreateSource(decimal zoom = 1)
        {
            var writeableBitmap = new WriteableBitmap(Width, Height, (double)(96 / zoom), (double)(96 / zoom), ToWinMedia(pixelFormat), null);
            writeableBitmap.WritePixels(new System.Windows.Int32Rect(0, 0, Width, Height), pixels, GetSizeInBytes(pixelFormat) * Width, 0, 0);
            return writeableBitmap;
        }

        private static System.Windows.Media.PixelFormat ToWinMedia(PixelFormat pixelFormat)
        {
            return pixelFormat switch
            {
                PixelFormat.Argb32 => PixelFormats.Bgra32,
                PixelFormat.Rgb24 => PixelFormats.Bgr24,
                _ => throw new NotSupportedException(),
            };
        }

        private static System.Drawing.Imaging.PixelFormat ToDrawingImaging(PixelFormat pixelFormat)
        {
            return pixelFormat switch
            {
                PixelFormat.Argb32 => System.Drawing.Imaging.PixelFormat.Format32bppArgb,
                PixelFormat.Rgb24 => System.Drawing.Imaging.PixelFormat.Format24bppRgb,
                _ => throw new NotSupportedException(),
            };
        }

        public static int GetSizeInBytes(PixelFormat pixelFormat)
        {
            return pixelFormat switch
            {
                PixelFormat.Argb32 => 4,
                PixelFormat.Rgb24 => 3,
                _ => throw new NotSupportedException(),
            };
        }
    }
}