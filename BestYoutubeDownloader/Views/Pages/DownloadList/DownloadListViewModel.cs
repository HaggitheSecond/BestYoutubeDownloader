using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using BestYoutubeDownloader.Common;
using BestYoutubeDownloader.Extensions;
using BestYoutubeDownloader.Services.YoutubeDL;
using Caliburn.Micro;

namespace BestYoutubeDownloader.Views.Pages.DownloadList
{
    public class DownloadListViewModel : Screen, IPage
    {
        private IYoutubeDownloaderService _youtubeDownloaderService;

        public string Name => "Download";

        private Action<string> _output;

        private BindableCollection<DownloadItem> _items;
        private DownloadItem _selectedItem;

        private string _addItemText;

        private string _latestDestination;

        private int _itemsToDownload;
        private int _downloadedItems;

        public BindableCollection<DownloadItem> Items
        {
            get { return this._items; }
            set { this.SetProperty(ref this._items, value); }
        }

        public DownloadItem SelectedItem
        {
            get { return this._selectedItem; }
            set { this.SetProperty(ref this._selectedItem, value); }
        }

        public string AddItemText
        {
            get { return this._addItemText; }
            set { this.SetProperty(ref this._addItemText, value); }
        }

        public int ItemsToDownload
        {
            get { return this._itemsToDownload; }
            set { this.SetProperty(ref this._itemsToDownload, value); }
        }

        public int DownloadedItems
        {
            get { return this._downloadedItems; }
            set { this.SetProperty(ref this._downloadedItems, value); }
        }

        public BestAsyncCommand AddItemCommand { get; }
        public BestAsyncCommand DownloadAllItemsCommand { get; }

        public DownloadListViewModel()
        {
            this._youtubeDownloaderService = IoC.Get<IYoutubeDownloaderService>();

            this.AddItemCommand = new BestAsyncCommand(async () => { await this.AddItem(this.AddItemText); }, this.CanAddItem);
            this.DownloadAllItemsCommand = new BestAsyncCommand(this.DownloadAllItems, this.CanDownloadAllItems);
            this.Items = new BindableCollection<DownloadItem>();

            this._output = this.DownloadOutput;
        }

        private bool CanDownloadAllItems()
        {
            return this.Items.Count != 0;
        }

        private async Task DownloadAllItems()
        {
            this.ItemsToDownload = this.Items.Count(f => f.Status == DownloadItemStatus.None);
            this.DownloadedItems = 0;

            foreach (var currentItem in this.Items)
            {
                if (currentItem.Status != DownloadItemStatus.None)
                    continue;

                currentItem.Status = DownloadItemStatus.Downloading;

                var result = await this._youtubeDownloaderService.DownloadVideo(this._output, currentItem.Url);

                if (result)
                {
                    currentItem.FileName = this._latestDestination;
                    currentItem.Status = DownloadItemStatus.SuccessfulDownload;
                }
                else
                {
                    currentItem.Status = DownloadItemStatus.Error;
                }

                this.DownloadedItems++;
            }

            this.ItemsToDownload = 0;
            this.DownloadedItems = 0;
        }

        private bool CanAddItem()
        {
            if (string.IsNullOrWhiteSpace(this.AddItemText))
                return false;

            if (this.AddItemText.IsViableYoutubeUrl() == false)
                return false;

            return true;
        }

        public async Task AddItem(string url)
        {
            if (url.IsViableYoutubeUrl() == false)
                return;

            if (this.Items.Any(f => f.Url == url))
                return;

            this.Items.Add(new DownloadItem(url));
        }

        public void RemoveSelectedItem()
        {
            var itemIndex = this.Items.IndexOf(this.SelectedItem);

            this.Items.Remove(this.SelectedItem);

            this.SelectedItem = itemIndex >= this.Items.Count 
                ? this.Items.LastOrDefault() 
                : this.Items.ElementAt(itemIndex);
        }

        private void DownloadOutput(string input)
        {
            if (string.IsNullOrEmpty(input))
                return;

            if (input.Contains("Destination"))
            {
                var inputParts = input.Split(' ').ToList();

                if (inputParts.Count < 3)
                    return;

                // remove "[download]"/"[ffmepg]" and "Destination"
                inputParts.RemoveAt(0);
                inputParts.RemoveAt(0);

                // readding whitespaces
                this._latestDestination = string.Join(" ", inputParts);
            }
        }
    }
}