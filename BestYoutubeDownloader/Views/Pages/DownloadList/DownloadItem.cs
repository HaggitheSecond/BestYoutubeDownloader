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

        public string Url
        {
            get { return this._url; }
            set { this.SetProperty(ref this._url, value); }
        }

        public DownloadItemStatus Status
        {
            get { return this._status; }
            set { this.SetProperty(ref this._status, value); }
        }

        public string FileName
        {
            get { return this._fileName; }
            set { this.SetProperty(ref this._fileName, value); }
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