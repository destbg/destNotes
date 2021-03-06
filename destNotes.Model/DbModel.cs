﻿using System;

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
        public double Opacity { get; set; }
    }

    public class ThemeDb
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Foreground { get; set; }
        public string Background { get; set; }
        public string Hover { get; set; }
        public bool DarkIcons { get; set; }
    }
}