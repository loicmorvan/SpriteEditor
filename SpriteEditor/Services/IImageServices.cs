namespace SpriteEditor.Services
{
    internal interface IImageServices
    {
        uint[] MovePixels(int pixelDisplacement, uint[] temp);
    }
}