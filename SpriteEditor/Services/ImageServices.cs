using System;
using System.Drawing;
using static SpriteEditor.Foundation.MathHelper;

namespace SpriteEditor.Services;

internal class ImageServices : IImageServices
{
    public Image MovePixels(Vector displacement, Image input)
    {
        var newPixels = new uint[input.Pixels.Length];

        for (int x = 0; x < input.Width; ++x)
        {
            for (int y = 0; y < input.Height; ++y)
            {
                int i = x + y * input.Width;
                int i2 = Mod(x - displacement.X, input.Width) + Mod(y - displacement.Y, input.Height) * input.Width;

                newPixels[i] = input.Pixels[i2];
            }
        }

        return new(newPixels, input.Width, input.Height);
    }

    public unsafe void Save(string path, Image image)
    {
        var bitmap = new Bitmap(image.Width, image.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
        var data = bitmap.LockBits(new Rectangle(0, 0, image.Width, image.Height), System.Drawing.Imaging.ImageLockMode.WriteOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

        var sizeInBytes = 4 * image.Width*image.Height;
        fixed (uint* sourcePtr = &image.Pixels[0])
        {
            Buffer.MemoryCopy(sourcePtr, (void*)data.Scan0, sizeInBytes, sizeInBytes);
        }
        
        bitmap.UnlockBits(data);

        bitmap.Save(path);
    }
}
