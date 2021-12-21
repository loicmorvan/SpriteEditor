using ReactiveUI;
using SpriteEditor.Services;
using System;
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

            writeableBitmap = new WriteableBitmap(new BitmapImage(new Uri(path)));

            Image = writeableBitmap;
            this.imageServices=imageServices;
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
