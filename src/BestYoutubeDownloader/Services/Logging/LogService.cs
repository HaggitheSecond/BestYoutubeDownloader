using System;
using System.IO;
using System.Runtime.CompilerServices;
using BestYoutubeDownloader.Events;
using BestYoutubeDownloader.Extensions;
using Caliburn.Micro;
using log4net.Appender;
using log4net.Config;
using log4net.Core;
using log4net.Repository.Hierarchy;

namespace BestYoutubeDownloader.Services.Logging
{
    public class LogService : ILogService
    {
        private const string ConfigFileName = "log4net.config";

        public LogService(IEventAggregator eventAggregator)
        {
            XmlConfigurator.Configure(new FileInfo(Path.Combine(AppContext.BaseDirectory, ConfigFileName)));

            var hierarchy = (Hierarchy)log4net.LogManager.GetRepository();
            hierarchy.Root.Level = Level.All;
            hierarchy.Root.AddAppender(new EventAppender(eventAggregator));
            hierarchy.RaiseConfigurationChanged(EventArgs.Empty);
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
