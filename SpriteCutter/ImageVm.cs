using DynamicData;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using SpriteEditor.Services;
using SpriteEditor.ViewModels;
using System;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows.Media;

namespace SpriteCutter
{
    internal class ImageVm : ReactiveObject, IImageVm, IDisposable
    {
        private readonly CompositeDisposable disposable = new();
        private readonly ObservableAsPropertyHelper<ImageSource[]> animationProperty;
        private ObservableAsPropertyHelper<ImageSource?>? animationFrameProperty;
        private readonly ObservableAsPropertyHelper<ImageSource> sourceProperty;

        public ImageVm(string? safeFileName = null)
        {
            InternalImage = safeFileName == null
                ? new Image(Enumerable.Repeat((byte)0XFF, 16).ToArray(), 2, 2)
                : new Image(safeFileName);

            sourceProperty = this
                .WhenAnyValue(x => x.InternalImage)
                .Select(x => x.CreateSource())
                .ToProperty(this, x => x.Source, ImageSourceEx.CreateDefault())
                .DisposeWith(disposable);

            animationProperty = this.WhenAnyValue(x => x.InternalImage, x => x.ColumnCount, x => x.RowCount)
                .Select(x => x.Item1.Split(x.Item2, x.Item3).Select(x => x.CreateSource()).ToArray())
                .ToProperty(this, x => x.Animation)
                .DisposeWith(disposable);

            this.WhenAnyValue(x => x.AnimationFrameRate)
                .Do(animationFrameRate =>
                {
                    animationFrameProperty?.Dispose();
                    animationFrameProperty = Observable.Timer(DateTimeOffset.Now, TimeSpan.FromSeconds(1f / animationFrameRate))
                        .Select(_ => NextImage(AnimationFrame))
                        .ToProperty(this, x => x.AnimationFrame)
                        .DisposeWith(disposable);
                })
                .Subscribe()
                .DisposeWith(disposable);
        }

        private ImageSource? NextImage(ImageSource? image)
        {
            if (Animation == null || Animation.Length == 0)
            {
                return null;
            }

            if (image == null)
            {
                return Animation[0];
            }

            return Animation[(Animation.IndexOf(image) + 1) % Animation.Length];
        }

        public void Dispose()
        {
            disposable.Dispose();
        }

        [Reactive]
        private Image InternalImage { get; set; }

        public ImageSource Source => sourceProperty.Value;

        public ImageSource[] Animation => animationProperty.Value;

        public ImageSource? AnimationFrame => animationFrameProperty?.Value;

        [Reactive]
        public int ColumnCount { get; set; } = 1;

        [Reactive]
        public int RowCount { get; set; } = 1;

        [Reactive]
        public int AnimationFrameRate { get; set; } = 30;
    }
}