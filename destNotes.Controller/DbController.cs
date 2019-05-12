using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using destNotes.Data;
using destNotes.Model;

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
            return await _manager.LoadData<Note>("Notes");
        }

        public async Task AddNote(Note note)
        {
            await _manager.AddData("Notes", note);
        }

        public async Task SaveNote(Note note)
        {
            await _manager.OverrideData("Notes", note, note.Id);
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