namespace SpriteEditor.Services
{
    internal interface IImageServices
    {
        Image MovePixels(Vector displacement, Image image);
    }
}