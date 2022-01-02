using System.Windows.Media;

namespace SpriteCutter
{
    internal interface IImageVm
    {
        ImageSource Source { get; }

        ImageSource[] Animation { get; }

        int ColumnCount { get; }

        int RowCount { get; }

        int AnimationFrameRate { get; set; }

        ImageSource? AnimationFrame { get; }
    }
}