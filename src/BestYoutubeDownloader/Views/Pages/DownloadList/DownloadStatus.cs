using System;

namespace BestYoutubeDownloader.Views.Pages.DownloadList
{
    public class DownloadStatus
    {
        public decimal? PercentDone { get; set; }

        public string? TotalSize { get; set; }

        public string? CurrentDownloadSpeed { get; set; }

        public TimeSpan? Eta { get; set; }
    }
}