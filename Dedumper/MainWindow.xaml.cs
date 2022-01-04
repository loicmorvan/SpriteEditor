using System;
using System.Windows;
using System.Windows.Input;

namespace Dedumper
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly IMainVm vm;

        public MainWindow()
        {
            InitializeComponent();

            vm = new MainVm();
            DataContext = vm;
        }
    }
}
