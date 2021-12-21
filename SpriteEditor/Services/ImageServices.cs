using static SpriteEditor.Foundation.MathHelper;

namespace SpriteEditor.Services;

internal class ImageServices : IImageServices
{
    public uint[] MovePixels(int pixelDisplacement, uint[] temp)
    {
        var newPixels = new uint[temp.Length];

        for (int i = 0; i < temp.Length; i++)
        {
            newPixels[i] = temp[Mod(i - pixelDisplacement, temp.Length)];
        }

        return newPixels;
    }
}
