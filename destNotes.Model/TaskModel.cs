using System.Collections.Generic;
using System.Windows.Media;

namespace destNotes.Model
{
    public class TaskModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public IList<TaskText> List { get; set; }
        public SolidColorBrush Color { get; set; }
    }
}