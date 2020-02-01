using System;
using System.Windows;

namespace BestYoutubeDownloader.Services.ExceptionHandling
{
    public class ExceptionHandler : IExceptionHandler
    {
        public void Handle(Exception e)
        {
            MessageBox.Show(e.Message, "An error occured", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
