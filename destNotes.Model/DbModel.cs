using System;

namespace destNotes.Model
{
    public class NoteDb
    {
        public string Id { get; set; }
        public DateTime Edited { get; set; }
        public string Color { get; set; }
    }

    public class TaskDb
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Color { get; set; }
    }
}