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
            Pixels = new uint[sizeInBytes];
            fixed (uint* destPtr = &Pixels[0])
            {
                Buffer.MemoryCopy((void*)data.Scan0, destPtr, sizeInBytes, sizeInBytes);
            }
        }

        public Image(uint[] pixels, int width, int height)
        {
            Pixels = pixels;
            Width = width;
            Height = height;
        }

        public uint[] Pixels { get; }

        public int Width { get; }

        public int Height { get; }

        public bool Equals(Image? other)
        {
            return
                other != null &&
                other.Width == Width &&
                other.Height == Height &&
                other.Pixels.SequenceEqual(Pixels);
        }

        public override bool Equals(object? obj)
        {
            return obj is Image other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Width, Height, Pixels.GetHashCode());
        }

        public unsafe void Save(string path)
        {
            using var bitmap = new Bitmap(Width, Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            var data = bitmap.LockBits(new Rectangle(0, 0, Width, Height), ImageLockMode.WriteOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            var sizeInBytes = 4 * Width * Height;
            fixed (uint* sourcePtr = &Pixels[0])
            {
                Buffer.MemoryCopy(sourcePtr, (void*)data.Scan0, sizeInBytes, sizeInBytes);
            }

            bitmap.UnlockBits(data);

            bitmap.Save(path);
        }

        public Image MovePixels(Vector displacement)
        {
            var newPixels = new uint[Pixels.Length];

            for (int x = 0; x < Width; ++x)
            {
                for (int y = 0; y < Height; ++y)
                {
                    int i = x + y * Width;
                    int i2 = Mod(x - displacement.X, Width) + Mod(y - displacement.Y, Height) * Width;

                    newPixels[i] = Pixels[i2];
                }
            }

            return new(newPixels, Width, Height);
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
            var newPixels = new uint[size.X * size.Y];

            for (int x = 0; x < size.X; ++x)
            {
                for (int y = 0; y < size.Y; ++y)
                {
                    int newIndex = x + y * size.X;
                    int sourceIndex = x + topLeft.X + (y + topLeft.Y) * Width;

                    newPixels[newIndex] = Pixels[sourceIndex];
                }
            }

            return new(newPixels, size.X, size.Y);
        }

        public ImageSource CreateSource()
        {
            var writeableBitmap = new WriteableBitmap(Width, Height, 96, 96, PixelFormats.Bgra32, null);
            writeableBitmap.WritePixels(new System.Windows.Int32Rect(0, 0, Width, Height), Pixels, 4 * Width, 0, 0);
            return writeableBitmap;
        }
    }
}