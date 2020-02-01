using System;

namespace BestYoutubeDownloader.Services.ExceptionHandling
{
    public interface IExceptionHandler
    {
        void Handle(Exception e);
    }
}
