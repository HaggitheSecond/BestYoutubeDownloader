﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BestYoutubeDownloader.Views
{
    /// <summary>
    /// Interaction logic for MainView.xaml
    /// </summary>
    public partial class MainView : UserControl
    {
        public MainView()
        {
            InitializeComponent();
        }

        private void MainView_OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            var view = (MainView) sender;

            this.MainListBox.MaxWidth = view.ActualWidth < 500 ? 40 : int.MaxValue;
        }
    }
}
