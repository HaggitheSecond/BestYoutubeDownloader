namespace BestYoutubeDownloader.Services.Settings
{
    public interface ISettingsService
    {
        DownloadSettings GetDownloadSettings();
        void UpdateDownloadSettings(DownloadSettings settings);
    }
}