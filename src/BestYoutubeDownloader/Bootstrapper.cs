using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using BestYoutubeDownloader.Common;
using BestYoutubeDownloader.Services;
using BestYoutubeDownloader.Services.CommandPrompt;
using BestYoutubeDownloader.Services.ExceptionHandling;
using BestYoutubeDownloader.Services.Import;
using BestYoutubeDownloader.Services.Message;
using BestYoutubeDownloader.Services.MetaDataTag;
using BestYoutubeDownloader.Services.Settings;
using BestYoutubeDownloader.Services.Storage;
using BestYoutubeDownloader.Services.Test;
using BestYoutubeDownloader.Services.YoutubeDL;
using BestYoutubeDownloader.Views;
using BestYoutubeDownloader.Views.EditMetaData;
using BestYoutubeDownloader.Views.Pages.DownloadList;
using BestYoutubeDownloader.Views.Pages.Info;
using BestYoutubeDownloader.Views.Pages.RawConsole;
using BestYoutubeDownloader.Views.Pages.Seperator;
using BestYoutubeDownloader.Views.Pages.Settings;
using BestYoutubeDownloader.Views.SupportedSites;
using Caliburn.Micro;

namespace BestYoutubeDownloader
{
    public class Bootstrapper : BootstrapperBase
    {
        #region Constructor and IoC-Intialization

        private SimpleContainer _container;

        public Bootstrapper()
        {
            this.Initialize();

            this.Application.DispatcherUnhandledException += this.Application_DispatcherUnhandledException;
        }

        protected override void Configure()
        {
            this._container = new SimpleContainer();
            this._container.Instance(this._container);

            this.RegisterServices();
        }

        private void RegisterServices()
        {
            this._container
                .Singleton<IWindowManager, WindowManager>()
                .Singleton<ITestService, TestService>()
                .Singleton<IYoutubeDownloaderService, YoutubeDownloaderService>()
                .Singleton<IStorageService, StorageService>()
                .Singleton<ISettingsService, SettingsService>()
                .Singleton<IImportService, ImportService>()
                .Singleton<IEventAggregator, EventAggregator>()
                .Singleton<IMetaDataTagService, MetaDataTagService>()
                .Singleton<ICommandPromptService, CommandPromptService>()
                .Singleton<IMessageService, MessageService>()
                .Singleton<IExceptionHandler, ExceptionHandler>();

            this._container
                .PerRequest<MainViewModel>()
                .PerRequest<DownloadListViewModel>()
                .PerRequest<SettingsViewModel>()
                .PerRequest<SeperatorViewModel>()
                .PerRequest<InfoViewModel>()
                .PerRequest<RawConsoleViewModel>()
                .PerRequest<SupportedSitesViewModel>()
                .PerRequest<EditMetaDataViewModel>();
        }

        #endregion

        #region Startup

        protected override void OnStartup(object sender, StartupEventArgs e)
        {
            this.DisplayRootViewFor<MainViewModel>(WindowSettings.GetMainWindowSettings());
        }
        
        #endregion

        #region IoC

        protected override object GetInstance(Type service, string key)
        {
            return this._container.GetInstance(service, key);
        }

        protected override IEnumerable<object> GetAllInstances(Type service)
        {
            return this._container.GetAllInstances(service);
        }

        protected override void BuildUp(object instance)
        {
            this._container.BuildUp(instance);
        }

        #endregion

        #region Exception

        protected override void OnUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            e.Handled = true;
            IoC.Get<IExceptionHandler>().Handle(e.Exception);
        }

        private void Application_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            e.Handled = true;
            IoC.Get<IExceptionHandler>().Handle(e.Exception);
        } 

        #endregion
    }
}