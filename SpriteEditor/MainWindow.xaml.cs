using SpriteEditor.ViewModels;
using System.Windows;

namespace SpriteEditor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            DataContext = new MainVm();
        }
    }
}
