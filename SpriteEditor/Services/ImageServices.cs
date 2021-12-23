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
}
