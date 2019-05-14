using destNotes.Data;
using destNotes.Model;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Media;

namespace destNotes.ViewModel
{
    public class DbController
    {
        private readonly DbManager _manager;

        public DbController()
        {
            _manager = new DbManager();
        }

        public async Task<IEnumerable<Note>> LoadNotes()
        {
            return from note in await _manager.LoadData<NoteDb>("Notes")
                   let temp = note.Color.Split().Select(byte.Parse).ToArray()
                   select new Note
                   {
                       Edited = note.Edited,
                       Id = note.Id,
                       Color = new SolidColorBrush(Color.FromRgb(temp[0], temp[1], temp[2]))
                   };
        }

        public async Task<Note> LoadNote(string id)
        {
            var note = await _manager.LoadData<NoteDb>("Notes", id);
            var temp = note.Color.Split().Select(byte.Parse).ToArray();
            return new Note
            {
                Edited = note.Edited,
                Id = note.Id,
                Color = new SolidColorBrush(Color.FromRgb(temp[0], temp[1], temp[2]))
            };
        }

        public async Task AddNote(Note note)
        {
            await _manager.AddData("Notes",
                new NoteDb
                {
                    Color = $"{note.Color.Color.R} {note.Color.Color.G} {note.Color.Color.B}",
                    Id = note.Id,
                    Edited = note.Edited
                });
        }

        public async Task SaveNote(Note note)
        {
            await _manager.OverrideData("Notes",
                new NoteDb
                {
                    Color = $"{note.Color.Color.R} {note.Color.Color.G} {note.Color.Color.B}",
                    Id = note.Id,
                    Edited = note.Edited
                }, note.Id);
        }

        public async Task DeleteNote(string id)
        {
            await _manager.DeleteData("Notes", id);
        }

        public async Task<Setting> LoadSetting()
        {
            return (await _manager.LoadData<Setting>("Settings")).FirstOrDefault();
        }

        public async Task SaveSetting(Setting setting)
        {
            await _manager.TruncateData("Settings");
            await _manager.AddData("Settings", setting);
        }
    }
}