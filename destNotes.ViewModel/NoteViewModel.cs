using System.ComponentModel;
using System.Runtime.CompilerServices;
using destNotes.Model;
using destNotes.ViewModel.Annotations;

namespace destNotes.ViewModel
{
    public class NoteViewModel : INotifyPropertyChanged
    {
        private Note _note;

        public Note Note
        {
            get => _note;
            set
            {
                _note = value;
                OnPropertyChanged();
            }
        }

        private readonly DbController _controller;

        public NoteViewModel(DbController controller, string id)
        {
            _controller = controller;
            _note = _controller.LoadNote(id).GetAwaiter().GetResult();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}