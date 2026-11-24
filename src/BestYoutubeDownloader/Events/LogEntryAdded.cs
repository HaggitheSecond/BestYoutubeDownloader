using System;
using BestYoutubeDownloader.Services.Logging;

namespace BestYoutubeDownloader.Events
{
    public class LogEntryAdded
    {
        public DateTime Time { get; private set; }

        public LogLevel Level { get; private set; }

        public string Source { get; private set; }

        public string Message { get; private set; }

        public Exception? Exception { get; private set; }

        public LogEntryAdded(DateTime time, LogLevel level, string source, string message, Exception? exception)
        {
            this.Time = time;
            this.Level = level;
            this.Source = source;
            this.Message = message;
            this.Exception = exception;
        }
    }
}
