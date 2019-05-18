using destNotes.Model;
using destNotes.ViewModel.Annotations;
using destNotes.ViewModel.Interface;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace destNotes.ViewModel
{
    public class SettingsViewModel : INotifyPropertyChanged
    {
        private Setting _setting;

        public Setting Setting
        {
            get => _setting;
            set
            {
                _setting = value;
                OnPropertyChanged();
            }
        }

        private readonly ISettingsController _controller;

        public SettingsViewModel(ISettingsController controller)
        {
            _controller = controller;
            _setting = _controller.LoadSetting().GetAwaiter().GetResult() ?? new Setting();
        }

        public void SaveSettings()
        {
            _controller.SaveSetting(Setting);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) => 
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}