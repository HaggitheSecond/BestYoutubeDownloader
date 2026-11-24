using BestYoutubeDownloader.Services.Logging;
using log4net.Core;

namespace BestYoutubeDownloader.Extensions
{
    public static class LogLevelExtensions
    {
        public static LogLevel ToLogLevel(this Level? self)
        {
            if (self is null)
                return LogLevel.Info;

            if (self >= Level.Error)
                return LogLevel.Error;

            if (self >= Level.Warn)
                return LogLevel.Warn;

            if (self >= Level.Info)
                return LogLevel.Info;

            return LogLevel.Debug;
        }
    }
}
