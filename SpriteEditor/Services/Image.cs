using System;
using System.Linq;

namespace SpriteEditor.Services
{
    public class Image : IEquatable<Image>
    {
        public Image(uint[] pixels, int width, int height)
        {
            Pixels=pixels;
            Width=width;
            Height=height;
        }

        public uint[] Pixels { get; }

        public int Width { get; }

        public int Height { get; }

        public bool Equals(Image? other)
        {
            return other != null &&
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
    }
}