using System;
using System.Windows.Input;

namespace SpriteEditor.Foundation
{
    internal class VoidCommand : ICommand
    {
        public event EventHandler? CanExecuteChanged
        {
            add { }
            remove { }
        }

        public bool CanExecute(object? parameter) => false;

        public void Execute(object? parameter) { }
    }
}
