using System.Collections.Generic;
using System.Threading.Tasks;
using destNotes.Model;

namespace destNotes.ViewModel.Interface
{
    public interface INoteController
    {
        Task<IEnumerable<Note>> LoadNotes();

        Task<Note> LoadNote(string id);

        Task AddNote(Note note);

        Task SaveNote(Note note);

        Task DeleteNote(string id);
    }
}