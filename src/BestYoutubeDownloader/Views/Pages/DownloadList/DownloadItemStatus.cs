namespace BestYoutubeDownloader.Views.Pages.DownloadList
{
    public enum DownloadItemStatus
    {
        None,
        Loading,
        Waiting,
        Downloading,
        Working,
        NeedsCheck,
        Canceled,
        NonDownloadable,
        MetaDataNonTagable,
        Error,
        SuccessfulDownload,
        ExtractingAudio,
        AlreadyDownloaded,
    }
}