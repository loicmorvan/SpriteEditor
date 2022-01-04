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
        private readonly ObservableAsPropertyHelper<ImageSource> imageProperty;
        private int width = 50;

        public MainVm()
        {
            OpenFile = ReactiveCommand
                .Create(OpenFileHandler)
                .DisposeWith(disposable);

            imageProperty =
                this.WhenAnyValue(x => x.Content, x => x.Width)
                    .Select(x =>
                    {
                        var (content, width) = x;

                        if (content == null)
                        {
                            return ImageSourceEx.CreateDefault();
                        }

                        var potentialPixelCount = content.Length / 4;
                        var height = potentialPixelCount / width;
                        var pixels = new uint[width * height];
                        Buffer.BlockCopy(content, 0, pixels, 0, 4 * width * height);
                        var image = new Image(pixels, width, height);
                        return image.CreateSource();
                    })
                    .ToProperty(this, x => x.Image, ImageSourceEx.CreateDefault())
                    .DisposeWith(disposable);
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

        public ImageSource Image => imageProperty.Value;

        public int Width
        {
            get => width;
            set
            {
                if (value > 0)
                {
                    this.RaiseAndSetIfChanged(ref width, value);
                }
            }
        }
    }
}
