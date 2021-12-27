using DynamicData;
using Microsoft.Win32;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows.Input;

namespace SpriteEditor.ViewModels;

internal class MainVm : ReactiveObject, IMainVm, IDisposable
{
    private readonly CompositeDisposable disposable = new();
    private readonly SourceList<IFrameVm> frames = new();

    private readonly ObservableAsPropertyHelper<IFrameVm?> transparencyFrameProperty;
    private readonly ObservableAsPropertyHelper<IFrameVm?> animationFrameProperty;
    private readonly ReadOnlyObservableCollection<IFrameVm> observable;

    public MainVm()
    {
        OpenFrames = ReactiveCommand
            .Create(OpenFramesHandler)
            .DisposeWith(disposable);

        frames.Connect()
              .DisposeMany()
              .Bind(out observable)
              .Subscribe()
              .DisposeWith(disposable);

        transparencyFrameProperty =
            this.WhenAnyValue(x => x.CurrentFrame, x => x.DisplayTransparencyFrame)
                .Select(x => x.Item2 ? NextFrame(x.Item1) : null)
                .ToProperty(this, x => x.TransparencyFrame, (IFrameVm?)null)
                .DisposeWith(disposable);

        animationFrameProperty = Observable.Timer(DateTimeOffset.Now, TimeSpan.FromMilliseconds(32))
            .Select(_ => NextFrame(AnimationFrame))
            .ToProperty(this, x => x.AnimationFrame)
            .DisposeWith(disposable);

        SaveAll = ReactiveCommand
            .Create(() =>
            {
                foreach (var command in observable.Select(x => x.Save))
                {
                    if (command.CanExecute(null))
                    {
                        command.Execute(null);
                    }
                }
            })
            .DisposeWith(disposable);
    }

    private IFrameVm? NextFrame(IFrameVm? frame)
    {
        if (observable.Count == 0)
        {
            return null;
        }

        if (frame == null)
        {
            return observable[0];
        }

        return observable[(observable.IndexOf(frame) + 1) % observable.Count];
    }

    public ICommand OpenFrames { get; }

    public ReadOnlyObservableCollection<IFrameVm> Frames => observable;

    [Reactive]
    public IFrameVm CurrentFrame { get; set; } = new DefaultFrameVm();

    public IFrameVm? TransparencyFrame => transparencyFrameProperty.Value;

    [Reactive]
    public bool DisplayTransparencyFrame { get; set; }

    public IFrameVm? AnimationFrame => animationFrameProperty.Value;

    public ICommand SaveAll { get; }

    public void Dispose()
    {
        disposable.Dispose();
    }

    private void OpenFramesHandler()
    {
        var window = new OpenFileDialog
        {
            Filter = "Images|*.png",
            Multiselect = true,
        };

        if (window.ShowDialog() == true)
        {
            frames.Clear();
            frames.AddRange(window.FileNames.Select(x => new FrameVm(x)));
        }
    }
}
