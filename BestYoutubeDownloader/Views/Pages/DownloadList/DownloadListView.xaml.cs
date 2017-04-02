using System;
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

namespace BestYoutubeDownloader.Views.Pages.DownloadList
{
    /// <summary>
    /// Interaction logic for DownloadListView.xaml
    /// </summary>
    public partial class DownloadListView : UserControl
    {
        DownloadListViewModel ViewModel => this.DataContext as DownloadListViewModel;

        public DownloadListView()
        {
            this.InitializeComponent();
        }

        private async void DownloadListView_OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.V && e.KeyboardDevice.Modifiers == ModifierKeys.Control)
            {
                await this.ViewModel.AddItem(Clipboard.GetText());
                e.Handled = true;
            }
        }

        private void DownloadListView_OnPreviewDrop(object sender, DragEventArgs e)
        {
            
        }
    }
}
