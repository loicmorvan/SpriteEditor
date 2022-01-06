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

            pixelFormat = PixelFormat.Argb8888;
        }

        public Image(byte[] pixels, int width, int height, PixelFormat pixelFormat = PixelFormat.Argb8888)
        {
            this.pixels = pixels;
            Width = width;
            Height = height;
            this.pixelFormat = pixelFormat;
        }

        public static Image CreateDefault()
        {
            return new Image(new byte[] {
                0x00, 0x00, 0xFF,
                0x00, 0xFF, 0x00,
                0xFF, 0x00, 0x00,
                0xFF, 0xFF, 0xFF,
            }, 2, 2, PixelFormat.Rgb888);
        }

        public int Width { get; }

        public int Height { get; }

        public bool Equals(Image? other)
        {
            return
                other != null &&
                other.Width == Width &&
                other.Height == Height &&
                other.pixelFormat == pixelFormat &&
                other.pixels.SequenceEqual(pixels);
        }

        public override bool Equals(object? obj)
        {
            return obj is Image other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Width, Height, pixels.GetHashCode(), pixelFormat);
        }

        public unsafe void Save(string path)
        {
            using var bitmap = new Bitmap(Width, Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            var data = bitmap.LockBits(
                new Rectangle(0, 0, Width, Height),
                ImageLockMode.WriteOnly,
                pixelFormat.ToDrawingImaging());

            var sizeInBytes = pixelFormat.GetSizeInBytes() * Width * Height;
            fixed (byte* sourcePtr = &pixels[0])
            {
                Buffer.MemoryCopy(sourcePtr, (void*)data.Scan0, sizeInBytes, sizeInBytes);
            }

            bitmap.UnlockBits(data);

            bitmap.Save(path);
        }

        public Image MovePixels(Vector displacement)
        {
            var newPixels = new byte[pixels.Length];

            for (int x = 0; x < Width; ++x)
            {
                for (int y = 0; y < Height; ++y)
                {
                    int i = x + y * Width;
                    int i2 = Mod(x - displacement.X, Width) + Mod(y - displacement.Y, Height) * Width;

                    for (int b = 0; b < pixelFormat.GetSizeInBytes(); ++b)
                    {
                        newPixels[i * pixelFormat.GetSizeInBytes() + b] = pixels[i2 * pixelFormat.GetSizeInBytes() + b];
                    }
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
            var newPixels = new byte[size.X * size.Y];

            for (int x = 0; x < size.X; ++x)
            {
                for (int y = 0; y < size.Y; ++y)
                {
                    int newIndex = x + y * size.X;
                    int sourceIndex = x + topLeft.X + (y + topLeft.Y) * Width;

                    for (int b = 0; b < pixelFormat.GetSizeInBytes(); ++b)
                    {
                        newPixels[newIndex * pixelFormat.GetSizeInBytes() + b] = pixels[sourceIndex * pixelFormat.GetSizeInBytes() + b];
                    }
                }
            }

            return new(newPixels, size.X, size.Y);
        }

        public ImageSource CreateSource(decimal zoom = 1)
        {
            var candidateFormat = pixelFormat switch
            {
                PixelFormat.Argb8888 => PixelFormats.Bgra32,
                PixelFormat.Argb4444 => PixelFormats.Bgra32,
                PixelFormat.Rgb888 => PixelFormats.Bgr24,
                PixelFormat.Bgr888 => PixelFormats.Rgb24,
                PixelFormat.Rgb565 => PixelFormats.Rgb24,
                PixelFormat.ExperimentalAlpha16 => PixelFormats.Bgra32,
                _ => throw new NotImplementedException(),
            };

            var writeableBitmap = new WriteableBitmap(Width, Height, (double)(96 / zoom), (double)(96 / zoom), candidateFormat, null);

            var pixels = this.pixels;
            switch (pixelFormat)
            {
                case PixelFormat.Argb4444:
                    {
                        var realPixelCount = pixels.Length / 2;
                        var newPixels = new byte[4 * realPixelCount];
                        for (int i = 0; i < realPixelCount; i++)
                        {
                            var r = (byte)(0xF0 & pixels[2 * i + 0] << 4);
                            var g = (byte)(0xF0 & pixels[2 * i + 0]);
                            var b = (byte)(0xF0 & pixels[2 * i + 1] << 4);
                            var a = (byte)(0xF0 & pixels[2 * i + 1]);

                            // TODO: Not sure.
                            newPixels[4 * i + 0] = r;
                            newPixels[4 * i + 1] = g;
                            newPixels[4 * i + 2] = b;
                            newPixels[4 * i + 3] = a;
                        }

                        pixels = newPixels;
                        break;
                    }

                case PixelFormat.Rgb565:
                    {
                        var realPixelCount = pixels.Length / 2;
                        var newPixels = new byte[3 * realPixelCount];
                        for (int i = 0; i < realPixelCount; ++i)
                        {
                            var c1 = pixels[2 * i + 0];
                            var c2 = pixels[2 * i + 1];
                            var r = (c1 & 0b_0001_1111) << 3;
                            var g = ((c1 & 0b_1110_0000) >> 5) + ((c2 & 0b_0000_0111) << 3) << 2;
                            var b = (c2 & 0b_1111_1000) << 3;

                            newPixels[3 * i + 0] = (byte)r;
                            newPixels[3 * i + 1] = (byte)g;
                            newPixels[3 * i + 2] = (byte)b;
                        }

                        pixels = newPixels;
                        break;
                    }

                case PixelFormat.ExperimentalAlpha16:
                    {
                        var realPixelCount = pixels.Length / 2;
                        var newPixels = new byte[4 * realPixelCount];
                        for (int i = 0; i < realPixelCount; i++)
                        {
                            var a = (byte)(0b_1110_0000 & pixels[2 * i + 1]);

                            newPixels[4 * i + 0] = a;
                            newPixels[4 * i + 1] = a;
                            newPixels[4 * i + 2] = a;
                            newPixels[4 * i + 3] = 255;
                        }

                        pixels = newPixels;
                        break;
                    }
            }

            var sizeInBytes = pixelFormat switch
            {
                PixelFormat.Argb8888 => 4,
                PixelFormat.Rgb888 => 3,
                PixelFormat.Bgr888 => 3,
                PixelFormat.Rgb565 => 3,
                PixelFormat.Argb4444 => 4,
                PixelFormat.ExperimentalAlpha16 => 4,
                _ => throw new NotImplementedException(),
            };

            writeableBitmap.WritePixels(new System.Windows.Int32Rect(0, 0, Width, Height), pixels, sizeInBytes * Width, 0, 0);
            return writeableBitmap;
        }
    }
}