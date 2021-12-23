﻿using ReactiveUI;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace SpriteEditor.ViewModels
{
    internal class DefaultFrameVm : IFrameVm
    {
        public DefaultFrameVm()
        {
            MoveLeft = ReactiveCommand.Create(() => { });
            MoveRight = ReactiveCommand.Create(() => { });
            MoveUp = ReactiveCommand.Create(() => { });
            MoveDown = ReactiveCommand.Create(() => { });

            var writeableBitmap = new WriteableBitmap(2, 2, 96, 96, PixelFormats.Bgra32, null);
            writeableBitmap.WritePixels(
                new System.Windows.Int32Rect(0, 0, 2, 2),
                new uint[] { 0xFFFF0000, 0xFF00FF00, 0xFF0000FF, 0xFFFFFFFF },
                2 * sizeof(uint), 0, 0);

            Image = writeableBitmap;
        }

        public ImageSource Image { get; }

        public ICommand MoveLeft { get; }

        public ICommand MoveRight { get; }

        public ICommand MoveUp { get; }

        public ICommand MoveDown { get; }
    }
}
