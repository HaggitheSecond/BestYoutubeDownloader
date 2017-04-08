using System;
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

        public BestCommand CheckSitesCommand { get; }

        public InfoViewModel()
        {
            this.CheckSitesCommand = new BestCommand(this.CheckSites);
        }

        private void CheckSites()
        {
            var windowManager = IoC.Get<IWindowManager>();

            var viewModel = new SupportedSitesViewModel();

            windowManager.ShowDialog(viewModel, null, WindowSettings.GetWindowSettings(400, 600));
        }
    }
}