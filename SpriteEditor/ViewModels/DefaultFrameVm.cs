using SpriteEditor.Foundation;
using System.Windows.Input;
using System.Windows.Media;

namespace SpriteEditor.ViewModels
{
    internal class DefaultFrameVm : IFrameVm
    {
        public DefaultFrameVm()
        {
            MoveLeft = new VoidCommand();
            MoveRight = new VoidCommand();
            MoveUp = new VoidCommand();
            MoveDown = new VoidCommand();
            Save = new VoidCommand();

            Image = ImageSourceEx.CreateDefault();
        }

        public ImageSource Image { get; }

        public ICommand MoveLeft { get; }

        public ICommand MoveRight { get; }

        public ICommand MoveUp { get; }

        public ICommand MoveDown { get; }

        public ICommand Save { get; }
    }
}
