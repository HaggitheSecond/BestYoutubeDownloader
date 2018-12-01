using System;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using BestYoutubeDownloader.Common;
using BestYoutubeDownloader.Extensions;
using BestYoutubeDownloader.Services.CommandPrompt;
using BestYoutubeDownloader.Services.Settings;
using BestYoutubeDownloader.Services.YoutubeDL;
using Caliburn.Micro;

namespace BestYoutubeDownloader.Views.Pages.RawConsole
{
    public class RawConsoleViewModel : Screen, IPage
    {
        public string Name => "Console";
        public ImageSource Icon => new BitmapImage(new Uri("pack://application:,,,/Resources/Icons/Console-64.png"));
        public SolidColorBrush Color => new SolidColorBrush(Colors.Gray);

        private readonly IYoutubeDownloaderService _youtubeDlService;
        private readonly ISettingsService _settingsService;

        private string _output;
        private string _rawInput;

        private string _rawInputUrl;
        private string _rawInputOutputLocation;

        public string Output
        {
            get { return this._output; }
            set { this.SetProperty(ref this._output, value); }
        }

        public string RawInput
        {
            get { return this._rawInput; }
            set { this.SetProperty(ref this._rawInput, value); }
        }

        public string RawInputUrl
        {
            get { return this._rawInputUrl; }
            set { this.SetProperty(ref this._rawInputUrl, value); }
        }

        public string RawInputOutputLocation
        {
            get { return this._rawInputOutputLocation; }
            set { this.SetProperty(ref this._rawInputOutputLocation, value); }
        }

        public BestCommand ClearConsoleCommand { get; }

        public BestAsyncCommand ExecuteRawInputCommand { get; }

        public BestCommand AddRawInputOutputLocationCommand { get; }

        public BestCommand AddRawInputUrlCommand { get; }

        public BestCommand AddYoutubeDlCommand { get; }

        public BestCommand AddExtractAudioCommand { get; }

        public RawConsoleViewModel(IYoutubeDownloaderService youtubeDlService, ISettingsService settingsService)
        {
            this._youtubeDlService = youtubeDlService;
            this._settingsService = settingsService;

            IoC.Get<ICommandPromptService>().RegisterOutputAction(this.AddOutput);

            this.ClearConsoleCommand = new BestCommand(() => { this.Output = string.Empty; }, () => string.IsNullOrEmpty(this.Output) == false);
            this.ExecuteRawInputCommand = new BestAsyncCommand(this.ExecuteRawInput, this.CanExecuteRawInput);

            this.AddRawInputUrlCommand = new BestCommand(() =>
            {
                if (string.IsNullOrWhiteSpace(this.RawInput))
                    this.RawInput = string.Empty;

                this.RawInput += this.RawInputUrl;
            }, () => string.IsNullOrWhiteSpace(this.RawInputUrl) == false && this.RawInputUrl.IsViableUrl());

            this.AddRawInputOutputLocationCommand = new BestCommand(() =>
            {
                if (string.IsNullOrWhiteSpace(this.RawInput))
                    this.RawInput = string.Empty;

                this.RawInput += "-o " + this.RawInputOutputLocation + @"\%(title)s.%(ext)s ";
            }, () => string.IsNullOrWhiteSpace(this.RawInputOutputLocation) == false && this.RawInputOutputLocation.IsViableDirectory());

            this.AddYoutubeDlCommand = new BestCommand(() =>
            {
                if (string.IsNullOrWhiteSpace(this.RawInput))
                    this.RawInput = string.Empty;

                this.RawInput = this.RawInput.Insert(0, "youtube-dl ");

            });
            this.AddExtractAudioCommand = new BestCommand(() =>
            {
                if (string.IsNullOrWhiteSpace(this.RawInput))
                    this.RawInput = string.Empty;

                this.RawInput += "--extract-audio --audio-format mp3 ";
            });

            this.RawInputOutputLocation = this._settingsService.GetDownloadSettings().OutputLocation;
            this.RawInput = string.Empty;
        }

        private bool CanExecuteRawInput()
        {
            return string.IsNullOrWhiteSpace(this.RawInput) == false;
        }

        private async Task ExecuteRawInput()
        {
            
        }

        private void RawOutput(string obj)
        {

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