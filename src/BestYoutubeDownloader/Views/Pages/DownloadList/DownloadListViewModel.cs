using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using BestYoutubeDownloader.Common;
using BestYoutubeDownloader.Events;
using BestYoutubeDownloader.Extensions;
using BestYoutubeDownloader.Helper;
using BestYoutubeDownloader.Services.ExceptionHandling;
using BestYoutubeDownloader.Services.Import;
using BestYoutubeDownloader.Services.Logging;
using BestYoutubeDownloader.Services.Message;
using BestYoutubeDownloader.Services.MetaDataTag;
using BestYoutubeDownloader.Services.Settings;
using BestYoutubeDownloader.Services.Download;
using Caliburn.Micro;
using DevExpress.Mvvm.Native;
using Screen = Caliburn.Micro.Screen;

namespace BestYoutubeDownloader.Views.Pages.DownloadList
{
    public class DownloadListViewModel : Screen, IPage, IHandle<SettingsChanged>
    {
        private readonly IDownloadService _downloadService;
        private readonly ISettingsService _settingsService;
        private readonly IMetaDataTagService _metaDataTagService;
        private readonly IMessageService _messageService;
        private readonly IExceptionHandler _exceptionHandler;
        private readonly ILogService _logService;

        public string Name => "Download";
        public ImageSource Icon => new BitmapImage(new Uri("pack://application:,,,/Resources/Icons/Download-48.png"));
        public SolidColorBrush Color => new SolidColorBrush(Colors.Red);

        private Action<DownloadStatus> _output;

        private BindableCollection<DownloadItemViewModel> _items;
        private DownloadItemViewModel _selectedItem;

        private string _addItemText;

        private int _itemsToDownload;
        private int _downloadedItems;

        private bool _isDownloading;

        private bool _addingItem;
        private bool _showAddItemsTextBlock;
        private DownloadStatus _currentDownloadStatus;
        private bool _isExtractingAudio;

        public BindableCollection<DownloadItemViewModel> Items
        {
            get { return this._items; }
            set { this.SetProperty(ref this._items, value); }
        }

        public DownloadItemViewModel SelectedItem
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

        public DownloadListViewModel(IDownloadService downloadService,
            ISettingsService settingsService,
            IMetaDataTagService metaDataTagService,
            IEventAggregator eventAggregator,
            IMessageService messageService,
            IExceptionHandler exceptionHandler,
            ILogService logService)
        {
            this._logService = logService;
            this._downloadService = downloadService;
            this._settingsService = settingsService;
            this._metaDataTagService = metaDataTagService;
            this._messageService = messageService;
            this._exceptionHandler = exceptionHandler;

            eventAggregator.SubscribeOnUIThread(this);

            this.ShowAddItemCommand = new BestCommand(() => { this.AddingItem = true; });
            this.ClearItemsCommand = new BestCommand(() => { this.Items.Clear(); }, this.Items != null && this.Items.Count != 0 && this.IsDownloading == false);
            this.ClearFinishedItemsCommand = new BestCommand(() =>
            {
                var items = this.Items?.Where(f => f.Status != DownloadItemStatus.SuccessfulDownload && f.Status != DownloadItemStatus.AlreadyDownloaded)
                    ?? new List<DownloadItemViewModel>();

                this.Items = new BindableCollection<DownloadItemViewModel>(items);

            }, this.Items != null && this.Items.Count != 0);

            this.AddItemCommand = new BestAsyncCommand(async () => { await this.AddItem(this.AddItemText); }, this.CanAddItem);
            this.ImportItemsCommand = new BestAsyncCommand(this.ImportItems);

            this.OpenOutputCommand = new BestCommand(this.OpenOutput, this.CanOpenOutput);

            this.DownloadAllItemsCommand = new BestAsyncCommand(this.DownloadAllItems, this.CanDownloadAllItems);

            this.Items = new BindableCollection<DownloadItemViewModel>();
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
            ProcessHelper.OpenDirectoryOrFile(this._settingsService.GetDownloadSettings().OutputLocation);
        }

        private bool CanDownloadAllItems()
        {
            return this.Items.Any(f => f.Status == DownloadItemStatus.None);
        }

        private async Task DownloadAllItems()
        {
            try
            {
                await this._downloadService.Validate();
            }
            catch (Exception e)
            {
                this._exceptionHandler.Handle(e);
                return;
            }

            try
            {
                this.IsDownloading = true;

                this.ItemsToDownload = this.Items.Count(f => f.Status == DownloadItemStatus.None);
                this.DownloadedItems = 0;

                this.Items.Where(f => f.Status == DownloadItemStatus.None).ForEach(f => f.Status = DownloadItemStatus.Waiting);

                this._logService.Info($"Starting the download of {this.ItemsToDownload} item(s)");

                foreach (var currentItem in this.Items)
                {
                    var result = await currentItem.Download(this._output);

                    if (result)
                        this.DownloadedItems++;
                }
            }
            finally
            {
                this._logService.Info($"Finished downloading, {this.DownloadedItems} of {this.ItemsToDownload} item(s) were successful");

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
            {
                this._logService.Info($"Rejected '{url}' because it is not a valid url");

                var caption = "Non valid url";
                var message = $"It appears, that the url in your clipboard is not valid.{Environment.NewLine}" +
                              $"Your clipboard: '{Clipboard.GetText()}'";

                this._messageService.Show(message, caption, System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
                return;
            }

            if (this.Items.Any(f => f.Url == url))
            {
                this._logService.Info($"Rejected '{url}' because it was already added");

                var caption = "Url already added";
                var message = $"The url you're trying to add is already added.";

                this._messageService.Show(message, caption, System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
                return;
            }

            var item = new DownloadItemViewModel(url);

            this.Items.Add(item);

            item.Status = DownloadItemStatus.Loading;

            var metaData = await this._downloadService.GetMetaData(item.Url);

            if (metaData == null)
            {
                this._logService.Warn($"Added '{item.Url}', but it is not downloadable");

                item.Status = DownloadItemStatus.NonDownloadable;
            }
            else
            {
                item.AddMetaData(metaData, this._settingsService.GetDownloadSettings().AudioFormat);
                item.Status = DownloadItemStatus.None;

                this._logService.Info($"Added '{item.Url}' ({metaData.Title})");
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

            this._logService.Info($"Imported {items.Count} url(s) from '{fileDialog.FileName}', replacing {this.Items.Count} existing item(s)");

            this.Items.Clear();
            this.Items.AddRange(items);

            foreach (var currentItem in items)
            {
                currentItem.Status = DownloadItemStatus.Loading;

                var metaData = await this._downloadService.GetMetaData(currentItem.Url);

                currentItem.Status = DownloadItemStatus.None;

                if (metaData is null)
                {
                    this._logService.Warn($"No metadata could be loaded for the imported url '{currentItem.Url}'");

                    continue;
                }

                currentItem.AddMetaData(metaData, this._settingsService.GetDownloadSettings().AudioFormat);
            }

            this.AddingItem = false;
        }

        public void RemoveSelectedItem()
        {
            if (this.SelectedItem is null)
                return;

            var itemIndex = this.Items.IndexOf(this.SelectedItem);

            this.Items.Remove(this.SelectedItem);

            if (this.Items.Any())
                this.SelectedItem = this.Items.ElementAt(itemIndex) ?? this.Items.Last();
        }

        private void DownloadOutput(DownloadStatus status)
        {
            if (status.IsExtractingAudio)
                return;

            this.CurrentDownloadStatus = status;
        }

        public Task HandleAsync(SettingsChanged message, CancellationToken cancellationToken)
        {
            this.IsExtractingAudio = message.Settings.ExtractAudio;
            return Task.CompletedTask;
        }
    }
}