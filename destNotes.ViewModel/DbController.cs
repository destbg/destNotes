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

        #region Note

        public async Task<IEnumerable<Note>> LoadNotes()
        {
            return from note in await _manager.LoadData<NoteDb>("Notes")
                   select new Note
                   {
                       Edited = note.Edited,
                       Id = note.Id,
                       Color = GetColorFromString(note.Color)
                   };
        }

        public async Task<Note> LoadNote(string id)
        {
            var note = await _manager.LoadData<NoteDb>("Notes", id);
            return new Note
            {
                Edited = note.Edited,
                Id = note.Id,
                Color = GetColorFromString(note.Color)
            };
        }

        public async Task AddNote(Note note)
        {
            await _manager.AddData("Notes",
                new NoteDb
                {
                    Color = GetStringFromColor(note.Color.Color),
                    Id = note.Id,
                    Edited = note.Edited
                });
        }

        public async Task SaveNote(Note note)
        {
            await _manager.OverrideData("Notes",
                new NoteDb
                {
                    Color = GetStringFromColor(note.Color.Color),
                    Id = note.Id,
                    Edited = note.Edited
                }, note.Id);
        }

        public async Task DeleteNote(string id)
        {
            await _manager.DeleteData("Notes", id);
        }

        #endregion
        #region Settings

        public async Task<Setting> LoadSetting()
        {
            return (await _manager.LoadData<Setting>("Settings")).FirstOrDefault();
        }

        public async Task SaveSetting(Setting setting)
        {
            await _manager.TruncateData("Settings");
            await _manager.AddData("Settings", setting);
        }

        #endregion
        #region Tasks

        public async Task<IEnumerable<TaskModel>> LoadTasks()
        {
            var list = await _manager.LoadData<TaskText>("TasksList");
            return from task in await _manager.LoadData<TaskDb>("Tasks")
                   select new TaskModel
                   {
                       Id = task.Id,
                       Name = task.Name,
                       Color = GetColorFromString(task.Color),
                       List = list.Where(w => w.TaskId == task.Id).ToList()
                   };
        }

        public async Task<TaskModel> LoadTask(string id)
        {
            var task = await _manager.LoadData<TaskDb>("Tasks", id);
            return new TaskModel
            {
                Id = task.Id,
                Color = GetColorFromString(task.Color),
                Name = task.Name,
                List = (await _manager.LoadData<TaskText>("TasksList", "TaskId", id)).ToList()
            };
        }

        public async Task AddTask(TaskModel task)
        {
            await _manager.AddData("Tasks", new TaskDb
            {
                Id = task.Id,
                Color = GetStringFromColor(task.Color.Color),
                Name = task.Name
            });
            await _manager.AddListOfData("TasksList", task.List, "TaskId", task.Id);
        }

        public async Task SaveTask(TaskModel task)
        {
            await _manager.OverrideData("Tasks", new TaskDb
            {
                Id = task.Id,
                Color = GetStringFromColor(task.Color.Color),
                Name = task.Name
            }, task.Id);
            await _manager.AddListOfData("TasksList", task.List, "TaskId", task.Id);
        }

        public async Task DeleteTask(string id)
        {
            await _manager.DeleteData("Tasks", id);
        }

        public async Task DeleteTaskText(string id)
        {
            await _manager.DeleteData("TasksList", id);
        }

        #endregion

        private static SolidColorBrush GetColorFromString(string str)
        {
            var split = str.Split().Select(byte.Parse).ToArray();
            return new SolidColorBrush(Color.FromRgb(split[0], split[1], split[2]));
        }

        private static string GetStringFromColor(Color color)
        {
            return $"{color.R} {color.G} {color.B}";
        }
    }
}