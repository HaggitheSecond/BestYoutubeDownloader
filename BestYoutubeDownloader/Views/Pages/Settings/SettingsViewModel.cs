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

        private ISettingsService _settingsService;

        private DownloadSettings _settings;

        private string _outputDirectoryPath;

        public string OutputDirectoryPath
        {
            get { return this._outputDirectoryPath; }
            set { this.SetProperty(ref this._outputDirectoryPath, value); }
        }

        public BestCommand ChangeDirectoryCommand { get; }

        public BestCommand SaveCommand { get; }

        public SettingsViewModel()
        {
            this._settingsService = IoC.Get<ISettingsService>();

            this.ChangeDirectoryCommand = new BestCommand(this.ChangeDirectory);
            this.SaveCommand = new BestCommand(this.Save);

            this._settings = this._settingsService.GetDownloadSettings();

            this._outputDirectoryPath = this._settings.OutputLocation;
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
            var settings = new DownloadSettings
            {
                OutputLocation = this.OutputDirectoryPath
            };

            this._settingsService.UpdateDownloadSettings(settings);
        }
    }
}