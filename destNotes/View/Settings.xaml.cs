﻿using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace destNotes.View
{
    public partial class Settings : UserControl
    {
        public Settings()
        {
            InitializeComponent();
        }

        private void MoveWindow(object sender, MouseButtonEventArgs e) => 
            Application.Current.MainWindow?.DragMove();

        private void CloseApplication(object sender, RoutedEventArgs e) => 
            Application.Current.MainWindow?.Hide();
    }
}