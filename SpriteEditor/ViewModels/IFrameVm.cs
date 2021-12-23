using System.Windows.Input;
using System.Windows.Media;

namespace SpriteEditor.ViewModels
{
    public interface IFrameVm
    {
        ImageSource Image { get; }

        ICommand MoveLeft { get; }
        ICommand MoveRight { get; }
        ICommand MoveUp { get; }
        ICommand MoveDown { get; }
    }
}
