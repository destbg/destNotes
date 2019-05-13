using destNotes.Model;
using System.Collections.ObjectModel;

namespace destNotes.ViewModel
{
    public class NoteListViewModel
    {
        public ObservableCollection<Note> Notes { get; }

        private readonly DbController _controller;

        public NoteListViewModel(DbController controller)
        {
            _controller = controller;
            Notes = new ObservableCollection<Note>(controller.LoadNotes().GetAwaiter().GetResult());
        }
    }
}