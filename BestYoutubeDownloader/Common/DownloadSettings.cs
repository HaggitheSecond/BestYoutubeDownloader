namespace BestYoutubeDownloader.Common
{
    public class DownloadSettings
    {
        public string OutputLocation { get; set; }

        // audio options

        public bool ExtractAudio { get; set; }

        public FileFormats AudioFormat { get; set; }

        public bool TagAudio { get; set; }

        public bool TagCoverImage { get; set; }

        // debug infos

        public bool PrintDebugInfo { get; set; }

        public bool PrintTraffic { get; set; }

        // program settings

        public bool ShowConsole { get; set; }
    }
}