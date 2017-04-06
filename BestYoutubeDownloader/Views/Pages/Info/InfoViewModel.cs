using System;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Caliburn.Micro;

namespace BestYoutubeDownloader.Views.Pages.Info
{
    public class InfoViewModel : Screen, IPage
    {
        public string Name => "About";
        public ImageSource Icon => new BitmapImage(new Uri("pack://application:,,,/Resources/Icons/Info-64.png"));
    }
}