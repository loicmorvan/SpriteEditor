using Microsoft.Win32;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using SpriteEditor.Services;
using SpriteEditor.ViewModels;
using System;
using System.IO;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;

namespace Dedumper
{
    internal class MainVm : ReactiveObject, IMainVm
    {
        private readonly CompositeDisposable disposable = new();
        private readonly ObservableAsPropertyHelper<Image?> internalImageProperty;
        private readonly ObservableAsPropertyHelper<int> heightProperty;
        private readonly ObservableAsPropertyHelper<ImageSource?> imageProperty;
        private int width = 50;

        public MainVm()
        {
            OpenFile = ReactiveCommand
                .Create(OpenFileHandler)
                .DisposeWith(disposable);

            internalImageProperty =
                this.WhenAnyValue(x => x.Content, x => x.Offset, x => x.Width, x => x.PixelFormat)
                    .Select(x =>
                    {
                        var (content, offset, width, pixelFormat) = x;

                        return CreateImage(content, offset, width, pixelFormat);
                    })
                    .ToProperty(this, x => x.InternalImage, SpriteEditor.Services.Image.CreateDefault())
                    .DisposeWith(disposable);

            heightProperty =
                this.WhenAnyValue(x => x.InternalImage)
                    .Select(x => x?.Height ?? 50)
                    .ToProperty(this, x => x.Height, 50)
                    .DisposeWith(disposable);

            imageProperty =
                this.WhenAnyValue(x => x.InternalImage, x => x.Zoom)
                    .Select(x =>
                    {
                        var (image, zoom) = x;

                        return image?.CreateSource(zoom);
                    })
                    .ToProperty(this, x => x.Image, ImageSourceEx.CreateDefault())
                    .DisposeWith(disposable);

            IncreaseWidth = ReactiveCommand.Create(() => Width += 1).DisposeWith(disposable);
            DecreaseWidth = ReactiveCommand.Create(() => Width -= 1).DisposeWith(disposable);
        }

        private static Image? CreateImage(byte[]? content, int offset, int width, SpriteEditor.Services.PixelFormat pixelFormat)
        {
            if (content == null)
            {
                return null;
            }

            var pixelSizeInBytes = SpriteEditor.Services.Image.GetSizeInBytes(pixelFormat);
            var potentialPixelCount = content.Length / pixelSizeInBytes;
            var height = Math.Min(potentialPixelCount / width, 500);
            var offsetInBytes = offset * pixelSizeInBytes;
            var sizeInBytes = pixelSizeInBytes * width * height;
            var pixels = new byte[sizeInBytes];
            Buffer.BlockCopy(content, offsetInBytes, pixels, 0, sizeInBytes);
            return new Image(pixels, width, height, pixelFormat);
        }

        private async Task OpenFileHandler()
        {
            var window = new OpenFileDialog
            {
                Filter = "All files|*.*",
                Multiselect = false,
            };

            if (window.ShowDialog() == true)
            {
                Content = await File.ReadAllBytesAsync(window.FileName);
            }
        }

        public ICommand OpenFile { get; }

        [Reactive]
        private byte[]? Content { get; set; }

        public ImageSource? Image => imageProperty.Value;

        public int Width
        {
            get => width;
            set
            {
                if (value > 4)
                {
                    this.RaiseAndSetIfChanged(ref width, value);
                }
            }
        }

        public ICommand IncreaseWidth { get; }

        public ICommand DecreaseWidth { get; }

        [Reactive]
        public decimal Zoom { get; set; } = 1;

        private Image? InternalImage => internalImageProperty.Value;

        [Reactive]
        public SpriteEditor.Services.PixelFormat PixelFormat { get; set; }

        [Reactive]
        public int Offset { get; set; }

        public int Height => heightProperty.Value;
    }
}
