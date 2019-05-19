using System.Windows;
using System.Windows.Media;

namespace destNotes.Model
{
    public class Theme
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public SolidColorBrush Foreground { get; set; }
        public SolidColorBrush Background { get; set; }
        public SolidColorBrush Hover { get; set; }
        public bool DarkIcons { get; set; }
        public Visibility IsDefault => 
            Name == "Dark" || Name == "Light" 
                ? Visibility.Hidden 
                : Visibility.Visible;
    }
}