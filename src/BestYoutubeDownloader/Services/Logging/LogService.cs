using System;
using System.IO;
using System.Runtime.CompilerServices;
using log4net;
using log4net.Config;

namespace BestYoutubeDownloader.Services.Logging
{
    public class LogService : ILogService
    {
        private const string ConfigFileName = "log4net.config";

        public LogService()
        {
            XmlConfigurator.Configure(new FileInfo(Path.Combine(AppContext.BaseDirectory, ConfigFileName)));
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

        private ILog GetLogger(string callerFilePath)
        {
            return LogManager.GetLogger(Path.GetFileNameWithoutExtension(callerFilePath));
        }
    }
}
