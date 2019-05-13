using destNotes.ViewModel;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace destNotes.View
{
    public partial class NoteWindow : Window
    {
        private bool _isBold;
        private bool _isItalic;
        private bool _isUnderline;
        private bool _isStrike;
        private bool _isBullets;
        private readonly NoteViewModel _noteViewModel;
        private readonly string _id;
        private bool _canSave;
        private bool _isFocused;

        public NoteWindow(DbController controller, string id)
        {
            InitializeComponent();
            _noteViewModel = new NoteViewModel(controller, id);
            DataContext = _noteViewModel;
            _id = id;

            var color = _noteViewModel.Note.Color.Color;
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

            MultiText.SetValue(Block.LineHeightProperty, 0.1);
            MultiText.Document.Blocks.Clear();
            LoadRichTextBox();

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

            switch (button.Name)
            {
                case "BoldToggle":
                    _isBold = check;
                    break;

                case "ItalicToggle":
                    _isItalic = check;
                    break;

                case "UnderlineToggle":
                    _isUnderline = check;
                    if (check)
                    {
                        _isStrike = false;
                        StrikeToggle.IsChecked = false;
                    }
                    break;

                case "StrikeToggle":
                    _isStrike = check;
                    if (check)
                    {
                        _isUnderline = false;
                        UnderlineToggle.IsChecked = false;
                    }
                    break;

                case "BulletsToggle":
                    _isBullets = check;
                    break;
            }
            MultiText.Focus();
        }

        private void MultiTextKeyDown(object sender, KeyEventArgs e)
        {
            var key = e.Key.ToString();
            if (!char.TryParse(key, out var c) || !char.IsLetterOrDigit(c)
                || (Keyboard.GetKeyStates(Key.LeftCtrl) | Keyboard.GetKeyStates(Key.RightCtrl)
                    & KeyStates.Down) == KeyStates.Down)
            {
                if (_isBullets && e.Key == Key.Enter)
                {
                    var runE = new Run(string.Empty, MultiText.CaretPosition)
                    {
                        Text = "\n• "
                    };
                    e.Handled = true;
                    MultiText.CaretPosition = runE.ContentEnd.GetInsertionPosition(LogicalDirection.Forward);
                    SaveRichTextBox();
                }
                return;
            }
            e.Handled = true;

            var run = new Run(string.Empty, MultiText.CaretPosition);

            if (!MultiText.Selection.IsEmpty)
                MultiText.Selection.Text = string.Empty;

            if (_isBold)
                run.FontWeight = FontWeights.Bold;
            if (_isItalic)
                run.FontStyle = FontStyles.Italic;
            if (_isUnderline)
                run.TextDecorations = TextDecorations.Underline;
            else if (_isStrike)
                run.TextDecorations = TextDecorations.Strikethrough;

            var shiftToggle = (Keyboard.GetKeyStates(Key.LeftShift) |
                               Keyboard.GetKeyStates(Key.RightShift) & KeyStates.Toggled) ==
                              KeyStates.Toggled;

            run.Text = (Keyboard.GetKeyStates(Key.CapsLock) & KeyStates.Toggled) == KeyStates.Toggled
                ? shiftToggle ? key : key.ToLower()
                : shiftToggle ? key.ToLower() : key;

            MultiText.CaretPosition = run.ContentEnd.GetInsertionPosition(LogicalDirection.Forward);
            SaveRichTextBox();
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

        private void CloseNote(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void SaveRichTextBox()
        {
            if (!_canSave)
                _canSave = true;
        }

        private void LoadRichTextBox()
        {
            if (!File.Exists($"{_id}.xaml")) return;
            var range = new TextRange(MultiText.Document.ContentStart, MultiText.Document.ContentEnd);
            var fStream = new FileStream($"{_id}.xaml", FileMode.OpenOrCreate);
            range.Load(fStream, DataFormats.XamlPackage);
            fStream.Close();
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
                if (_canSave)
                    await SaveNoteText();
            }
        }

        private Task SaveNoteText()
        {
            var range = new TextRange(MultiText.Document.ContentStart, MultiText.Document.ContentEnd);
            var fStream = new FileStream($"{_id}.xaml", FileMode.Create);
            range.Save(fStream, DataFormats.XamlPackage);
            fStream.Close();
            _canSave = false;
            return Task.CompletedTask;
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
            OptionsGrid.Visibility = Visibility.Hidden;
        }
    }
}