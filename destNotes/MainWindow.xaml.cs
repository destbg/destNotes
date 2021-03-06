﻿using destNotes.Model;
using destNotes.View;
using destNotes.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

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

            var ni = new System.Windows.Forms.NotifyIcon
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
            var setting = _controller.LoadSettings().GetAwaiter().GetResult();
            if (setting == null) return;
            var theme = new SettingsViewModel(_controller).Themes.FirstOrDefault(f => f.Id == setting.Theme);
            if (theme == null) return;

            dirs.RemoveAt(dirs.Count - 1);
            dirs.RemoveAt(dirs.Count - 1);
            dirs.Add(theme.DarkIcons
                ? new ResourceDictionary
                {
                    Source = new Uri("/destNotes;component/View/Theme/DarkIcons.xaml", UriKind.Relative)
                }
                : new ResourceDictionary
                {
                    Source = new Uri("/destNotes;component/View/Theme/LightIcons.xaml", UriKind.Relative)
                });
            dirs.Add(new ResourceDictionary
            {
                {"BackgroundColor", theme.Background},
                {"ForegroundColor", theme.Foreground},
                {"HoverColor", theme.Hover}
            });
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
            foreach (var window in Application.Current.Windows)
            {
                if (!(window is TaskWindow taskWindow) || taskWindow.Id != taskModel.Id) continue;
                taskWindow.Focus();
                return;
            }
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
            foreach (var window in Application.Current.Windows)
            {
                if (!(window is NoteWindow noteWindow) || noteWindow.Id != noteModel.Id) continue;
                noteWindow.Focus();
                return;
            }
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