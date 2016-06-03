﻿using System.Windows;

namespace GUI {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow() {
            InitializeComponent();
            PiCrossGUI.DataContext = new PiCrossViewModel();
        }
    }
}
