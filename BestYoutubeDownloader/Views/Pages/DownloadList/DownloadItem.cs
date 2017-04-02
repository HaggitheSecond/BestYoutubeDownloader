using System.Diagnostics;
using BestYoutubeDownloader.Common;
using BestYoutubeDownloader.Extensions;
using Caliburn.Micro;

namespace BestYoutubeDownloader.Views.Pages.DownloadList
{
    public class DownloadItem : PropertyChangedBase
    {
        private string _url;
        private bool _wasSuccesful;

        public string Url
        {
            get { return this._url; }
            set { this.SetProperty(ref this._url, value); }
        }
        
        public bool WasSuccesful
        {
            get { return this._wasSuccesful; }
            set { this.SetProperty(ref this._wasSuccesful, value); }
        }

        public BestCommand OpenUrlCommand { get; }

        public DownloadItem(string url)
        {
            this.OpenUrlCommand = new BestCommand(this.OpenUrl);

            this.Url = url;


        }

        private void OpenUrl()
        {
            Process.Start(this.Url);
        }
    }
}