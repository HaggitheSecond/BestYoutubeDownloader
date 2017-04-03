using System;
using BestYoutubeDownloader.Common;
using BestYoutubeDownloader.Extensions;
using BestYoutubeDownloader.Services.YoutubeDL;
using Caliburn.Micro;

namespace BestYoutubeDownloader.Views.Pages.RawConsole
{
    public class RawConsoleViewModel : Screen, IPage
    {
        public string Name => "Console";

        private IYoutubeDownloaderService _youtubeDlService;

        private string _output;
        
        public string Output
        {
            get { return this._output; }
            set { this.SetProperty(ref this._output, value); }
        }

        public BestCommand ClearConsoleCommand { get; }

        public RawConsoleViewModel()
        {
            this._youtubeDlService = IoC.Get<IYoutubeDownloaderService>();
            
            this._youtubeDlService.RegisterOutputAction(this.AddOutput);

            this.ClearConsoleCommand = new BestCommand(() => { this.Output = string.Empty; }, () => string.IsNullOrEmpty(this.Output) == false);
        }

        private void AddOutput(string s)
        {
            if (string.IsNullOrEmpty(s))
                this.Output += Environment.NewLine;
            else
                this.Output += Environment.NewLine + DateTime.Now.TimeOfDay.ToString("hh\\:mm\\:ss") + ": " + s;
        }
    }
}