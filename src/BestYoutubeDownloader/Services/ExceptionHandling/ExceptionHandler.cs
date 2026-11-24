using System;
using System.Windows;
using BestYoutubeDownloader.Services.Logging;

namespace BestYoutubeDownloader.Services.ExceptionHandling
{
    public class ExceptionHandler : IExceptionHandler
    {
        private readonly ILogService _logService;

        public ExceptionHandler(ILogService logService)
        {
            this._logService = logService;
        }

        public void Handle(Exception e)
        {
            this._logService.Error(e.Message, e);

            MessageBox.Show(e.Message, "An error occured", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
