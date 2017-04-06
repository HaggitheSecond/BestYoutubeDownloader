using System.Windows;
using System.Windows.Input;
using BestYoutubeDownloader.Common;
using BestYoutubeDownloader.Extensions;
using BestYoutubeDownloader.Services;
using BestYoutubeDownloader.Services.Settings;
using BestYoutubeDownloader.Services.YoutubeDL;
using BestYoutubeDownloader.Views.Pages;
using BestYoutubeDownloader.Views.Pages.DownloadList;
using BestYoutubeDownloader.Views.Pages.Info;
using BestYoutubeDownloader.Views.Pages.RawConsole;
using BestYoutubeDownloader.Views.Pages.Seperator;
using BestYoutubeDownloader.Views.Pages.Settings;
using Caliburn.Micro;
using DevExpress.Mvvm;

namespace BestYoutubeDownloader.Views
{
    public class MainViewModel : Conductor<IPage>.Collection.OneActive
    {
        private DownloadListViewModel _downloadListViewModel;
        private RawConsoleViewModel _rawConsoleViewModel;
        private SettingsViewModel _settingsViewModel;
        private InfoViewModel _infoViewModel;

        public DownloadListViewModel DownloadListViewModel
        {
            get { return this._downloadListViewModel; }
            set { this.SetProperty(ref this._downloadListViewModel, value); }
        }


        public RawConsoleViewModel RawConsoleViewModel
        {
            get { return this._rawConsoleViewModel; }
            set { this.SetProperty(ref this._rawConsoleViewModel, value); }
        }

        public SettingsViewModel SettingsViewModel
        {
            get { return this._settingsViewModel; }
            set { this.SetProperty(ref this._settingsViewModel, value); }
        }

        public InfoViewModel InfoViewModel
        {
            get { return this._infoViewModel; }
            set { this.SetProperty(ref this._infoViewModel, value); }
        }

        public MainViewModel()
        {
            this.DisplayName = "Youtube DL";

            this.DownloadListViewModel = IoC.Get<DownloadListViewModel>();
            this.Items.Add(this.DownloadListViewModel);

            if (IoC.Get<ISettingsService>().GetDownloadSettings().ShowConsole)
            {
                this.RawConsoleViewModel = IoC.Get<RawConsoleViewModel>();
                this.Items.Add(this.RawConsoleViewModel);
            }

            this.SettingsViewModel = IoC.Get<SettingsViewModel>();
            this.Items.Add(this.SettingsViewModel);

            this.Items.Add(IoC.Get<SeperatorViewModel>());

            this.InfoViewModel = IoC.Get<InfoViewModel>();
            this.Items.Add(this.InfoViewModel);
        }
        
        protected override async void OnActivate()
        {
            var youtubeDlService = IoC.Get<IYoutubeDownloaderService>();

            var validationResult = await youtubeDlService.Validate();

            if (validationResult != string.Empty)
            {
                if (MessageBox.Show(validationResult, "Error", MessageBoxButton.OK, MessageBoxImage.Error) == MessageBoxResult.OK)
                {
                    Application.Current.Shutdown();
                }
            }

            this.ActivateItem(this.DownloadListViewModel);
        }
    }
}