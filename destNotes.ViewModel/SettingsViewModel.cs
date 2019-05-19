using destNotes.Model;
using destNotes.ViewModel.Interface;
using System;
using System.Collections.ObjectModel;
using System.Windows.Media;

namespace destNotes.ViewModel
{
    public class SettingsViewModel
    {
        public ObservableCollection<Theme> Themes { get; }
        public Setting Setting { get; }

        private readonly ISettingsController _controller;

        public SettingsViewModel(ISettingsController controller)
        {
            _controller = controller;
            Setting = _controller.LoadSettings().GetAwaiter().GetResult() ?? new Setting();
            Themes = new ObservableCollection<Theme>(_controller.LoadThemes().GetAwaiter().GetResult());
            var hasDark = false;
            var hasLight = false;
            foreach (var theme in Themes)
                switch (theme.Name)
                {
                    case "Dark":
                        hasDark = true;
                        break;
                    case "Light":
                        hasLight = true;
                        break;
                }

            if (!hasDark)
                Themes.Add(new Theme
                {
                    Id = Guid.NewGuid().ToString("N"),
                    Name = "Dark",
                    Hover = new SolidColorBrush(Color.FromRgb(76, 76, 76)),
                    Background = new SolidColorBrush(Color.FromRgb(24, 26, 27)),
                    Foreground = new SolidColorBrush(Color.FromRgb(232, 230, 227)),
                    DarkIcons = true
                });
            if (!hasLight)
                Themes.Add(new Theme
                {
                    Id = Guid.NewGuid().ToString("N"),
                    Name = "Light",
                    Hover = new SolidColorBrush(Color.FromRgb(76, 76, 76)),
                    Background = new SolidColorBrush(Color.FromRgb(248, 248, 255)),
                    Foreground = new SolidColorBrush(Color.FromRgb(0, 0, 0)),
                    DarkIcons = false
                });
            if (!hasLight || !hasDark)
                SaveThemes();
        }

        public async void SaveThemes() =>
            await _controller.SaveThemes(Themes);

        public async void SaveSetting() => 
            await _controller.SaveSettings(Setting);
    }
}