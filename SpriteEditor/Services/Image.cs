namespace SpriteEditor.Services
{
    public class Image
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
    }
}