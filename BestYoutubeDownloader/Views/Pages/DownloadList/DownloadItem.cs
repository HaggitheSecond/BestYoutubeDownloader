using System;
using System.Diagnostics;
using System.Windows.Media;
using BestYoutubeDownloader.Common;
using BestYoutubeDownloader.Extensions;
using BestYoutubeDownloader.Views.EditMetaData;
using Caliburn.Micro;

namespace BestYoutubeDownloader.Views.Pages.DownloadList
{
    public class DownloadItem : PropertyChangedBase
    {
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

                this.SetProperty(ref this._status, value);
            }
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

        public BestCommand OpenUrlCommand { get; }

        public BestCommand OpenFileCommand { get; }

        public BestCommand ChangeMetaDataCommand { get; }

        public BestCommand ResetStatusCommand { get; }

        public DownloadItem(string url)
        {
            this.OpenUrlCommand = new BestCommand(this.OpenUrl);

            this.OpenFileCommand = new BestCommand(this.OpenFile, this.CanOpenFile);

            this.ChangeMetaDataCommand = new BestCommand(this.ChangeMetaData, this.CanChangeMetaData);

            this.ResetStatusCommand = new BestCommand(this.ResetStatus, this.CanResetStatus);

            this.Url = url;

            this.Status = DownloadItemStatus.None;
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
    }
}