using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using SpriteEditor.Services;
using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Image = SpriteEditor.Services.Image;

namespace SpriteEditor.ViewModels
{
    internal class FrameVm : ReactiveObject, IFrameVm, IDisposable
    {
        private readonly CompositeDisposable disposable;

        private readonly ObservableAsPropertyHelper<ImageSource> imageProperty;

        public unsafe FrameVm(string path)
        {
            disposable = new CompositeDisposable();

            MoveLeft = ReactiveCommand
                .Create(() => MovePixelsHandler(new Vector(-1, 0)))
                .DisposeWith(disposable);

            MoveRight = ReactiveCommand
                .Create(() => MovePixelsHandler(new Vector(1, 0)))
                .DisposeWith(disposable);

            MoveUp = ReactiveCommand
                .Create(() => MovePixelsHandler(new Vector(0, -1)))
                .DisposeWith(disposable);

            MoveDown = ReactiveCommand
                .Create(() => MovePixelsHandler(new Vector(0, 1)))
                .DisposeWith(disposable);

            imageProperty = this
                .WhenAnyValue(x => x.InternalImage)
                .Select(x =>
                {
                    if (x == null)
                    {
                        return ImageSourceEx.CreateDefault();
                    }

                    var writeableBitmap = new WriteableBitmap(x.Width, x.Height, 96, 96, PixelFormats.Bgra32, null);
                    writeableBitmap.WritePixels(new System.Windows.Int32Rect(0, 0, x.Width, x.Height), x.Pixels, 4 * x.Width, 0, 0);
                    return writeableBitmap;
                })
                .ToProperty(this, x => x.Image, ImageSourceEx.CreateDefault())
                .DisposeWith(disposable);

            InternalImage = new Image(path);

            Save = ReactiveCommand
                .Create(() => InternalImage.Save(path))
                .DisposeWith(disposable);
        }

        [Reactive]
        private Image InternalImage { get; set; }

        private void MovePixelsHandler(Vector vector)
        {
            InternalImage = InternalImage.MovePixels(vector);
        }

        public void Dispose()
        {
            disposable.Dispose();
        }

        public ImageSource Image => imageProperty.Value;

        public ICommand MoveLeft { get; }

        public ICommand MoveRight { get; }

        public ICommand MoveUp { get; }

        public ICommand MoveDown { get; }

        public ICommand Save { get; }
    }
}
