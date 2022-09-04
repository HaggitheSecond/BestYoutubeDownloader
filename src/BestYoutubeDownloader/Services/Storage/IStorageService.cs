using System;

namespace BestYoutubeDownloader.Services.Storage
{
    public interface IStorageService
    {
        bool Save<T>(T input, string? fileName = null);

        T? Load<T>(string? fileName = null);
    }
}