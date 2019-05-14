using destNotes.Model;
using destNotes.View;
using destNotes.ViewModel;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
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

            ShowNoteList();

            _ni = new NotifyIcon
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
            var listItem = new System.Windows.Forms.ToolStripMenuItem("List",
                null,
                delegate
                {
                    this.Show();
                    ShowNoteList();
                    this.WindowState = WindowState.Normal;
                });
            var exitItem = new System.Windows.Forms.ToolStripMenuItem("Exit", 
                    null,
                    delegate
                    {
                        Application.Current.Shutdown();
                    });

            _ni.ContextMenuStrip.Items.Add(logoItem);
            _ni.ContextMenuStrip.Items.Add(new System.Windows.Forms.ToolStripSeparator());
            _ni.ContextMenuStrip.Items.Add(settingsItem);
            _ni.ContextMenuStrip.Items.Add(listItem);
            _ni.ContextMenuStrip.Items.Add(new System.Windows.Forms.ToolStripSeparator());
            _ni.ContextMenuStrip.Items.Add(exitItem);

            _ni.Click += delegate
            {
                this.Show();
                ShowNoteList();
                this.WindowState = WindowState.Normal;
            };
        }

        private void ShowSettings(object sender = null, RoutedEventArgs e = null)
        {
            var settings = new Settings();
            settings.ShowNoteList.Click += ShowNoteList;
            ControlPrincipal.Content = settings;
        }

        private void ShowNoteList(object sender = null, RoutedEventArgs e = null)
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
            note.ShowNoteList.Click += ShowNoteListClick;
            note.DeleteNote.Click += DeleteNote;
            note.AddNote.Click += AddNote;
            note.Show();
        }

        private async void DeleteNote(object sender, RoutedEventArgs e)
        {
            if (!(sender is Button button)) return;
            await _controller.DeleteNote(button.Tag.ToString());
            var parent = VisualTreeHelper.GetParent(button);
            while (!(parent is Window))
                parent = VisualTreeHelper.GetParent(parent);
            (parent as Window).Close();
        }

        private void ShowNoteListClick(object sender, RoutedEventArgs e)
        {
            this.Show();
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
            note.ShowNoteList.Click += ShowNoteListClick;
            note.DeleteNote.Click += DeleteNote;
            note.AddNote.Click += AddNote;
            note.Show();
            ShowNoteList();
        }
    }
}