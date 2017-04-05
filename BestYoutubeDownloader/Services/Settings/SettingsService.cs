using BestYoutubeDownloader.Services.Storage;
using BestYoutubeDownloader.Services.YoutubeDL;
using Caliburn.Micro;

namespace BestYoutubeDownloader.Services.Settings
{
    public class SettingsService : ISettingsService
    {
        private DownloadSettings _settings;

        private readonly IStorageService _storageService;

        public SettingsService()
        {
            this._storageService = IoC.Get<IStorageService>();
        }

        public DownloadSettings GetDownloadSettings()
        {
            if (this._settings == null)
                this.LoadSettings();

            return this._settings;
        }

        private void LoadSettings()
        {
            var settings = this._storageService.Load<DownloadSettings>();

            if (settings == null)
            {
                this._storageService.Save(new DownloadSettings());
                settings = this._storageService.Load<DownloadSettings>();
            }

            this._settings = settings;
        }

        public void UpdateDownloadSettings(DownloadSettings settings)
        {
            this._storageService.Save(settings);
            this._settings = settings;
        }
    }
}