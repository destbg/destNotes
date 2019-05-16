using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;

namespace destNotes.ViewModel
{
    public class NoteController
    {
        private bool _isBold;
        private bool _isItalic;
        private bool _isUnderline;
        private bool _isStrike;
        private bool _isBullets;
        private bool _canSave;
        private readonly string _id;
        private readonly RichTextBox _textBox;

        public NoteController(string id, RichTextBox textBox)
        {
            _id = id;
            _textBox = textBox;
        }

        public bool ValidateKeyDownEvent(string key, KeyEventArgs e)
        {
            if (char.TryParse(key, out var c) && char.IsLetterOrDigit(c) &&
                (Keyboard.GetKeyStates(Key.LeftCtrl) | Keyboard.GetKeyStates(Key.RightCtrl)
                 & KeyStates.Down) != KeyStates.Down) return false;

            if (!_isBullets || e.Key != Key.Enter) return true;

            var runE = new Run(string.Empty, _textBox.CaretPosition)
            {
                Text = "\n• "
            };
            e.Handled = true;
            _textBox.CaretPosition = runE.ContentEnd.GetInsertionPosition(LogicalDirection.Forward);
            SaveRichTextBox();
            return true;
        }

        public void SetRunSettings(string key)
        {
            var run = new Run(string.Empty, _textBox.CaretPosition);

            if (!_textBox.Selection.IsEmpty)
                _textBox.Selection.Text = string.Empty;

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

            run.Text = Keyboard.GetKeyStates(Key.CapsLock) == KeyStates.None
                ? shiftToggle ? key : key.ToLower()
                : shiftToggle
                    ? key.ToLower()
                    : key;

            _textBox.CaretPosition = run.ContentEnd.GetInsertionPosition(LogicalDirection.Forward);
            SaveRichTextBox();
        }

        private void SaveRichTextBox()
        {
            if (!_canSave)
                _canSave = true;
        }

        public void LoadRichTextBox()
        {
            if (!File.Exists($"{_id}.xaml")) return;
            var range = new TextRange(_textBox.Document.ContentStart, _textBox.Document.ContentEnd);
            var fStream = new FileStream($"{_id}.xaml", FileMode.OpenOrCreate);
            range.Load(fStream, DataFormats.XamlPackage);
            fStream.Close();
        }

        public Task SaveNoteText(bool closeNote)
        {
            if (!closeNote && !_canSave) return Task.CompletedTask;
            var range = new TextRange(_textBox.Document.ContentStart, _textBox.Document.ContentEnd);
            var fStream = new FileStream($"{_id}.xaml", FileMode.Create);
            range.Save(fStream, DataFormats.XamlPackage);
            fStream.Close();
            _canSave = false;
            return Task.CompletedTask;
        }

        public void ChangeTextSettings(string tag, ToggleButton strikeButton, ToggleButton underlineButton, bool check)
        {
            switch (tag)
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
                        strikeButton.IsChecked = false;
                    }
                    break;

                case "StrikeToggle":
                    _isStrike = check;
                    if (check)
                    {
                        _isUnderline = false;
                        underlineButton.IsChecked = false;
                    }
                    break;

                case "BulletsToggle":
                    _isBullets = check;
                    break;
            }
        }
    }
}