using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using BestYoutubeDownloader.Common;
using BestYoutubeDownloader.Extensions;
using BestYoutubeDownloader.Helper;
using BestYoutubeDownloader.Services.MetaDataTag;
using BestYoutubeDownloader.Services.Settings;
using BestYoutubeDownloader.Services.YoutubeDL;
using BestYoutubeDownloader.Views.EditMetaData;
using Caliburn.Micro;
using Action = System.Action;

namespace BestYoutubeDownloader.Views.Pages.DownloadList
{
    public class DownloadItem : PropertyChangedBase
    {
        #region Fields

        private readonly IYoutubeDownloaderService _youtubeDownloaderService;
        private readonly ISettingsService _settingsService;
        private readonly IMetaDataTagService _metaDataTagService;

        private string _url;
        private DownloadItemStatus _status;
        private decimal _currentPercent;

        private string _fileName;

        private string _title;
        private TimeSpan? _duration;

        private MetaData _metaData;
        private Mp3MetaData _mp3MetaData;

        private ImageSource _image;

        private FileFormats _format;
        private bool _isDownloading;

        private string _statusTooltip;

        #endregion

        #region Properties

        public string Url
        {
            get { return this._url; }
            set { this.SetProperty(ref this._url, value); }
        }

        public DownloadItemStatus Status
        {
            get { return this._status; }
            set
            {
                this.IsDownloading = value == DownloadItemStatus.Downloading;

                switch (value)
                {
                    case DownloadItemStatus.None:
                        this.StatusTooltip = string.Empty;
                        break;
                    case DownloadItemStatus.Loading:
                        this.StatusTooltip = "Loading";
                        break;
                    case DownloadItemStatus.Waiting:
                        this.StatusTooltip = "Waiting";
                        break;
                    case DownloadItemStatus.Downloading:
                        this.StatusTooltip = "Downloading";
                        break;
                    case DownloadItemStatus.Working:
                        this.StatusTooltip = "Working";
                        break;
                    case DownloadItemStatus.NeedsCheck:

                        if (this.Mp3MetaData != null)
                            this.StatusTooltip = this.Mp3MetaData.CheckReason;

                        break;
                    case DownloadItemStatus.Canceled:
                        this.StatusTooltip = "Canceled";
                        break;
                    case DownloadItemStatus.NonDownloadable:
                        this.StatusTooltip = "Non downloadable";
                        break;
                    case DownloadItemStatus.MetaDataNonTagable:
                        this.StatusTooltip = "Metadata not tagable";
                        break;
                    case DownloadItemStatus.Error:
                        this.StatusTooltip = "Error";
                        break;
                    case DownloadItemStatus.SuccessfulDownload:
                        this.StatusTooltip = "Success";
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(value), value, null);
                }

                this.SetProperty(ref this._status, value);
            }
        }

        public string StatusTooltip
        {
            get { return this._statusTooltip; }
            set { this.SetProperty(ref this._statusTooltip, value); }
        }

        public decimal CurrentPercent
        {
            get { return this._currentPercent; }
            set { this.SetProperty(ref this._currentPercent, value); }
        }

        public string FileName
        {
            get { return this._fileName; }
            set { this.SetProperty(ref this._fileName, value); }
        }

        public string Title
        {
            get { return this._title; }
            set { this.SetProperty(ref this._title, value); }
        }

        public TimeSpan? Duration
        {
            get { return this._duration; }
            set { this.SetProperty(ref this._duration, value); }
        }

        public Mp3MetaData Mp3MetaData
        {
            get { return this._mp3MetaData; }
            set { this.SetProperty(ref this._mp3MetaData, value); }
        }

        public FileFormats Format
        {
            get { return this._format; }
            set { this.SetProperty(ref this._format, value); }
        }

        public ImageSource Image
        {
            get { return this._image; }
            set { this.SetProperty(ref this._image, value); }
        }

        public bool IsDownloading
        {
            get { return this._isDownloading; }
            set { this.SetProperty(ref this._isDownloading, value); }
        }


        #endregion

        #region Commands

        public BestCommand OpenUrlCommand { get; }

        public BestCommand OpenFileCommand { get; }

        public BestCommand ChangeMetaDataCommand { get; }

        public BestCommand ResetStatusCommand { get; }

        #endregion

        public DownloadItem(string url)
        {
            this._youtubeDownloaderService = IoC.Get<IYoutubeDownloaderService>();
            this._settingsService = IoC.Get<ISettingsService>();
            this._metaDataTagService = IoC.Get<IMetaDataTagService>();

            this.OpenUrlCommand = new BestCommand(this.OpenUrl);

            this.OpenFileCommand = new BestCommand(this.OpenFile, this.CanOpenFile);

            this.ChangeMetaDataCommand = new BestCommand(this.ChangeMetaData, this.CanChangeMetaData);

            this.ResetStatusCommand = new BestCommand(this.ResetStatus, this.CanResetStatus);

            this.Url = url;

            this.Status = DownloadItemStatus.None;
        }

        #region CommandMethods

        private bool CanOpenFile()
        {
            return string.IsNullOrWhiteSpace(this.FileName) == false;
        }

        private void OpenFile()
        {
            Process.Start(this.FileName);
        }

        private void OpenUrl()
        {
            Process.Start(this.Url);
        }

        private bool CanChangeMetaData()
        {
            return (this.Status == DownloadItemStatus.NeedsCheck || this.Status == DownloadItemStatus.SuccessfulDownload)
                   && this.Format == FileFormats.Mp3
                   && this.FileName.ContainsNonAscii() == false;
        }

        private void ChangeMetaData()
        {
            var windowManager = IoC.Get<IWindowManager>();

            var viewModel = IoC.Get<EditMetaDataViewModel>();

            viewModel.Initialize(this.Mp3MetaData, this._metaData, this.FileName, this.Url, this.Image);

            var result = windowManager.ShowDialog(viewModel, null, WindowSettings.GetWindowSettings(500, 500));

            if (result.HasValue && result.Value)
            {
                this._image = viewModel.Image;
                this.Status = DownloadItemStatus.SuccessfulDownload;
            }
        }

        private bool CanResetStatus()
        {
            return this.Status == DownloadItemStatus.SuccessfulDownload;
        }

        private void ResetStatus()
        {
            this.Status = DownloadItemStatus.None;
            this.Mp3MetaData = null;
            this.FileName = string.Empty;
        }

        #endregion

        #region Public Methods

        public async Task<bool> Download(Action<string> output)
        {
            if (this.Status != DownloadItemStatus.Waiting)
                return false;

            this.Status = DownloadItemStatus.Downloading;

            var result = await this._youtubeDownloaderService.DownloadVideo(WrapedOutput, this.Url, this._settingsService.GetDownloadSettings());

            if (result)
            {
                return await this.SetMetaData();
            }

            this.Status = DownloadItemStatus.Error;
            return false;
            
            void WrapedOutput(string input)
            {
                if (string.IsNullOrEmpty(input))
                    return;

                if (YoutubeDlOutputHelper.TryGetFilePath(input, out string filePath))
                    this.FileName = filePath;

                if (YoutubeDlOutputHelper.TryReadDownloadStatus(input, out DownloadStatus status))
                    this.CurrentPercent = status.PercentDone;

                output.Invoke(input);
            }
        }

        public void AddMetaData(MetaData metaData, FileFormats format)
        {
            this._metaData = metaData;
            this.Format = format;

            this.Title = metaData.Title;

            if (double.TryParse(metaData.Duration, out double duration) == false)
                return;

            this.Duration = TimeSpan.FromSeconds(duration);
        }

        #endregion

        #region Privat Methods

        private async Task<bool> SetMetaData()
        {
            if (this.FileName.ContainsNonAscii())
            {
                this.Status = DownloadItemStatus.MetaDataNonTagable;
                return true;
            }

            var settings = this._settingsService.GetDownloadSettings();

            if (settings.TagAudio && settings.AudioFormat == FileFormats.Mp3)
            {
                this.Status = DownloadItemStatus.Working;

                var mp3Data = MetaDataHelper.GetTitleAndArtist(Path.GetFileNameWithoutExtension(this.FileName));

                this.Mp3MetaData = mp3Data;

                if (mp3Data.NeedCheck)
                {
                    this.Status = DownloadItemStatus.NeedsCheck;
                }

                this._metaDataTagService.TagMetaData(this.FileName, mp3Data);

                if (settings.TagCoverImage)
                {
                    await this.SetImage();
                }

                if (settings.AdjustFileName && this.Status == DownloadItemStatus.SuccessfulDownload)
                {
                    this.SetFileName();
                }
            }

            if (this.Status != DownloadItemStatus.NeedsCheck)
                this.Status = DownloadItemStatus.SuccessfulDownload;

            return true;
        }

        private async Task SetImage()
        {
            var imageResult = await this._youtubeDownloaderService.GetThumbNail(this.Url);

            if (imageResult != null)
            {
                var bitmapImage = imageResult as BitmapImage;
                var path = bitmapImage?.UriSource.LocalPath;

                if (path != null)
                {
                    if (File.Exists(path))
                    {
                        this._metaDataTagService.TagCoverImage(this.FileName, path);
                        this.Image = imageResult;
                    }
                }
            }
            else
            {
                this.Status = DownloadItemStatus.NeedsCheck;
            }
        }

        private void SetFileName()
        {

        }

        #endregion
    }
}