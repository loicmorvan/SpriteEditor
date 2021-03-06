using System.Collections.ObjectModel;
using System.Windows.Input;

namespace SpriteEditor.ViewModels
{
    internal interface IMainVm
    {
        public ICommand OpenFrames { get; }

        ReadOnlyObservableCollection<IFrameVm> Frames { get; }

        IFrameVm CurrentFrame { get; set; }

        IFrameVm? TransparencyFrame { get; }

        bool DisplayTransparencyFrame { get; set; }

        IFrameVm? AnimationFrame { get; }

        ICommand SaveAll { get; }
    }
}
