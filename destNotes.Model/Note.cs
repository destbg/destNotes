using System;
using System.Windows.Media;

namespace destNotes.Model
{
    public class Note
    {
        public string Id { get; set; }
        public DateTime Edited { get; set; }
        public SolidColorBrush Color { get; set; }
    }
}