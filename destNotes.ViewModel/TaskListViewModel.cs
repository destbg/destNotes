using destNotes.Model;
using System.Collections.ObjectModel;
using destNotes.ViewModel.Interface;

namespace destNotes.ViewModel
{
    public class TaskListViewModel
    {
        public ObservableCollection<TaskModel> Tasks { get; }

        private readonly ITaskController _controller;

        public TaskListViewModel(ITaskController controller)
        {
            _controller = controller;
            Tasks = new ObservableCollection<TaskModel>(controller.LoadTasks().GetAwaiter().GetResult());
        }
    }
}