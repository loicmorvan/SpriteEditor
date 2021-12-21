using DynamicData;
using Microsoft.Win32;
using ReactiveUI;
using SpriteEditor.Services;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace SpriteEditor.ViewModels
{
    internal class MainVm : IMainVm
    {
        private readonly ObservableCollection<IFrameVm> frames = new();

        private IFrameVm currentFrame = new DefaultFrameVm();

        public MainVm()
        {
            OpenFrames = ReactiveCommand.Create(OpenFramesHandlerAsync);
        }

        public ICommand OpenFrames { get; }

        public ObservableCollection<IFrameVm> Frames => frames;

        public IFrameVm CurrentFrame
        {
            get => currentFrame;
            set
            {
                currentFrame = value;
                TransparencyFrame = frames[(frames.IndexOf(value) + 1) % frames.Count];
            }
        }

        public IFrameVm? TransparencyFrame { get; private set; }

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
}
