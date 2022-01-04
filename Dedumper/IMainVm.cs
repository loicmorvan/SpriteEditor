using System.Windows.Input;
using System.Windows.Media;

namespace Dedumper
{
    internal interface IMainVm
    {
        ICommand OpenFile { get; }

        ImageSource Image { get; }

        int Width { get; set; }
    }
}
