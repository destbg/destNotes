using System.Collections.ObjectModel;
using destNotes.Model;

namespace destNotes.ViewModel
{
    public class TaskListViewModel
    {
        public ObservableCollection<TaskModel> Tasks { get; }

        private readonly DbController _controller;

        public TaskListViewModel(DbController controller)
        {
            _controller = controller;
            Tasks = new ObservableCollection<TaskModel>(controller.LoadTasks().GetAwaiter().GetResult());
        }
    }
}