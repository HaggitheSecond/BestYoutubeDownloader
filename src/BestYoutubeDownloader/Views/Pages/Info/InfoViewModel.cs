using System;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using BestYoutubeDownloader.Common;
using BestYoutubeDownloader.Views.SupportedSites;
using Caliburn.Micro;

namespace BestYoutubeDownloader.Views.Pages.Info
{
    public class InfoViewModel : Screen, IPage
    {
        public string Name => "About";
        public ImageSource Icon => new BitmapImage(new Uri("pack://application:,,,/Resources/Icons/Info-64.png"));
        public SolidColorBrush Color => new SolidColorBrush(Colors.Yellow);

        public BestAsyncCommand CheckSitesCommand { get; }

        public InfoViewModel()
        {
            this.CheckSitesCommand = new BestAsyncCommand(this.CheckSites);
        }

        private async Task CheckSites()
        {
            await IoC.Get<IWindowManager>().ShowDialogAsync(IoC.Get<SupportedSitesViewModel>(), null, WindowSettings.GetWindowSettings(400, 600));
        }
    }
}