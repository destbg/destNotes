using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using destNotes.ViewModel;

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

        public NoteWindow(DbController controller, string id)
        {
            InitializeComponent();
            _noteViewModel = new NoteViewModel(controller, id);
            DataContext = _noteViewModel;

            MultiText.SetValue(Block.LineHeightProperty, 0.1);
            MultiText.Document.Blocks.Clear();
            LoadRichTextBox(_noteViewModel.Note.Id);

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
                    SaveRichTextBox(_noteViewModel.Note.Id);
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
            SaveRichTextBox(_noteViewModel.Note.Id);
        }

        private async void WindowGotFocus(object sender, RoutedEventArgs e)
        {
            FooterPanel.Visibility = Visibility.Visible;
            for (var i = HeaderPanel.Margin.Top; i < 0; i += 3)
            {
                HeaderPanel.Margin = new Thickness(0, i, 0, -i);
                await Task.Delay(1);
            }
        }

        private void WindowLostFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            HeaderPanel.Margin = new Thickness(0, -30, 0, 30);
            FooterPanel.Visibility = Visibility.Collapsed;
        }

        private void CloseNote(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void SaveRichTextBox(string id)
        {
            var range = new TextRange(MultiText.Document.ContentStart, MultiText.Document.ContentEnd);
            var fStream = new FileStream($"{id}.xaml", FileMode.Create);
            range.Save(fStream, DataFormats.XamlPackage);
            fStream.Close();
        }

        private void LoadRichTextBox(string id)
        {
            if (!File.Exists($"{id}.xaml")) return;
            var range = new TextRange(MultiText.Document.ContentStart, MultiText.Document.ContentEnd);
            var fStream = new FileStream($"{id}.xaml", FileMode.OpenOrCreate);
            range.Load(fStream, DataFormats.XamlPackage);
            fStream.Close();
        }
    }
}
