using System;
using System.Linq;
using System.Windows.Forms;
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

        private readonly ISettingsService _settingsService;
        
        private string _outputDirectoryPath;

        private bool _extractAudio;

        private BindableCollection<string> _availableAudioFormats;
        private string _selectedAudioFormat;

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

        public BestCommand ChangeDirectoryCommand { get; }

        public BestCommand SaveCommand { get; }

        public SettingsViewModel()
        {
            this._settingsService = IoC.Get<ISettingsService>();

            this.ChangeDirectoryCommand = new BestCommand(this.ChangeDirectory);
            this.SaveCommand = new BestCommand(this.Save);

            this.AvailableAudioFormats = new BindableCollection<string>(Enum.GetNames(typeof(FileFormats)));

            this.LoadSettings();
        }

        private void LoadSettings()
        {
            var settings = this._settingsService.GetDownloadSettings();

            this.OutputDirectoryPath = settings.OutputLocation;
            this.ExtractAudio = settings.ExtractAudio;

            this.SelectedAudioFormat = this.AvailableAudioFormats.FirstOrDefault(f => f == settings.AudioFormat.ToString());
        }

        private void ChangeDirectory()
        {
            var dialog = new FolderBrowserDialog();

            if (dialog.ShowDialog() != DialogResult.OK)
                return;

            this.OutputDirectoryPath = dialog.SelectedPath;
        }

        private void Save()
        {
            if(Enum.TryParse(this.SelectedAudioFormat, out FileFormats format) == false)
                return;

            var settings = new DownloadSettings
            {
                OutputLocation = this.OutputDirectoryPath,
                ExtractAudio = this.ExtractAudio,
                AudioFormat = format
            };

            this._settingsService.UpdateDownloadSettings(settings);
        }
    }
}