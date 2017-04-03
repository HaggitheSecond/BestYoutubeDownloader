using System;
using System.Threading.Tasks;

namespace BestYoutubeDownloader.Services.YoutubeDL
{
    public interface IYoutubeDownloaderService
    {
        Task<bool> DownloadVideo(Action<string> output, string url);

        Task<string> GetMetaData(string url);
        
        Task<string> Validate();

        void ReloadSettings();

        void RegisterOutputAction(Action<string> output);
    }
}