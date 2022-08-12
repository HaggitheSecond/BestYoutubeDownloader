using System;
using System.Threading.Tasks;
using System.Windows.Media;
using BestYoutubeDownloader.Common;
using BestYoutubeDownloader.Services.Settings;

namespace BestYoutubeDownloader.Services.YoutubeDL
{
    public interface IYoutubeDownloaderService
    {
        Task<bool> IsValid();

        Task<bool> DownloadVideo(Action<string> output, string url, DownloadSettings settings);

        Task<MetaData> GetMetaData(string url);

        Task<ImageSource> GetThumbNail(string url);

        Task<string> UpdateYoutubeDl();

        Task<string> GetYoutubeDlVersion();

        Task Validate();
    }
}