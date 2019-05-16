using destNotes.Model;
using destNotes.ViewModel.Interface;
using System.Collections.ObjectModel;

namespace destNotes.ViewModel
{
    public class NoteListViewModel
    {
        public ObservableCollection<Note> Notes { get; }

        private readonly INoteController _controller;

        public NoteListViewModel(INoteController controller)
        {
            _controller = controller;
            Notes = new ObservableCollection<Note>(controller.LoadNotes().GetAwaiter().GetResult());
        }
    }
}