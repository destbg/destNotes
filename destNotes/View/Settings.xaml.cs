using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using destNotes.Model;
using destNotes.ViewModel;
using destNotes.ViewModel.Interface;

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

            if (_settingsViewModel.Setting.Theme == Theme.Dark)
                DarkTheme.IsChecked = true;
            else LightTheme.IsChecked = true;
        }

        private void MoveWindow(object sender, MouseButtonEventArgs e) =>
            Application.Current.MainWindow?.DragMove();

        private void CloseApplication(object sender, RoutedEventArgs e) =>
            Application.Current.MainWindow?.Hide();

        private void ChangeTheme(object sender, RoutedEventArgs e)
        {
            if (!(sender is RadioButton button)) return;
            var dirs = Application.Current.Resources.MergedDictionaries;
            dirs.RemoveAt(dirs.Count - 1);
            switch (button.Name)
            {
                case "LightTheme":
                    dirs.Add(new ResourceDictionary
                    {
                        Source = new Uri("/destNotes;component/View/Theme/LightTheme.xaml", UriKind.Relative)
                    });
                    _settingsViewModel.Setting.Theme = Theme.Light;
                    break;
                case "DarkTheme":
                    dirs.Add(new ResourceDictionary
                    {
                        Source = new Uri("/destNotes;component/View/Theme/DarkTheme.xaml", UriKind.Relative)
                    });
                    _settingsViewModel.Setting.Theme = Theme.Dark;
                    break;
            }

            _settingsViewModel.SaveSettings();
        }
    }
}