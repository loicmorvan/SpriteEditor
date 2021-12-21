namespace SpriteEditor.Foundation
{
    internal static class MathHelper
    {
        public static int Mod(int value, int divisor)
        {
            return (value % divisor + divisor) % divisor;
        }
    }
}
