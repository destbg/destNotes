using destNotes.Data;
using destNotes.Model;
using destNotes.ViewModel.Interface;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Media;

namespace destNotes.ViewModel
{
    public class DbController : INoteController, ISettingsController, ITaskController
    {
        private readonly DbManager _manager;

        public DbController()
        {
            _manager = new DbManager();
        }

        #region Note

        public async Task<IEnumerable<Note>> LoadNotes() =>
            from note in await _manager.LoadData<NoteDb>("Notes")
            select new Note
            {
                Edited = note.Edited,
                Id = note.Id,
                Color = GetColorFromString(note.Color)
            };

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

        public async Task AddNote(Note note) =>
            await _manager.AddData("Notes",
                new NoteDb
                {
                    Color = GetStringFromColor(note.Color.Color),
                    Id = note.Id,
                    Edited = note.Edited
                });

        public async Task SaveNote(Note note) =>
            await _manager.OverrideData("Notes",
                new NoteDb
                {
                    Color = GetStringFromColor(note.Color.Color),
                    Id = note.Id,
                    Edited = note.Edited
                }, note.Id);

        public async Task DeleteNote(string id) =>
            await _manager.DeleteData("Notes", id);

        #endregion Note
        #region Settings

        public async Task<Setting> LoadSettings() =>
            (await _manager.LoadData<Setting>("Settings")).FirstOrDefault();

        public async Task SaveSettings(Setting setting)
        {
            await _manager.TruncateData("Settings");
            await _manager.AddData("Settings", setting);
        }

        public async Task<IEnumerable<Theme>> LoadThemes() =>
            from theme in await _manager.LoadData<ThemeDb>("Themes")
            select new Theme
            {
                Id = theme.Id,
                Name = theme.Name,
                Background = GetColorFromString(theme.Background),
                Foreground = GetColorFromString(theme.Foreground),
                Hover = GetColorFromString(theme.Hover),
                DarkIcons = theme.DarkIcons
            };

        public async Task SaveThemes(IEnumerable<Theme> themes)
        {
            await _manager.TruncateData("Themes");
            await _manager.AddListOfData("Themes",
                from theme in themes
                select new ThemeDb
                {
                    Id = theme.Id,
                    Name = theme.Name,
                    Background = GetStringFromColor(theme.Background.Color),
                    Foreground = GetStringFromColor(theme.Foreground.Color),
                    Hover = GetStringFromColor(theme.Hover.Color),
                    DarkIcons = theme.DarkIcons
                });
        }

        public async Task SaveTheme(Theme theme) =>
            await _manager.OverrideData("Themes",
                new ThemeDb
                {
                    Id = theme.Id,
                    Name = theme.Name,
                    Background = GetStringFromColor(theme.Background.Color),
                    Foreground = GetStringFromColor(theme.Foreground.Color),
                    Hover = GetStringFromColor(theme.Hover.Color),
                    DarkIcons = theme.DarkIcons
                }, theme.Id);

        public async Task AddTheme(Theme theme) =>
            await _manager.AddData("Themes",
                new ThemeDb
                {
                    Id = theme.Id,
                    Name = theme.Name,
                    Background = GetStringFromColor(theme.Background.Color),
                    Foreground = GetStringFromColor(theme.Foreground.Color),
                    Hover = GetStringFromColor(theme.Hover.Color),
                    DarkIcons = theme.DarkIcons
                });

        #endregion Settings
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
                       List = list.Where(w => w.TaskId == task.Id).ToList(),
                       Opacity = task.Opacity
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
                List = (await _manager.LoadData<TaskText>("TasksList", "TaskId", id)).ToList(),
                Opacity = task.Opacity
            };
        }

        public async Task AddTask(TaskModel task)
        {
            await _manager.AddData("Tasks", new TaskDb
            {
                Id = task.Id,
                Color = GetStringFromColor(task.Color.Color),
                Name = task.Name,
                Opacity = task.Opacity
            });
            await _manager.AddListOfData("TasksList", task.List, "TaskId", task.Id);
        }

        public async Task SaveTask(TaskModel task)
        {
            await _manager.OverrideData("Tasks", new TaskDb
            {
                Id = task.Id,
                Color = GetStringFromColor(task.Color.Color),
                Name = task.Name,
                Opacity = task.Opacity
            }, task.Id);
            await _manager.AddListOfData("TasksList", task.List, "TaskId", task.Id);
        }

        public async Task DeleteTask(string id) =>
            await _manager.DeleteData("Tasks", id);

        public async Task DeleteTaskText(string id) =>
            await _manager.DeleteData("TasksList", id);

        #endregion Tasks

        private static SolidColorBrush GetColorFromString(string str)
        {
            var split = str.Split().Select(byte.Parse).ToArray();
            return new SolidColorBrush(Color.FromRgb(split[0], split[1], split[2]));
        }

        private static string GetStringFromColor(Color color) =>
            $"{color.R} {color.G} {color.B}";
    }
}