﻿using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using BestYoutubeDownloader.Common;
using BestYoutubeDownloader.Events;
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
    public class DownloadListViewModel : Screen, IPage, IHandle<SettingsChanged>
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
        
        private int _itemsToDownload;
        private int _downloadedItems;

        private bool _isDownloading;

        private bool _addingItem;
        private bool _showAddItemsTextBlock;
        private DownloadStatus _currentDownloadStatus;
        private bool _isExtractingAudio;

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

        public DownloadStatus CurrentDownloadStatus
        {
            get { return this._currentDownloadStatus; }
            set { this.SetProperty(ref this._currentDownloadStatus, value); }
        }

        public bool IsDownloading
        {
            get { return this._isDownloading; }
            set { this.SetProperty(ref this._isDownloading, value); }
        }

        public bool IsExtractingAudio
        {
            get { return this._isExtractingAudio; }
            set { this.SetProperty(ref this._isExtractingAudio, value); }
        }

        // non mvvm-properties - if you know a cleaner/better way to solve these you're welcome to share

        public bool AddingItem
        {
            get { return this._addingItem; }
            set { this.SetProperty(ref this._addingItem, value); }
        }

        public bool ShowAddItemsTextBlock
        {
            get { return this._showAddItemsTextBlock; }
            set { this.SetProperty(ref this._showAddItemsTextBlock, value); }
        }

        public BestCommand ShowAddItemCommand { get; }
        public BestCommand ClearItemsCommand { get; }

        public BestCommand ClearFinishedItemsCommand { get; }

        public BestAsyncCommand AddItemCommand { get; }
        public BestAsyncCommand ImportItemsCommand { get; }

        public BestCommand OpenOutputCommand { get; }

        public BestAsyncCommand DownloadAllItemsCommand { get; }

        public DownloadListViewModel(IYoutubeDownloaderService youtubeDlService,
            ISettingsService settingsService,
            IMetaDataTagService metaDataTagService,
            IEventAggregator eventAggregator)
        {
            this._youtubeDownloaderService = youtubeDlService;
            this._settingsService = settingsService;
            this._metaDataTagService = metaDataTagService;

            eventAggregator.Subscribe(this);

            this.ShowAddItemCommand = new BestCommand(() => { this.AddingItem = true; });
            this.ClearItemsCommand = new BestCommand(() => { this.Items.Clear(); }, this.Items != null && this.Items.Count != 0 && this.IsDownloading == false);
            this.ClearFinishedItemsCommand = new BestCommand(() =>
            {
                var items = this.Items.Where(f => f.Status != DownloadItemStatus.SuccessfulDownload);
                this.Items = new BindableCollection<DownloadItem>(items);

            }, this.Items != null && this.Items.Count != 0);

            this.AddItemCommand = new BestAsyncCommand(async () => { await this.AddItem(this.AddItemText); }, this.CanAddItem);
            this.ImportItemsCommand = new BestAsyncCommand(this.ImportItems);

            this.OpenOutputCommand = new BestCommand(this.OpenOutput, this.CanOpenOutput);

            this.DownloadAllItemsCommand = new BestAsyncCommand(this.DownloadAllItems, this.CanDownloadAllItems);

            this.Items = new BindableCollection<DownloadItem>();
            this.Items.CollectionChanged += (sender, args) =>
            {
                if (this.Items == null || this.Items.Count == 0)
                    this.ShowAddItemsTextBlock = true;
                else
                    this.ShowAddItemsTextBlock = false;
            };

            this.AddingItem = false;
            this.ShowAddItemsTextBlock = true;

            this._output = this.DownloadOutput;

            this.IsExtractingAudio = this._settingsService.GetDownloadSettings().ExtractAudio;
        }

        private bool CanOpenOutput()
        {
            return string.IsNullOrWhiteSpace(this._settingsService.GetDownloadSettings().OutputLocation) == false;
        }

        private void OpenOutput()
        {
            Process.Start(this._settingsService.GetDownloadSettings().OutputLocation);
        }

        private bool CanDownloadAllItems()
        {
            return this.Items.Count(f => f.Status == DownloadItemStatus.None) != 0;
        }

        private async Task DownloadAllItems()
        {
            try
            {
                this.IsDownloading = true;

                this.ItemsToDownload = this.Items.Count(f => f.Status == DownloadItemStatus.None);
                this.DownloadedItems = 0;

                this.Items.Where(f => f.Status == DownloadItemStatus.None).ForEach(f => f.Status = DownloadItemStatus.Waiting);

                foreach (var currentItem in this.Items)
                {
                    var result = await currentItem.Download(this._output);

                    if (result)
                        this.DownloadedItems++;
                }
            }
            finally
            {
                this.IsDownloading = false;
            }
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

            this.AddingItem = false;
            this.AddItemText = string.Empty;
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

            this.Items.Clear();
            this.Items.AddRange(items);

            foreach (var currentItem in items)
            {
                currentItem.Status = DownloadItemStatus.Loading;

                var metaData = await this._youtubeDownloaderService.GetMetaData(currentItem.Url);

                currentItem.AddMetaData(metaData, this._settingsService.GetDownloadSettings().AudioFormat);

                currentItem.Status = DownloadItemStatus.None;
            }

            this.AddingItem = false;
        }

        public void RemoveSelectedItem()
        {
            var itemIndex = this.Items.IndexOf(this.SelectedItem);

            this.Items.Remove(this.SelectedItem);

            if (this.Items.Count != 0)
                this.SelectedItem = itemIndex >= this.Items.Count
                    ? this.Items.LastOrDefault()
                    : this.Items.ElementAt(itemIndex);
        }

        private void DownloadOutput(string input)
        {
            if (string.IsNullOrEmpty(input))
                return;

            if (YoutubeDlOutputHelper.TryReadDownloadStatus(input, out DownloadStatus status))
            {
                this.CurrentDownloadStatus = status;

                var currentItem = this.Items.FirstOrDefault(f => f.IsDownloading);

                if (currentItem != null)
                    currentItem.CurrentPercent = status.PercentDone;
            }
        }

        public void Handle(SettingsChanged message)
        {
            this.IsExtractingAudio = message.Settings.ExtractAudio;
        }
    }
}