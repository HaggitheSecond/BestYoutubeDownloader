using System;
using System.Threading.Tasks;

namespace BestYoutubeDownloader.Services.YoutubeDL
{
    public interface IYoutubeDownloaderService
    {
        Task<bool> DownloadVideo(Action<string> output, string url);

        Task<string> GetThumbNail(string url);
        
        Task<string> Validate();

        void ReloadSettings();
    }
}