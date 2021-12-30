namespace SpriteCutter
{
    internal class ImageVm : IImageVm
    {
        public ImageVm() { }

        public ImageVm(string safeFileName)
        {
            FileName = safeFileName;
        }

        public string? FileName { get; } = null;
    }
}