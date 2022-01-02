using System.Windows.Media;

namespace SpriteCutter
{
    internal interface IImageVm
    {
        ImageSource Source { get; }

        ImageSource[] Animation { get; }

        int ColumnCount { get; }

        int RowCount { get; }

        ImageSource? AnimationFrame { get; }
    }
}