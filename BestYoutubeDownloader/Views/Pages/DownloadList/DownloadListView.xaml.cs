using System;
using System.Collections.Generic;
using System.IO;
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
using BestYoutubeDownloader.Extensions;

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

        private async void DownloadListView_OnPreviewDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.Text))
            {
                var data = (string)e.Data.GetData(DataFormats.Text);

                await this.ViewModel.AddItem(data);
            }
        }

        private void DownloadList_OnDragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.Text))
            {
                var data = (string)e.Data.GetData(DataFormats.Text);

                this.DownloadList.AllowDrop = data.IsViableUrl();
            }
            else
            {
                this.DownloadList.AllowDrop = false;
            }
        }
        
        private async void DownloadList_OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.V && e.KeyboardDevice.Modifiers == ModifierKeys.Control)
            {
                await this.ViewModel.AddItem(Clipboard.GetText());
                e.Handled = true;

                return;
            }

            if (e.Key == Key.Delete)
            {
                this.ViewModel.RemoveSelectedItem();

                return;
            }
        }

        private void DownloadList_OnMouseEnter(object sender, MouseEventArgs e)
        {
            // forces keyboard focus on the listbox which is needed to enable keydownevents
            Keyboard.Focus(this.DownloadList);
        }

        private void Grid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            var grid = (Grid) sender;

            var cp =
                this.DownloadList.ItemContainerGenerator.ContainerFromItem(0) as
                    ContentPresenter;

            var tb = cp.FindVisualChild<TextBox>();

        }

        private void ListBoxItem_LeftMouseDown(object sender, MouseButtonEventArgs e)
        {
            if(e.ClickCount != 2)
                return;

            var grid = (Grid) sender;

            if(grid == null)
                return;

            var item = (DownloadItem)grid.DataContext;

            item.ChangeMetaDataCommand.Execute(null);
        }

        private void DownloadList_OnSourceUpdated(object sender, DataTransferEventArgs e)
        {
            
        }
    }
}
