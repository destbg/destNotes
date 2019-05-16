using System.Windows;
using destNotes.Model;

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