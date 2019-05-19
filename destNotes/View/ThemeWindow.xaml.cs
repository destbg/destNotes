using System;
using destNotes.ViewModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using destNotes.Model;

namespace destNotes.View
{
    public partial class ThemeWindow : Window
    {
        private readonly SettingsViewModel _settingsViewModel;
        private readonly ThemeViewModel _themeViewModel;

        public event EventHandler UpdateTheme;

        public ThemeWindow(SettingsViewModel settingsViewModel, Theme theme)
        {
            InitializeComponent();
            _settingsViewModel = settingsViewModel;
            _themeViewModel = new ThemeViewModel(theme);
            DataContext = _themeViewModel;
        }

        private void MoveWindow(object sender, MouseButtonEventArgs e) =>
            this.DragMove();

        private void SaveTheme(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NameBox.Text))
                return;
            _themeViewModel.Theme.Name = NameBox.Text;
            var bg = BackgroundColor.SelectedColor.Value;
            _themeViewModel.Theme.Background = new SolidColorBrush(Color.FromRgb(bg.R, bg.G, bg.B));
            var fg = ForegroundColor.SelectedColor.Value;
            _themeViewModel.Theme.Foreground = new SolidColorBrush(Color.FromRgb(fg.R, fg.G, fg.B));
            var ho = HoverColor.SelectedColor.Value;
            _themeViewModel.Theme.Hover = new SolidColorBrush(Color.FromRgb(ho.R, ho.G, ho.B));
            _themeViewModel.Theme.DarkIcons = !GetLuminance(bg);
            var themes = _settingsViewModel.Themes;
            var found = false;
            for (var i = 0; i < themes.Count; i++)
            {
                if (themes[i].Id != _themeViewModel.Theme.Id) continue;
                themes[i] = _themeViewModel.Theme;
                found = true;
                break;
            }
            if (!found)
                _settingsViewModel.Themes.Add(_themeViewModel.Theme);
            _settingsViewModel.SaveThemes();
            UpdateTheme?.Invoke(null, null);
            this.Close();
        }

        private static bool GetLuminance(Color color)
        {
            var luminance = 0.2126 * color.R + 0.7152 * color.G + 0.0722 * color.B;
            return luminance > 127.5;
        }
    }
}
