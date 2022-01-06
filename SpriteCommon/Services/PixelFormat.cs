namespace SpriteEditor.Services
{
    public record PixelFormat(int SizeInBytes, Channel R, Channel G, Channel B, Channel? A = null);
}