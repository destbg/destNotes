using destNotes.Model;
using System.Windows;

namespace destNotes.View
{
    public partial class TaskTransfer : Window
    {
        public TaskText Task { get; }

        public TaskTransfer(TaskText task)
        {
            InitializeComponent();
            Task = task;
            OnlyTextBox.Text = task.Text;
        }
    }
}