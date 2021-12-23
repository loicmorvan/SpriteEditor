using ReactiveUI;
using SpriteEditor.Services;
using System.Drawing;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace SpriteEditor.ViewModels
{
    internal class FrameVm : IFrameVm
    {
        private readonly WriteableBitmap writeableBitmap;
        private readonly IImageServices imageServices;

        public FrameVm(string path, IImageServices imageServices)
        {
            MoveLeft = ReactiveCommand.Create(() => MovePixelsHandler(new Vector(-1, 0)));
            MoveRight = ReactiveCommand.Create(() => MovePixelsHandler(new Vector(1, 0)));
            MoveUp = ReactiveCommand.Create(() => MovePixelsHandler(new Vector(0, -1)));
            MoveDown = ReactiveCommand.Create(() => MovePixelsHandler(new Vector(0, 1)));

            using var bmp = new Bitmap(path);
            writeableBitmap = new WriteableBitmap(bmp.Width, bmp.Height, 96, 96, PixelFormats.Bgra32, null);
            var data = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), System.Drawing.Imaging.ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            writeableBitmap.WritePixels(new System.Windows.Int32Rect(0, 0, bmp.Width, bmp.Height), data.Scan0, 4 * bmp.Width * bmp.Height, 4 * bmp.Width, 0, 0);

            Image = writeableBitmap;
            this.imageServices = imageServices;
        }

        private void MovePixelsHandler(Vector vector)
        {
            var temp = new uint[writeableBitmap.PixelHeight * writeableBitmap.PixelWidth];
            writeableBitmap.CopyPixels(temp, 4 * writeableBitmap.PixelWidth, 0);

            var output = imageServices.MovePixels(vector, new Services.Image(temp, writeableBitmap.PixelWidth, writeableBitmap.PixelHeight));

            writeableBitmap.WritePixels(
                new System.Windows.Int32Rect(0, 0, writeableBitmap.PixelWidth, writeableBitmap.PixelHeight),
                output.Pixels, 4 * writeableBitmap.PixelWidth, 0, 0);
        }

        public ImageSource Image { get; }

        public ICommand MoveLeft { get; }

        public ICommand MoveRight { get; }

        public ICommand MoveUp { get; }

        public ICommand MoveDown { get; }
    }
}
