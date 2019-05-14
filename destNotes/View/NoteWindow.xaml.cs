using destNotes.ViewModel;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using destNotes.ViewModel.Input;

namespace destNotes.View
{
    public partial class NoteWindow : Window
    {
        private readonly NoteViewModel _noteViewModel;
        private readonly NoteController _note;
        private bool _isFocused;

        public NoteWindow(DbController controller, string id)
        {
            InitializeComponent();
            _note = new NoteController(id, MultiText);
            _noteViewModel = new NoteViewModel(controller, id);
            DataContext = _noteViewModel;

            SetLuminance(_noteViewModel.Note.Color.Color);

            MultiText.SetValue(Block.LineHeightProperty, 0.1);
            MultiText.Document.Blocks.Clear();
            _note.LoadRichTextBox();

            this.ShowInTaskbar = false;
        }

        private void MoveWindow(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void ChangeText(object sender, RoutedEventArgs e)
        {
            var button = sender as ToggleButton;
            if (button?.IsChecked == null) return;
            var check = button.IsChecked.Value;

            _note.ChangeTextSettings(button.Name, StrikeToggle, UnderlineToggle, check);

            MultiText.Focus();
        }

        private void MultiTextKeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                var key = e.Key.ToString();
                if (_note.ValidateKeyDownEvent(key, e))
                    return;

                e.Handled = true;

                _note.SetRunSettings(key);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Something went wrong. \nError message: " + ex.Message);
            }
        }

        private async void WindowGotFocus(object sender, RoutedEventArgs e)
        {
            _isFocused = true;
            FooterPanel.Visibility = Visibility.Visible;
            for (var i = HeaderPanel.Margin.Top; i < 0; i += 3)
            {
                HeaderPanel.Margin = new Thickness(0, i, 0, -i);
                await Task.Delay(1);
            }
        }

        private async void WindowLostFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            await Task.Delay(5);
            if (!_isFocused)
            {
                HeaderPanel.Margin = new Thickness(0, -30, 0, 30);
                FooterPanel.Visibility = Visibility.Collapsed;
            }
            _isFocused = false;
        }

        private async void CloseNote(object sender, RoutedEventArgs e)
        {
            await _note.SaveNoteText(true);
            this.Close();
        }

        private void NoteWindowSizeChanged(object sender, SizeChangedEventArgs e)
        {
            MultiText.Height = this.Height - 90;
            MultiText.Width = this.Width - 10;
        }

        private async void LoopSave(object sender, RoutedEventArgs e)
        {
            while (true)
            {
                await Task.Delay(5000);
                await _note.SaveNoteText(false);
            }
        }

        private void ShowOptions(object sender, RoutedEventArgs e)
        {
            OptionsGrid.Visibility = Visibility.Visible;
        }

        private void HideSettings(object sender, MouseButtonEventArgs e)
        {
            if (ColorCanvas.SelectedColor.HasValue)
            {
                var color = ColorCanvas.SelectedColor.Value;
                var note = _noteViewModel.Note;
                note.Color = new SolidColorBrush(Color.FromRgb(color.R, color.G, color.B));
                _noteViewModel.SaveNote();
                SetLuminance(color);
            }
            OptionsGrid.Visibility = Visibility.Hidden;
        }

        private void SetLuminance(Color color)
        {
            var luminance = 0.2126 * color.R + 0.7152 * color.G + 0.0722 * color.B;
            if (luminance > 127.5)
            {
                AddNoteImage.Source = new BitmapImage(new Uri(@"/destNotes;component/Assets/plus-dark.png", UriKind.Relative));
                CloseImage.Source = new BitmapImage(new Uri(@"/destNotes;component/Assets/window-close-dark.png", UriKind.Relative));
                OptionsImage.Source = new BitmapImage(new Uri(@"/destNotes;component/Assets/dots-horizontal-dark.png", UriKind.Relative));
            }
            else
            {
                AddNoteImage.Source = new BitmapImage(new Uri(@"/destNotes;component/Assets/plus.png", UriKind.Relative));
                CloseImage.Source = new BitmapImage(new Uri(@"/destNotes;component/Assets/window-close.png", UriKind.Relative));
                OptionsImage.Source = new BitmapImage(new Uri(@"/destNotes;component/Assets/dots-horizontal.png", UriKind.Relative));
            }
        }
    }
}