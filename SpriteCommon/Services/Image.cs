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

            pixelFormat = PixelFormats.Argb8888;
        }

        public Image(byte[] pixels, int width, int height, PixelFormat pixelFormat)
        {
            this.pixels = pixels;
            Width = width;
            Height = height;
            this.pixelFormat = pixelFormat;
        }

        public static Image CreateDefault()
        {
            return new Image(new byte[] {
                0xFF, 0x00, 0x00,
                0x00, 0xFF, 0x00,
                0x00, 0x00, 0xFF,
                0xFF, 0xFF, 0xFF,
            }, 2, 2, PixelFormats.Rgb888);
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
            throw new NotImplementedException();

            //using var bitmap = new Bitmap(Width, Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            //var data = bitmap.LockBits(
            //    new Rectangle(0, 0, Width, Height),
            //    ImageLockMode.WriteOnly,
            //    pixelFormat.ToDrawingImaging());

            //var sizeInBytes = pixelFormat.SizeInBytes * Width * Height;
            //fixed (byte* sourcePtr = &pixels[0])
            //{
            //    Buffer.MemoryCopy(sourcePtr, (void*)data.Scan0, sizeInBytes, sizeInBytes);
            //}

            //bitmap.UnlockBits(data);

            //bitmap.Save(path);
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

                    for (int b = 0; b < pixelFormat.SizeInBytes; ++b)
                    {
                        newPixels[i * pixelFormat.SizeInBytes + b] = pixels[i2 * pixelFormat.SizeInBytes + b];
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

                    for (int b = 0; b < pixelFormat.SizeInBytes; ++b)
                    {
                        newPixels[newIndex * pixelFormat.SizeInBytes + b] = pixels[sourceIndex * pixelFormat.SizeInBytes + b];
                    }
                }
            }

            return new(newPixels, size.X, size.Y, pixelFormat);
        }

        public ImageSource CreateSource(decimal zoom = 1)
        {
            var writeableBitmap = new WriteableBitmap(
                Width, Height,
                (double)(96 / zoom), (double)(96 / zoom),
                System.Windows.Media.PixelFormats.Bgra32,
                null);

            var pixels = this.pixels;
            var realPixelCount = pixels.Length / pixelFormat.SizeInBytes;
            var transformedPixels = new byte[4 * realPixelCount];
            for (int i = 0; i < realPixelCount; ++i)
            {
                var pixelData = new Span<byte>(pixels, pixelFormat.SizeInBytes * i, pixelFormat.SizeInBytes);
                var pixelValue = GetPixelValue(pixelData);
                var r = GetChannelValue(pixelValue, pixelFormat.R);
                var g = GetChannelValue(pixelValue, pixelFormat.G);
                var b = GetChannelValue(pixelValue, pixelFormat.B);

                transformedPixels[4 * i + 0] = r;
                transformedPixels[4 * i + 1] = g;
                transformedPixels[4 * i + 2] = b;
                transformedPixels[4 * i + 3] = 255;
            }

            writeableBitmap.WritePixels(new System.Windows.Int32Rect(0, 0, Width, Height), transformedPixels, 4 * Width, 0, 0);
            return writeableBitmap;
        }

        private static byte GetChannelValue(uint pixelValue, Channel channel)
        {
            return (byte)((pixelValue & channel.Mask) >> channel.RightShift);
        }

        private static uint GetPixelValue(Span<byte> pixelData)
        {
            uint pixelValue = 0;

            for (int i = 0; i < pixelData.Length; ++i)
            {
                pixelValue <<= 8;
                pixelValue |= pixelData[i];
            }

            return pixelValue;
        }
    }
}