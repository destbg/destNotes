using destNotes.Model;
using destNotes.View;
using destNotes.ViewModel;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using NotifyIcon = System.Windows.Forms.NotifyIcon;

namespace destNotes
{
    public partial class MainWindow : Window
    {
        private readonly DbController _controller;

        public MainWindow()
        {
            InitializeComponent();

            _controller = new DbController();

            ShowNoteList();

            var ni = new NotifyIcon
            {
                Icon = new System.Drawing.Icon("Assets/destlogo.ico"),
                Visible = true,
                Text = "destV5",
                ContextMenuStrip = new System.Windows.Forms.ContextMenuStrip()
            };

            var logoItem = new System.Windows.Forms.ToolStripMenuItem("Control panel",
                System.Drawing.Image.FromFile("Assets/destlogo.ico"))
            {
                Enabled = false
            };
            var settingsItem = new System.Windows.Forms.ToolStripMenuItem("Settings",
                null,
                delegate
                {
                    this.Show();
                    ShowSettings();
                    this.WindowState = WindowState.Normal;
                });
            var taskListItem = new System.Windows.Forms.ToolStripMenuItem("Task List",
                null,
                delegate
                {
                    ShowTaskList();
                    this.WindowState = WindowState.Normal;
                });
            var noteListItem = new System.Windows.Forms.ToolStripMenuItem("Note List",
                null,
                delegate
                {
                    ShowNoteList();
                    this.WindowState = WindowState.Normal;
                });
            var exitItem = new System.Windows.Forms.ToolStripMenuItem("Exit",
                    null,
                    delegate
                    {
                        Application.Current.Shutdown();
                    });

            ni.ContextMenuStrip.Items.Add(logoItem);
            ni.ContextMenuStrip.Items.Add(new System.Windows.Forms.ToolStripSeparator());
            ni.ContextMenuStrip.Items.Add(settingsItem);
            ni.ContextMenuStrip.Items.Add(noteListItem);
            ni.ContextMenuStrip.Items.Add(taskListItem);
            ni.ContextMenuStrip.Items.Add(new System.Windows.Forms.ToolStripSeparator());
            ni.ContextMenuStrip.Items.Add(exitItem);

            ni.MouseClick += Ni_MouseClick;


            var dirs = Application.Current.Resources.MergedDictionaries;
            switch (_controller.LoadSetting().GetAwaiter().GetResult()?.Theme.ToString())
            {
                case "Light":
                    dirs.RemoveAt(dirs.Count - 1);
                    dirs.Add(new ResourceDictionary
                    {
                        Source = new Uri("/destNotes;component/View/Theme/LightTheme.xaml", UriKind.Relative)
                    });
                    break;
                case "Dark":
                    dirs.RemoveAt(dirs.Count - 1);
                    dirs.Add(new ResourceDictionary
                    {
                        Source = new Uri("/destNotes;component/View/Theme/DarkTheme.xaml", UriKind.Relative)
                    });
                    break;
            }
        }

        private void Ni_MouseClick(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button != System.Windows.Forms.MouseButtons.Left) return;
            ShowNoteList();
            this.WindowState = WindowState.Normal;
        }

        private void ShowSettings(object sender = null, RoutedEventArgs e = null)
        {
            var settings = new Settings(_controller);
            settings.ShowNoteList.Click += ShowNoteList;
            ControlPrincipal.Content = settings;
        }

        private void ShowNoteList(object sender = null, RoutedEventArgs e = null)
        {
            this.Show();
            var noteList = new NoteList
            {
                DataContext = new NoteListViewModel(_controller)
            };
            noteList.NoteListBox.MouseDoubleClick += ShowNote;
            noteList.AddNote.Click += AddNote;
            noteList.ShowSettings.Click += ShowSettings;
            noteList.ShowTasks.Click += ShowTaskList;
            ControlPrincipal.Content = noteList;
        }

        private void ShowTaskList(object sender = null, RoutedEventArgs e = null)
        {
            this.Show();
            var taskList = new TaskList
            {
                DataContext = new TaskListViewModel(_controller)
            };
            taskList.TaskListBox.MouseDoubleClick += ShowTask;
            taskList.ShowSettings.Click += ShowSettings;
            taskList.ShowNotes.Click += ShowNoteList;
            taskList.AddTask.Click += AddTask;
            ControlPrincipal.Content = taskList;
        }

        private async void AddTask(object sender, RoutedEventArgs e)
        {
            var id = Guid.NewGuid().ToString("N");
            await _controller.AddTask(new TaskModel
            {
                Id = id,
                Color = new SolidColorBrush(Colors.OrangeRed),
                List = new List<TaskText>(),
                Name = "Default Name",
                Opacity = 0.9
            });
            var task = new TaskWindow(_controller, id);
            task.DeleteTask.Click += DeleteTask;
            task.ShowTaskList.Click += ShowTaskList;
            task.AddTask.Click += AddTask;
            task.Show();
            ShowTaskList();
        }

        private async void DeleteTask(object sender, RoutedEventArgs e)
        {
            if (!(sender is Button button)) return;
            await _controller.DeleteTask(button.Tag.ToString());
            var parent = VisualTreeHelper.GetParent(button);
            while (!(parent is Window))
                parent = VisualTreeHelper.GetParent(parent);
            (parent as Window).Close();
            ShowTaskList();
        }

        private void ShowTask(object sender, MouseButtonEventArgs e)
        {
            var listBox = sender as ListBox;
            if (!(listBox?.SelectedItem is TaskModel taskModel)) return;
            var task = new TaskWindow(_controller, taskModel.Id);
            task.DeleteTask.Click += DeleteTask;
            task.ShowTaskList.Click += ShowTaskList;
            task.AddTask.Click += AddTask;
            task.Show();
            listBox.UnselectAll();
        }

        private void ShowNote(object sender, MouseButtonEventArgs e)
        {
            var listBox = sender as ListBox;
            if (!(listBox?.SelectedItem is Note noteModel)) return;
            var note = new NoteWindow(_controller, noteModel.Id);
            note.ShowNoteList.Click += ShowNoteList;
            note.DeleteNote.Click += DeleteNote;
            note.AddNote.Click += AddNote;
            note.Show();
            listBox.UnselectAll();
        }

        private async void DeleteNote(object sender, RoutedEventArgs e)
        {
            if (!(sender is Button button)) return;
            await _controller.DeleteNote(button.Tag.ToString());
            var parent = VisualTreeHelper.GetParent(button);
            while (!(parent is Window))
                parent = VisualTreeHelper.GetParent(parent);
            (parent as Window).Close();
            ShowNoteList();
        }

        private async void AddNote(object sender, RoutedEventArgs e)
        {
            var id = Guid.NewGuid().ToString("N");
            await _controller.AddNote(new Note
            {
                Id = id,
                Color = new SolidColorBrush(Colors.OrangeRed),
                Edited = DateTime.Now
            });
            var note = new NoteWindow(_controller, id);
            note.ShowNoteList.Click += ShowNoteList;
            note.DeleteNote.Click += DeleteNote;
            note.AddNote.Click += AddNote;
            note.Show();
            ShowNoteList();
        }
    }
}