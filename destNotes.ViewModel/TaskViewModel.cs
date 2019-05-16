using destNotes.Model;
using destNotes.ViewModel.Annotations;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using destNotes.ViewModel.Interface;

namespace destNotes.ViewModel
{
    public class TaskViewModel : INotifyPropertyChanged
    {
        public TaskModel Task { get; }

        public ObservableCollection<TaskText> Tasks { get; }

        private readonly ITaskController _controller;

        public TaskViewModel(ITaskController controller, string id)
        {
            _controller = controller;
            Task = _controller.LoadTask(id).GetAwaiter().GetResult();
            Tasks = new ObservableCollection<TaskText>(Task.List);
        }

        public async void SaveTask()
        {
            Task.List = Tasks.ToList();
            await _controller.SaveTask(Task);
            OnPropertyChanged(nameof(Task));
        }

        public void AddTask()
        {
            Tasks.Add(new TaskText
            {
                Id = Guid.NewGuid().ToString(),
                TaskId = Task.Id,
                Text = "Sample text"
            });
            SaveTask();
        }

        public async void RemoveTask(string id) => 
            await _controller.DeleteTaskText(id);

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) => 
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}