using DynamicData;
using Microsoft.Win32;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using SpriteEditor.Services;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Windows.Input;

namespace SpriteEditor.ViewModels;

internal class MainVm : ReactiveObject, IMainVm
{
    private readonly ObservableCollection<IFrameVm> frames = new();
    private readonly ObservableAsPropertyHelper<IFrameVm?> transparencyFrameProperty;

    public MainVm()
    {
        OpenFrames = ReactiveCommand.Create(OpenFramesHandlerAsync);

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
                .ToProperty(this, x => x.TransparencyFrame, (IFrameVm?)null);
    }

    public ICommand OpenFrames { get; }

    public ObservableCollection<IFrameVm> Frames => frames;

    [Reactive]
    public IFrameVm CurrentFrame { get; set; } = new DefaultFrameVm();

    public IFrameVm? TransparencyFrame => transparencyFrameProperty.Value;

    [Reactive]
    public bool DisplayTransparencyFrame { get; set; }

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
