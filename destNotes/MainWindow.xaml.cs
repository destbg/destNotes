using System;
using destNotes.View;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using destNotes.Model;
using destNotes.ViewModel;
using NotifyIcon = System.Windows.Forms.NotifyIcon;

namespace destNotes
{
    public partial class MainWindow : Window
    {
        private readonly NotifyIcon _ni;
        private readonly DbController _controller;

        public MainWindow()
        {
            InitializeComponent();

            _controller = new DbController();

            ShowNoteList(null, null);

            var exitMenuItem = new System.Windows.Forms.MenuItem
            {
                Text = "E&xit"
            };
            var listMenuItem = new System.Windows.Forms.MenuItem
            {
                Text = "List"
            };
            exitMenuItem.Click += delegate
            {
                Application.Current.Shutdown();
            };
            listMenuItem.Click += delegate
            {
                this.WindowState = WindowState.Normal;
            };

            _ni = new NotifyIcon
            {
                Icon = new System.Drawing.Icon("Assets/destlogo.ico"),
                Visible = true,
                Text = "destV5",
                ContextMenu = new System.Windows.Forms.ContextMenu(
                    new[] { listMenuItem, exitMenuItem })
            };
            _ni.Click += delegate
            {
                this.Show();
                this.WindowState = WindowState.Normal;
            };
        }

        private void ShowSettings(object sender, RoutedEventArgs e)
        {
            var settings = new Settings();
            settings.ShowNoteList.Click += ShowNoteList;
            ControlPrincipal.Content = settings;
        }

        private void ShowNoteList(object sender, RoutedEventArgs e)
        {
            var noteList = new NoteList
            {
                DataContext = new NoteListViewModel(_controller)
            };
            noteList.NoteListBox.MouseDoubleClick += ShowNote;
            noteList.AddNote.Click += AddNote;
            noteList.ShowSettings.Click += ShowSettings;
            ControlPrincipal.Content = noteList;
        }
        private void ShowNote(object sender, MouseButtonEventArgs e)
        {
            if (!((sender as ListBox)?.SelectedItem is Note noteModel)) return;
            var note = new NoteWindow(_controller, noteModel.Id);
            note.AddNote.Click += AddNote;
            note.Show();
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
            note.AddNote.Click += AddNote;
            note.Show();
        }
    }
}