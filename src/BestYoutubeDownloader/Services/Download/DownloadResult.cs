namespace BestYoutubeDownloader.Services.Download
{
    public class DownloadResult
    {
        public DownloadResultStatus Status { get; set; }

        public string? FilePath { get; set; }
    }
}
