using System;
using System.Threading.Tasks;
using BestYoutubeDownloader.Common;
using BestYoutubeDownloader.Services.Settings;

namespace BestYoutubeDownloader.Services.YoutubeDL
{
    public interface IYoutubeDownloaderService
    {
        Task<bool> ExecuteCommand(Action<string> output, string command);

        Task<bool> DownloadVideo(Action<string> output, string url, DownloadSettings settings);

        Task<MetaData> GetMetaData(string url);
        
        Task<string> Validate();

        void RegisterOutputAction(Action<string> output);
    }
}