using System.Windows.Input;

namespace SpriteCutter
{
    internal interface IMainVm
    {
        ICommand OpenImage { get; }
        IImageVm Image { get; }
    }
}