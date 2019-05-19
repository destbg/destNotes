using destNotes.Model;
using destNotes.ViewModel.Annotations;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace destNotes.ViewModel
{
    public class ThemeViewModel : INotifyPropertyChanged
    {
        public Theme Theme { get; }

        public ThemeViewModel(Theme theme)
        {
            Theme = theme;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) => 
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}