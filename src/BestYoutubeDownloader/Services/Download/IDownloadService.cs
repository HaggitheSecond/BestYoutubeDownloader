using System;
using System.Threading.Tasks;
using System.Windows.Media;
using BestYoutubeDownloader.Common;
using BestYoutubeDownloader.Services.Settings;
using BestYoutubeDownloader.Views.Pages.DownloadList;

namespace BestYoutubeDownloader.Services.Download
{
    public interface IDownloadService
    {
        Task<bool> IsValid();

        Task<DownloadResult> DownloadVideo(Action<DownloadStatus> output, string url, DownloadSettings settings, string? outputDirectory = null);

        Task<MetaData?> GetMetaData(string url);

        Task<ImageSource?> GetThumbNail(string url);
        void ClearThumbNailsDirectory();

        Task<string> UpdateYoutubeDl();

        Task<string> GetYoutubeDlVersion();

        Task Validate();
    }
}