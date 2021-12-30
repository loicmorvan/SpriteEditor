﻿using Microsoft.Win32;
using ReactiveUI;
using System.Windows.Input;

namespace SpriteCutter
{
    internal class MainVm : IMainVm
    {
        public MainVm()
        {
            OpenImage = ReactiveCommand.Create(OpenImageHandler);
            Image = new ImageVm();
        }

        public ICommand OpenImage { get; }

        public IImageVm Image { get; private set; }

        private void OpenImageHandler()
        {
            var window = new OpenFileDialog
            {
                Filter = "Images|*.png",
                Multiselect = false,
            };

            if (window.ShowDialog() == true)
            {
                Image = new ImageVm(window.FileName);
            }
        }
    }
}
