using System;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using BestYoutubeDownloader.Events;
using BestYoutubeDownloader.Extensions;
using Caliburn.Micro;
using log4net.Appender;
using log4net.Config;
using log4net.Core;
using log4net.Repository.Hierarchy;

namespace BestYoutubeDownloader.Services.Logging
{
    public class LogService : ILogService, IHandle<SettingsChanged>
    {
        private const string ConfigFileName = "log4net.config";

        private readonly Hierarchy _hierarchy;

        public LogService(IEventAggregator eventAggregator)
        {
            XmlConfigurator.Configure(new FileInfo(Path.Combine(AppContext.BaseDirectory, ConfigFileName)));

            this._hierarchy = (Hierarchy)log4net.LogManager.GetRepository();
            this._hierarchy.Root.Level = Level.All;
            this._hierarchy.Root.AddAppender(new EventAppender(eventAggregator));
            this._hierarchy.RaiseConfigurationChanged(EventArgs.Empty);

            eventAggregator.SubscribeOnUIThread(this);
        }

        public void Debug(string message, [CallerFilePath] string callerFilePath = "")
        {
            this.GetLogger(callerFilePath).Debug(message);
        }

        public void Info(string message, [CallerFilePath] string callerFilePath = "")
        {
            this.GetLogger(callerFilePath).Info(message);
        }

        public void Warn(string message, Exception? exception = null, [CallerFilePath] string callerFilePath = "")
        {
            this.GetLogger(callerFilePath).Warn(message, exception);
        }

        public void Error(string message, Exception? exception = null, [CallerFilePath] string callerFilePath = "")
        {
            this.GetLogger(callerFilePath).Error(message, exception);
        }

        public void SetFileLogLevel(LogLevel level)
        {
            var fileAppenders = this._hierarchy.Root.Appenders
                .OfType<AppenderSkeleton>()
                .Where(f => f is not EventAppender);

            foreach (var appender in fileAppenders)
            {
                appender.Threshold = level.ToLog4NetLevel();
            }
        }

        public Task HandleAsync(SettingsChanged message, CancellationToken cancellationToken)
        {
            this.SetFileLogLevel(message.Settings.LogLevel);

            return Task.CompletedTask;
        }

        private log4net.ILog GetLogger(string callerFilePath)
        {
            return log4net.LogManager.GetLogger(Path.GetFileNameWithoutExtension(callerFilePath));
        }

        private class EventAppender : AppenderSkeleton
        {
            private readonly IEventAggregator _eventAggregator;

            public EventAppender(IEventAggregator eventAggregator)
            {
                this._eventAggregator = eventAggregator;
            }

            protected override void Append(LoggingEvent loggingEvent)
            {
                var message = new LogEntryAdded(
                    loggingEvent.TimeStamp,
                    loggingEvent.Level.ToLogLevel(),
                    loggingEvent.LoggerName ?? string.Empty,
                    loggingEvent.RenderedMessage ?? string.Empty,
                    loggingEvent.ExceptionObject);

                this._eventAggregator.PublishOnCurrentThreadAsync(message);
            }
        }
    }
}
