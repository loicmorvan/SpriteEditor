using System;
using System.Windows;
using System.Windows.Controls;

namespace SpriteCutter
{
    /// <summary>
    /// Interaction logic for Grid2.xaml
    /// </summary>
    public partial class Grid2 : UserControl
    {
        public Grid2()
        {
            InitializeComponent();
        }

        public int ColumnCount
        {
            get { return (int)GetValue(ColumnCountProperty); }
            set { SetValue(ColumnCountProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ColumnCount.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ColumnCountProperty =
            DependencyProperty.Register(nameof(ColumnCount), typeof(int), typeof(Grid2), new PropertyMetadata(2, OnColumnCountChanged));

        private static void OnColumnCountChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Grid2 @this = (Grid2)d;
            if (@this != null)
            {
                @this.grid.ColumnDefinitions.Clear();
                for (int i = 0; i < (int)e.NewValue; ++i)
                {
                    @this.grid.ColumnDefinitions.Add(new ColumnDefinition());
                }
            }
        }

        public int RowCount
        {
            get { return (int)GetValue(RowCountProperty); }
            set { SetValue(RowCountProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RowCount.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RowCountProperty =
            DependencyProperty.Register(nameof(RowCount), typeof(int), typeof(Grid2), new PropertyMetadata(2, OnRowCountChanged));

        private static void OnRowCountChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Grid2 @this = (Grid2)d;
            if (@this != null)
            {
                @this.grid.RowDefinitions.Clear();
                for (int i = 0; i < (int)e.NewValue; ++i)
                {
                    @this.grid.RowDefinitions.Add(new RowDefinition());
                }
            }
        }
    }
}
