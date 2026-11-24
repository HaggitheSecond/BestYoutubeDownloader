using BestYoutubeDownloader.Common;
using BestYoutubeDownloader.Services.Logging;
using BestYoutubeDownloader.Services.Storage;
using BestYoutubeDownloader.Services.Download;
using Caliburn.Micro;

namespace BestYoutubeDownloader.Services.Settings
{
    public class SettingsService : ISettingsService
    {
        private DownloadSettings? _settings;

        private readonly IStorageService _storageService;
        private readonly ILogService _logService;

        public SettingsService()
        {
            this._storageService = IoC.Get<IStorageService>();
            this._logService = IoC.Get<ILogService>();
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
                this._logService.Info("No settings could be loaded, creating default settings");

                this._storageService.Save(new DownloadSettings());
                settings = this._storageService.Load<DownloadSettings>();
            }

            // welp, couldnt load settings anyways, just create some temp settings
            if (settings is null)
            {
                this._logService.Warn("The settings could not be loaded or created, using temporary default settings");

                settings = new();
            }

            return settings;
        }

        public void UpdateDownloadSettings(DownloadSettings settings)
        {
            this._storageService.Save(settings);
            this._settings = settings;

            this._logService.Info($"Settings were saved (output location '{settings.OutputLocation}', extract audio {settings.ExtractAudio}, audio format {settings.AudioFormat})");
        }
    }
}
