﻿using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using BestYoutubeDownloader.Common;
using BestYoutubeDownloader.Events;
using BestYoutubeDownloader.Extensions;
using BestYoutubeDownloader.Services;
using BestYoutubeDownloader.Services.ExceptionHandling;
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
    public class MainViewModel : Conductor<IPage>.Collection.OneActive, IHandle<SettingsChanged>
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly ISettingsService _settingsService;
        private readonly IYoutubeDownloaderService _youtubeDownloaderService;

        private DownloadListViewModel _downloadListViewModel;
        private RawConsoleViewModel _rawConsoleViewModel;
        private SettingsViewModel _settingsViewModel;
        private InfoViewModel _infoViewModel;

        private bool _isEnabled;

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

        public bool IsEnabled
        {
            get { return this._isEnabled; }
            set { this.SetProperty(ref this._isEnabled, value); }
        }

        public MainViewModel(IEventAggregator eventAggregator, ISettingsService settingsService, IYoutubeDownloaderService youtubeDownloaderService)
        {
            this._eventAggregator = eventAggregator;
            this._settingsService = settingsService;
            this._youtubeDownloaderService = youtubeDownloaderService;

            this.DisplayName = "Youtube DL";

            this._eventAggregator.SubscribeOnUIThread(this);

            this.DownloadListViewModel = IoC.Get<DownloadListViewModel>();
            this.Items.Add(this.DownloadListViewModel);

            this.RawConsoleViewModel = IoC.Get<RawConsoleViewModel>();

            if (this._settingsService.GetDownloadSettings().ShowConsole)
                this.Items.Add(this.RawConsoleViewModel);

            this.SettingsViewModel = IoC.Get<SettingsViewModel>();
            this.Items.Add(this.SettingsViewModel);

            this.Items.Add(IoC.Get<SeperatorViewModel>());

            this.InfoViewModel = IoC.Get<InfoViewModel>();
            this.Items.Add(this.InfoViewModel);

            this.IsEnabled = true;
        }

        protected override async Task OnActivateAsync(CancellationToken cancellationToken)
        {
            await this.ActivateItemAsync(this.DownloadListViewModel);

            try
            {
                await this._youtubeDownloaderService.Validate();
            }
            catch (Exception e)
            {
                IoC.Get<IExceptionHandler>().Handle(e);
            }
        }

        public Task HandleAsync(SettingsChanged message, CancellationToken cancellationToken)
        {
            if (message.Settings.ShowConsole)
            {
                if (this.Items.Contains(this.RawConsoleViewModel) == false)
                    this.Items.Insert(1, this.RawConsoleViewModel);

            }
            else
            {
                this.Items.Remove(this.RawConsoleViewModel);
            }

            return Task.CompletedTask;
        }
    }
}