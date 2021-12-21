using ReactiveUI;
using SpriteEditor.Services;
using System;
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
            MoveLeft = ReactiveCommand.Create(() => MoveHorizontallyHandler(-1));
            MoveRight = ReactiveCommand.Create(() => MoveHorizontallyHandler(1));

            using var bmp = new Bitmap(path);
            writeableBitmap = new WriteableBitmap(bmp.Width, bmp.Height, 96, 96, PixelFormats.Bgra32, null);
            var data = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), System.Drawing.Imaging.ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            writeableBitmap.WritePixels(new System.Windows.Int32Rect(0, 0, bmp.Width, bmp.Height), data.Scan0, 4 * bmp.Width * bmp.Height, 4 * bmp.Width, 0, 0);

            Image = writeableBitmap;
            this.imageServices = imageServices;
        }

        private void MoveHorizontallyHandler(int pixelDisplacement)
        {
            var temp = new uint[writeableBitmap.PixelHeight * writeableBitmap.PixelWidth];
            writeableBitmap.CopyPixels(temp, 4 * writeableBitmap.PixelWidth, 0);

            temp = imageServices.MovePixels(pixelDisplacement, temp);

            writeableBitmap.WritePixels(
                new System.Windows.Int32Rect(0, 0, writeableBitmap.PixelWidth, writeableBitmap.PixelHeight),
                temp, 4 * writeableBitmap.PixelWidth, 0, 0);
        }

        public ImageSource Image { get; }

        public ICommand MoveLeft { get; }

        public ICommand MoveRight { get; }
    }
}
