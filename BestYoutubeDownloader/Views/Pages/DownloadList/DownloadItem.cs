using System;
using System.Diagnostics;
using BestYoutubeDownloader.Common;
using BestYoutubeDownloader.Extensions;
using Caliburn.Micro;

namespace BestYoutubeDownloader.Views.Pages.DownloadList
{
    public class DownloadItem : PropertyChangedBase
    {
        private string _url;
        private DownloadItemStatus _status;
        private string _fileName;

        private string _title;
        private TimeSpan? _duration;

        private MetaData _metaData;
        private Mp3MetaData _mp3MetaData;

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
                this.SetProperty(ref this._status, value);

                this.IsLoading = value == DownloadItemStatus.Downloading;
            }
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

        private bool _isLoading;

        public bool IsLoading
        {
            get { return this._isLoading; }
            set { this.SetProperty(ref this._isLoading, value); }
        }

        public BestCommand OpenUrlCommand { get; }

        public BestCommand OpenFileCommand { get; }

        public DownloadItem(string url)
        {
            this.OpenUrlCommand = new BestCommand(this.OpenUrl);

            this.OpenFileCommand = new BestCommand(this.OpenFile, this.CanOpenFile);

            this.Url = url;

            this.Status = DownloadItemStatus.None;
        }

        public void AddMetaData(MetaData metaData)
        {
            this._metaData = metaData;

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
    }
}