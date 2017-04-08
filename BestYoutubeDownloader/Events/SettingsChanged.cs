using BestYoutubeDownloader.Common;

namespace BestYoutubeDownloader.Events
{
    public class SettingsChanged
    {
        public DownloadSettings Settings { get; private set; }

        public SettingsChanged(DownloadSettings settings)
        {
            this.Settings = settings;
        }
    }
}