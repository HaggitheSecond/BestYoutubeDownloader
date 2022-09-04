using BestYoutubeDownloader.Common;
using BestYoutubeDownloader.Services.Storage;
using BestYoutubeDownloader.Services.YoutubeDL;
using Caliburn.Micro;

namespace BestYoutubeDownloader.Services.Settings
{
    public class SettingsService : ISettingsService
    {
        private DownloadSettings? _settings;

        private readonly IStorageService _storageService;

        public SettingsService()
        {
            this._storageService = IoC.Get<IStorageService>();
        }

        public DownloadSettings GetDownloadSettings()
        {
            return this._settings ?? this.LoadSettings();
        }

        private DownloadSettings LoadSettings()
        {
            var settings = this._storageService.Load<DownloadSettings>();

            if (settings is null)
            {
                this._storageService.Save(new DownloadSettings());
                settings = this._storageService.Load<DownloadSettings>();
            }

            // welp, couldnt load settings anyways, just create some temp settings
            if (settings is null)
                settings = new();

            return settings;
        }

        public void UpdateDownloadSettings(DownloadSettings settings)
        {
            this._storageService.Save(settings);
            this._settings = settings;
        }
    }
}