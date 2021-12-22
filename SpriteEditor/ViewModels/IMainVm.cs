using System.Collections.ObjectModel;
using System.Windows.Input;

namespace SpriteEditor.ViewModels
{
    internal interface IMainVm
    {
        public ICommand OpenFrames { get; }

        ObservableCollection<IFrameVm> Frames { get; }

        IFrameVm CurrentFrame { get; set; }

        IFrameVm? TransparencyFrame { get; }

        bool DisplayTransparencyFrame { get; set; }
    }
}
