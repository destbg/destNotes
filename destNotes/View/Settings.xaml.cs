using destNotes.ViewModel;
using destNotes.ViewModel.Interface;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using destNotes.Model;

namespace destNotes.View
{
    public partial class Settings : UserControl
    {
        private readonly SettingsViewModel _settingsViewModel;

        public Settings(ISettingsController controller)
        {
            InitializeComponent();
            _settingsViewModel = new SettingsViewModel(controller);
            DataContext = _settingsViewModel;
        }

        private void MoveWindow(object sender, MouseButtonEventArgs e) =>
            Application.Current.MainWindow?.DragMove();

        private void CloseApplication(object sender, RoutedEventArgs e) =>
            Application.Current.MainWindow?.Hide();

        private void ChangeTheme(object sender, RoutedEventArgs e)
        {
            if (!(sender is RadioButton button)) return;
            var name = button.Content.ToString();
            var theme = _settingsViewModel.Themes.First(f => f.Name == name);
            UpdateTheme(theme);
        }

        private void AddTheme(object sender, RoutedEventArgs e)
        {
            var theme = new ThemeWindow(_settingsViewModel, new Theme
            {
                Id = Guid.NewGuid().ToString("N")
            });
            theme.UpdateTheme += Theme_UpdateTheme;
            theme.Show();
        }

        private void DeleteTheme(object sender, RoutedEventArgs e)
        {
            var tag = ((sender as Button).Parent as StackPanel).Tag.ToString();
            _settingsViewModel.Themes.Remove(_settingsViewModel.Themes.First(f => f.Id == tag));
            _settingsViewModel.SaveThemes();
        }

        private void ChangeThemeAppearance(object sender, RoutedEventArgs e)
        {
            var tag = ((sender as Button).Parent as StackPanel).Tag.ToString();
            var theme = new ThemeWindow(_settingsViewModel, _settingsViewModel.Themes.First(f => f.Id == tag));
            theme.UpdateTheme += Theme_UpdateTheme;
            theme.Show();
        }

        private void Theme_UpdateTheme(object sender, EventArgs e)
        {
            var id = _settingsViewModel.Setting.Theme;
            UpdateTheme(_settingsViewModel.Themes.First(f => f.Id == id));
        }

        private void UpdateTheme(Theme theme)
        {
            var dirs = Application.Current.Resources.MergedDictionaries;
            dirs.RemoveAt(dirs.Count - 1);
            dirs.RemoveAt(dirs.Count - 1);
            dirs.Add(theme.DarkIcons ? new ResourceDictionary
            {
                Source = new Uri("/destNotes;component/View/Theme/DarkIcons.xaml", UriKind.Relative)
            } : new ResourceDictionary
            {
                Source = new Uri("/destNotes;component/View/Theme/LightIcons.xaml", UriKind.Relative)
            });
            dirs.Add(new ResourceDictionary
            {
                {"BackgroundColor", theme.Background},
                {"ForegroundColor", theme.Foreground},
                {"HoverColor", theme.Hover}
            });
            _settingsViewModel.Setting.Theme = theme.Id;
            _settingsViewModel.SaveSetting();
        }
    }
}