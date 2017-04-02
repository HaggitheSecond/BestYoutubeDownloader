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
        private string _selectedItem;

        private string _addItemText;
        private string _latestOutput;

        public BindableCollection<DownloadItem> Items
        {
            get { return this._items; }
            set { this.SetProperty(ref this._items, value); }
        }
        
        public string SelectedItem
        {
            get { return this._selectedItem; }
            set { this.SetProperty(ref this._selectedItem, value); }
        }
        
        public string AddItemText
        {
            get { return this._addItemText; }
            set { this.SetProperty(ref this._addItemText, value); }
        }

        public string LatestOutput
        {
            get { return this._latestOutput; }
            set { this.SetProperty(ref this._latestOutput, value); }
        }

        public BestAsyncCommand AddItemCommand { get; }
        public BestAsyncCommand DownloadAllItemsCommand { get; }

        public DownloadListViewModel()
        {
            this._youtubeDownloaderService = IoC.Get<IYoutubeDownloaderService>();

            this.AddItemCommand = new BestAsyncCommand(async () => { await this.AddItem(this.AddItemText); }, this.CanAddItem);
            this.DownloadAllItemsCommand = new BestAsyncCommand(this.DownloadAllItems, this.CanDownloadAllItems);
            this.Items = new BindableCollection<DownloadItem>();

            this._output = this.Output;
        }

        private bool CanDownloadAllItems()
        {
            return this.Items.Count != 0;
        }

        private async Task DownloadAllItems()
        {
            foreach (var currentItem in this.Items)
            {
                var result = await this._youtubeDownloaderService.DownloadVideo(this._output, currentItem.Url);
                currentItem.WasSuccesful = result;
            }   
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
            if(url.IsViableYoutubeUrl() == false)
                return;

            if (this.Items.Any(f => f.Url == url))
                return;

            this.Items.Add(new DownloadItem(url));
        }

        private void Output(string obj)
        {
            this.LatestOutput = obj;
        }
    }
}