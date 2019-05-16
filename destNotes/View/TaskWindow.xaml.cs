using destNotes.Model;
using destNotes.ViewModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace destNotes.View
{
    public partial class TaskWindow : Window
    {
        private readonly TaskViewModel _taskViewModel;
        private bool _startMove;
        private TaskText _taskText;
        private TaskTransfer _taskTransfer;

        public TaskWindow(DbController controller, string id)
        {
            InitializeComponent();
            _taskViewModel = new TaskViewModel(controller, id);
            DataContext = _taskViewModel;

            this.ShowInTaskbar = false;
        }

        private void MoveWindow(object sender, MouseButtonEventArgs e) =>
            this.DragMove();

        private void CloseTask(object sender, RoutedEventArgs e) =>
            this.Close();

        private void AddTaskToList(object sender, RoutedEventArgs e) =>
            _taskViewModel.AddTask();

        private void ChangeTaskName(object sender, KeyboardFocusChangedEventArgs e)
        {
            var textBox = sender as TextBox;
            if (string.IsNullOrWhiteSpace(textBox?.Text)) return;
            _taskViewModel.Task.Name = textBox.Text;
            _taskViewModel.SaveTask();
        }

        private void SaveTaskText(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (!(sender is TextBox textBox)) return;
            var id = textBox.Tag.ToString();
            _taskViewModel.Tasks.First(f => f.Id == id).Text = textBox.Text;
            _taskViewModel.SaveTask();
        }

        private void ItemSelected(object sender, SelectionChangedEventArgs e)
        {
            if (!((sender as ListBox)?.SelectedItem is TaskText taskText)) return;
            _taskText = taskText;
            TasksText.UnselectAll();
        }

        private void TasksText_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e) =>
            _startMove = true;

        private void TasksText_OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (!_startMove)
            {
                var overATask = false;
                var mouse = e.GetPosition(null);
                var x = mouse.X + this.Left - 5;
                var y = mouse.Y + this.Top - 2;
                foreach (var window in Application.Current.Windows)
                {
                    if (!(window is TaskWindow task)) continue;
                    if (!(task.Top < y) || !(y < task.Top + task.Height) || !(task.Left < x) ||
                        !(x < task.Left + task.Width)) continue;
                    _taskViewModel.RemoveTask(_taskTransfer.Task);
                    task.DragDropTask(_taskTransfer.Task);
                    overATask = true;
                }
                if (!overATask)
                    _taskViewModel.Tasks.Add(_taskTransfer.Task);
                _taskText = null;
                _taskTransfer.Close();
                _taskTransfer = null;
            }
            _startMove = false;
        }

        private void TasksText_OnPreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (!_startMove || _taskText == null)
            {
                if (_taskTransfer != null)
                {
                    var mousePos = e.GetPosition(null);
                    _taskTransfer.Left = mousePos.X + this.Left - 5;
                    _taskTransfer.Top = mousePos.Y + this.Top - 2;
                }
                return;
            }
            var mouse = e.GetPosition(null);
            _taskTransfer = new TaskTransfer(_taskText)
            {
                Width = this.Width,
                Left = mouse.X + this.Left - 5,
                Top = mouse.Y + this.Top - 2
            };
            _taskTransfer.Show();
            _taskViewModel.Tasks.Remove(_taskText);
            _startMove = false;
        }

        public void DragDropTask(TaskText task)
        {
            task.TaskId = _taskViewModel.Task.Id;
            _taskViewModel.Tasks.Add(task);
            _taskViewModel.SaveTask();
        }
    }
}