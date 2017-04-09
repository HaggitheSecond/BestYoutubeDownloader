using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using BestYoutubeDownloader.Common;
using BestYoutubeDownloader.Extensions;
using BestYoutubeDownloader.Helper;
using BestYoutubeDownloader.Services.Import;
using BestYoutubeDownloader.Services.MetaDataTag;
using BestYoutubeDownloader.Services.Settings;
using BestYoutubeDownloader.Services.YoutubeDL;
using Caliburn.Micro;
using DevExpress.Mvvm.Native;
using Screen = Caliburn.Micro.Screen;

namespace BestYoutubeDownloader.Views.Pages.DownloadList
{
    public class DownloadListViewModel : Screen, IPage
    {
        private readonly IYoutubeDownloaderService _youtubeDownloaderService;
        private readonly ISettingsService _settingsService;
        private readonly IMetaDataTagService _metaDataTagService;

        public string Name => "Download";
        public ImageSource Icon => new BitmapImage(new Uri("pack://application:,,,/Resources/Icons/Download-48.png"));
        public SolidColorBrush Color => new SolidColorBrush(Colors.Red);

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
        public BestAsyncCommand ImportItemsCommand { get; }

        public BestAsyncCommand DownloadAllItemsCommand { get; }

        public DownloadListViewModel(IYoutubeDownloaderService youtubeDlService, ISettingsService settingsService, IMetaDataTagService metaDataTagService)
        {
            this._youtubeDownloaderService = youtubeDlService;
            this._settingsService = settingsService;
            this._metaDataTagService = metaDataTagService;

            this.AddItemCommand = new BestAsyncCommand( async () => { await this.AddItem(this.AddItemText); }, this.CanAddItem);
            this.ImportItemsCommand = new BestAsyncCommand(this.ImportItems);

            this.DownloadAllItemsCommand = new BestAsyncCommand(this.DownloadAllItems, this.CanDownloadAllItems);

            this.Items = new BindableCollection<DownloadItem>();

            this._output = this.DownloadOutput;
        }

        private bool CanDownloadAllItems()
        {
            return this.Items.Count(f => f.Status == DownloadItemStatus.None) != 0;
        }

        private async Task DownloadAllItems()
        {
            this.ItemsToDownload = this.Items.Count(f => f.Status == DownloadItemStatus.None);
            this.DownloadedItems = 0;

            this.Items.Where(f => f.Status == DownloadItemStatus.None).ForEach(f => f.Status = DownloadItemStatus.Waiting);

            foreach (var currentItem in this.Items)
            {
                if (currentItem.Status != DownloadItemStatus.Waiting)
                    continue;

                var settings = this._settingsService.GetDownloadSettings();

                currentItem.Status = DownloadItemStatus.Downloading;

                var result = await this._youtubeDownloaderService.DownloadVideo(this._output, currentItem.Url, settings);

                if (result)
                {
                    currentItem.FileName = this._latestDestination;

                    if (settings.TagAudio && settings.AudioFormat == FileFormats.Mp3)
                    {
                        currentItem.Status = DownloadItemStatus.Working;

                        var mp3Data = MetaDataHelper.GetTitleAndArtist(Path.GetFileNameWithoutExtension(currentItem.FileName));

                        currentItem.Mp3MetaData = mp3Data;

                        if (mp3Data.NeedCheck)
                        {
                            currentItem.Status = DownloadItemStatus.NeedsCheck;
                            continue;
                        }

                        this._metaDataTagService.TagMetaData(currentItem.FileName, mp3Data);
                    }

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

            if (this.AddItemText.IsViableUrl() == false)
                return false;

            return true;
        }

        public async Task AddItem(string url)
        {
            if (url.IsViableUrl() == false)
                return;

            if (this.Items.Any(f => f.Url == url))
                return;

            var item = new DownloadItem(url);

            this.Items.Add(item);

            item.Status = DownloadItemStatus.Loading;

            var metaData = await this._youtubeDownloaderService.GetMetaData(item.Url);

            if (metaData == null)
            {
                item.Status = DownloadItemStatus.NonDownloadable;
            }
            else
            {
                item.AddMetaData(metaData, this._settingsService.GetDownloadSettings().AudioFormat);
                item.Status = DownloadItemStatus.None;
            }
        }

        private async Task ImportItems()
        {
            var fileDialog = new OpenFileDialog
            {
                Filter = @"Text files (*.txt)|*.txt",
                InitialDirectory = this._settingsService.GetDownloadSettings().OutputLocation
            };


            if (fileDialog.ShowDialog() != DialogResult.OK)
                return;

            var importService = IoC.Get<IImportService>();

            var items = importService.ImportDownloadItemsFromFile(fileDialog.FileName);

            this.Items = new BindableCollection<DownloadItem>(items);

            foreach (var currentItem in items)
            {
                currentItem.Status = DownloadItemStatus.Loading;

                var metaData = await this._youtubeDownloaderService.GetMetaData(currentItem.Url);

                currentItem.AddMetaData(metaData, this._settingsService.GetDownloadSettings().AudioFormat);

                currentItem.Status = DownloadItemStatus.None;
            }

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

            if (YoutubeDlOutputHelper.TryGetFilePath(input, out string filePath))
            {
                this._latestDestination = filePath;
            }
        }
    }
}