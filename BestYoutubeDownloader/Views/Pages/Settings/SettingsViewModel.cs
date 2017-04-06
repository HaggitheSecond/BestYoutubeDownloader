using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using BestYoutubeDownloader.Common;
using BestYoutubeDownloader.Extensions;
using BestYoutubeDownloader.Services.Settings;
using BestYoutubeDownloader.Services.Storage;
using Caliburn.Micro;
using DevExpress.Mvvm.UI;
using Screen = Caliburn.Micro.Screen;

namespace BestYoutubeDownloader.Views.Pages.Settings
{
    public class SettingsViewModel : Screen, IPage
    {
        public string Name => "Settings";
        public ImageSource Icon => new BitmapImage(new Uri("pack://application:,,,/Resources/Icons/Settings-64.png"));

        private readonly ISettingsService _settingsService;
        
        private string _outputDirectoryPath;

        private bool _extractAudio;
        private bool _tagAudio;

        private BindableCollection<string> _availableAudioFormats;
        private string _selectedAudioFormat;

        private bool _showConsole;

        public string OutputDirectoryPath
        {
            get { return this._outputDirectoryPath; }
            set { this.SetProperty(ref this._outputDirectoryPath, value); }
        }

        public bool ExtractAudio
        {
            get { return this._extractAudio; }
            set { this.SetProperty(ref this._extractAudio, value); }
        }

        public BindableCollection<string> AvailableAudioFormats
        {
            get { return this._availableAudioFormats; }
            set { this.SetProperty(ref this._availableAudioFormats, value); }
        }

        public string SelectedAudioFormat
        {
            get { return this._selectedAudioFormat; }
            set { this.SetProperty(ref this._selectedAudioFormat, value); }
        }

        public bool TagAudio
        {
            get { return this._tagAudio; }
            set { this.SetProperty(ref this._tagAudio, value); }
        }

        public bool ShowConsole
        {
            get { return this._showConsole; }
            set { this.SetProperty(ref this._showConsole, value); }
        }

        public BestCommand ChangeDirectoryCommand { get; }

        public BestAsyncCommand SaveCommand { get; }

        public SettingsViewModel()
        {
            this._settingsService = IoC.Get<ISettingsService>();

            this.ChangeDirectoryCommand = new BestCommand(this.ChangeDirectory);
            this.SaveCommand = new BestAsyncCommand(this.Save);

            this.AvailableAudioFormats = new BindableCollection<string>(Enum.GetNames(typeof(FileFormats)));

            this.LoadSettings();
        }

        private void LoadSettings()
        {
            var settings = this._settingsService.GetDownloadSettings();

            this.OutputDirectoryPath = settings.OutputLocation;
            this.ExtractAudio = settings.ExtractAudio;
            this.TagAudio = settings.TagAudio;
            this.ShowConsole = settings.ShowConsole;

            this.SelectedAudioFormat = this.AvailableAudioFormats.FirstOrDefault(f => f == settings.AudioFormat.ToString());
        }

        private void ChangeDirectory()
        {
            var dialog = new FolderBrowserDialog();

            if (dialog.ShowDialog() != DialogResult.OK)
                return;

            this.OutputDirectoryPath = dialog.SelectedPath;
        }

        private async Task Save()
        {
            if(Enum.TryParse(this.SelectedAudioFormat, out FileFormats format) == false)
                return;

            var settings = new DownloadSettings
            {
                OutputLocation = this.OutputDirectoryPath,
                ExtractAudio = this.ExtractAudio,
                AudioFormat = format,
                TagAudio = this.TagAudio,
                ShowConsole = this.ShowConsole
            };

            this._settingsService.UpdateDownloadSettings(settings);

            // waiting to make the button more responive
            await Task.Delay(1000);
        }
    }
}