using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
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
        private string _rawUrl;

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

        public string RawUrl
        {
            get { return this._rawUrl; }
            set { this.SetProperty(ref this._rawUrl, value); }
        }

        public DownloadItemStatus Status
        {
            get { return this._status; }
            set
            {
                this.IsDownloading = value == DownloadItemStatus.Downloading;
                this.UpdateStatusTooltip(value);

                this.SetProperty(ref this._status, value);
            }
        }

        private void UpdateStatusTooltip(DownloadItemStatus value)
        {
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
                    else
                        this.StatusTooltip = "Needs to be checked";

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
                case DownloadItemStatus.ExtractingAudio:
                    this.StatusTooltip = "Caution: for long videos this will take a few minutes!";
                    break;
                case DownloadItemStatus.AlreadyDownloaded:
                    this.StatusTooltip = "This file has already been downloaded";
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(value), value, null);
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

            this.Url = url.Contains("youtube.com") ? url.Split('&').First() : url;
            this.RawUrl = url;

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
                   && this.Format == FileFormats.Mp3;
        }

        private void ChangeMetaData()
        {
            var windowManager = IoC.Get<IWindowManager>();

            var viewModel = IoC.Get<EditMetaDataViewModel>();

            viewModel.Initialize(this.Mp3MetaData, this._metaData, this.FileName, this.Url, this.Image);

            var result = windowManager.ShowDialog(viewModel, null, WindowSettings.GetWindowSettings(500, 500));

            if (result.HasValue == false || result.Value == false)
                return;

            this.SetMetaData(viewModel.Mp3MetaData);
            this.SetImage(viewModel.Image as BitmapImage);

            if(viewModel.AdjustFileName)
                this.SetFileName();

            this.Status = DownloadItemStatus.SuccessfulDownload;
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
                if (this.Status == DownloadItemStatus.AlreadyDownloaded)
                    return true;

                return await this.SetProperties();
            }

            this.Status = DownloadItemStatus.Error;
            return false;

            void WrapedOutput(string input)
            {
                if (string.IsNullOrEmpty(input))
                    return;

                if (YoutubeDlOutputHelper.HasBeenDownloaded(input))
                    this.Status = DownloadItemStatus.AlreadyDownloaded;

                if (YoutubeDlOutputHelper.TryGetFilePath(input, out string filePath))
                    this.FileName = filePath;

                if (YoutubeDlOutputHelper.TryReadDownloadStatus(input, out DownloadStatus status))
                    this.CurrentPercent = status.PercentDone;

                if (YoutubeDlOutputHelper.IsExtractingAudio(input))
                    this.Status = DownloadItemStatus.ExtractingAudio;

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

        private async Task<bool> SetProperties()
        {
            try
            {
                // adjust the filename to the actual existing file

                // this is need because the filename extracted from the console will not contain non-ascii characters
                // but the actual filename will contain them - this is needed to be able to write metadata

                // could lead to problems when multiple programs/processes are writing to the directory simultaneously

                if (File.Exists(this.FileName) == false && string.IsNullOrEmpty(this.FileName) == false)
                {
                    // ReSharper disable once AssignNullToNotNullAttribute
                    var directory = new DirectoryInfo(Path.GetDirectoryName(this.FileName));

                    var latest = directory.GetFiles().ToList().OrderByDescending(f => f.CreationTime).First();

                    this.FileName = latest.FullName;
                }

                var settings = this._settingsService.GetDownloadSettings();

                if (settings.ExtractAudio && settings.TagAudio && settings.AudioFormat == FileFormats.Mp3)
                {
                    this.Status = DownloadItemStatus.Working;

                    this.SetMetaData(MetaDataHelper.GetTitleAndArtist(Path.GetFileNameWithoutExtension(this.FileName)));

                    if (settings.TagCoverImage)
                    {
                        var imageResult = await this._youtubeDownloaderService.GetThumbNail(this.Url);

                        if (imageResult != null)
                            this.SetImage(imageResult as BitmapImage);
                    }

                    if (settings.AdjustFileName && this.Status == DownloadItemStatus.Working)
                    {
                        this.SetFileName();
                    }
                }

                if (this.Status != DownloadItemStatus.NeedsCheck)
                    this.Status = DownloadItemStatus.SuccessfulDownload;

                return true;
            }
            catch (Exception e)
            {
                this.Status = DownloadItemStatus.Error;
                throw;
            }
        }

        private void SetMetaData(Mp3MetaData mp3Data)
        {
            this.Mp3MetaData = mp3Data;

            if (mp3Data.NeedCheck)
            {
                this.Status = DownloadItemStatus.NeedsCheck;
            }

            this.Mp3MetaData.SourceUrl = this.Url;

            this._metaDataTagService.TagMetaData(this.FileName, mp3Data);
        }

        private void SetImage(BitmapImage image)
        {
            if (image == null)
                return;

            var path = image.UriSource.LocalPath;
            
            if (File.Exists(path) == false)
                return;

            this._metaDataTagService.TagCoverImage(this.FileName, path);
            this.Image = image;
        }

        private void SetFileName()
        {
            var desiredName = string.IsNullOrEmpty(this.Mp3MetaData.Artist) == false
                ? this.Mp3MetaData.Artist + " - " + this.Mp3MetaData.Title
                : this.Mp3MetaData.Title;

            var fileName = Path.GetFileNameWithoutExtension(this.FileName);

            if (fileName == desiredName)
                return;

            var desiredPath = Path.GetDirectoryName(this.FileName) + @"\" + desiredName + ".mp3";

            if(File.Exists(desiredPath))
                File.Delete(desiredPath);

            File.Move(this.FileName, desiredPath);
            this.FileName = desiredPath;
            this.Title = desiredName;
        }

        #endregion
    }
}