using System;
using System.Threading.Tasks;

namespace BestYoutubeDownloader.Services.YoutubeDL
{
    public interface IYoutubeDownloaderService
    {
        Task<bool> ExecuteCommand(Action<string> output, string command);

        Task<bool> DownloadVideo(Action<string> output, string url);

        Task<string> GetMetaData(string url);
        
        Task<string> Validate();

        void ReloadSettings();

        void RegisterOutputAction(Action<string> output);
    }
}