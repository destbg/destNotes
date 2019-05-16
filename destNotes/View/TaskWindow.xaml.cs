using destNotes.Model;
using destNotes.ViewModel;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using destNotes.ViewModel.Interface;

namespace destNotes.View
{
    public partial class TaskWindow : Window
    {
        private readonly TaskViewModel _taskViewModel;
        private bool _startMove;
        private TaskText _taskText;
        private TaskTransfer _taskTransfer;

        public TaskWindow(ITaskController controller, string id)
        {
            InitializeComponent();
            _taskViewModel = new TaskViewModel(controller, id);
            DataContext = _taskViewModel;
            SetLuminance(_taskViewModel.Task.Color.Color);

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
                    _taskViewModel.RemoveTask(_taskTransfer.Task.Id);
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

        private void ShowOptions(object sender, RoutedEventArgs e) =>
            OptionsGrid.Visibility = Visibility.Visible;

        private void HideSettings(object sender, MouseButtonEventArgs e)
        {
            if (ColorCanvas.SelectedColor.HasValue)
            {
                var color = ColorCanvas.SelectedColor.Value;
                var task = _taskViewModel.Task;
                task.Color = new SolidColorBrush(Color.FromRgb(color.R, color.G, color.B));
                _taskViewModel.SaveTask();
                SetLuminance(color);
            }
            OptionsGrid.Visibility = Visibility.Hidden;
        }

        private void SetLuminance(Color color)
        {
            var luminance = 0.2126 * color.R + 0.7152 * color.G + 0.0722 * color.B;
            if (luminance > 127.5)
            {
                AddTaskImage.Source = new BitmapImage(new Uri(@"/destNotes;component/Assets/plus-dark.png", UriKind.Relative));
                CloseTaskImage.Source = new BitmapImage(new Uri(@"/destNotes;component/Assets/window-close-dark.png", UriKind.Relative));
            }
            else
            {
                AddTaskImage.Source = new BitmapImage(new Uri(@"/destNotes;component/Assets/plus.png", UriKind.Relative));
                CloseTaskImage.Source = new BitmapImage(new Uri(@"/destNotes;component/Assets/window-close.png", UriKind.Relative));
            }
        }

        private void RemoveTask(object sender, RoutedEventArgs e)
        {
            if (!((sender as Button)?.Parent is Grid parent)) return;
            foreach (TextBox child in parent.Children)
            {
                var tag = child.Tag.ToString();
                _taskViewModel.Tasks.Remove(_taskViewModel.Tasks.First(f => f.Id == tag));
                _taskViewModel.RemoveTask(tag);
                return;
            }
        }
    }
}