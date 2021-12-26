using DynamicData;
using Microsoft.Win32;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using SpriteEditor.Services;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows.Input;

namespace SpriteEditor.ViewModels;

internal class MainVm : ReactiveObject, IMainVm, IDisposable
{
    private readonly ObservableCollection<IFrameVm> frames = new();
    private readonly ObservableAsPropertyHelper<IFrameVm?> transparencyFrameProperty;
    private readonly ObservableAsPropertyHelper<IFrameVm?> animationFrameProperty;
    private readonly CompositeDisposable disposable = new();

    public MainVm()
    {
        OpenFrames = ReactiveCommand.Create(OpenFramesHandlerAsync)
            .DisposeWith(disposable);

        frames.Add(CurrentFrame);

        transparencyFrameProperty =
            this.WhenAnyValue(x => x.CurrentFrame, x => x.DisplayTransparencyFrame)
                .Select(x =>
                {
                    var (frame, displayTransparencyFrame) = x;

                    if (frames.Count == 0 || !displayTransparencyFrame)
                    {
                        return null;
                    }

                    return frames[(frames.IndexOf(frame) + 1) % frames.Count];
                })
                .ToProperty(this, x => x.TransparencyFrame, (IFrameVm?)null)
                .DisposeWith(disposable);

        animationFrameProperty = Observable.Timer(DateTimeOffset.Now, TimeSpan.FromMilliseconds(32))
            .Select(_ => AnimationFrame)
            .Select(frame =>
            {
                if (frames.Count == 0)
                {
                    return null;
                }

                if (frame == null)
                {
                    return frames[0];
                }

                return frames[(frames.IndexOf(frame) + 1) % frames.Count];
            })
            .ToProperty(this, x => x.AnimationFrame)
            .DisposeWith(disposable);

        SaveAll = ReactiveCommand.Create(() =>
                    {
                        foreach (var frame in frames)
                        {
                            if (frame.Save.CanExecute(null))
                            {
                                frame.Save.Execute(null);
                            }
                        }
                    })
                    .DisposeWith(disposable);
    }

    public ICommand OpenFrames { get; }

    public ObservableCollection<IFrameVm> Frames => frames;

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

    private void OpenFramesHandlerAsync()
    {
        var window = new OpenFileDialog
        {
            Filter = "Images|*.png",
            Multiselect = true,
        };

        if (window.ShowDialog() == true)
        {
            frames.Clear();
            frames.AddRange(window.FileNames.Select(x => new FrameVm(x, new ImageServices())));
        }
    }
}
