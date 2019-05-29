using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace destNotes.View
{
    public partial class TaskList : UserControl
    {
        public TaskList()
        {
            InitializeComponent();
        }

        private void MoveWindow(object sender, MouseButtonEventArgs e) => 
            Application.Current.MainWindow?.DragMove();

        private void CloseApplication(object sender, RoutedEventArgs e) => 
            Application.Current.MainWindow?.Hide();

        private void FrameworkElement_OnLoaded(object sender, RoutedEventArgs e)
        {
            var textBlock = (TextBlock)sender;
            var color = ((SolidColorBrush)textBlock.Foreground).Color;
            var foreground = 0.2126 * color.R + 0.7152 * color.G + 0.0722 * color.B;
            color = ((SolidColorBrush)((Grid)textBlock.Parent).Background).Color;
            var background = 0.2126 * color.R + 0.7152 * color.G + 0.0722 * color.B;
            if (Math.Abs(background - foreground) > 50) return;
            textBlock.Background = foreground > 127.5 ?
                new SolidColorBrush(Colors.Black) :
                new SolidColorBrush(Colors.White);
        }
    }
}