using System;
using System.Runtime.CompilerServices;

namespace BestYoutubeDownloader.Services.Logging
{
    public interface ILogService
    {
        void Debug(string message, [CallerFilePath] string callerFilePath = "");

        void Info(string message, [CallerFilePath] string callerFilePath = "");

        void Warn(string message, Exception? exception = null, [CallerFilePath] string callerFilePath = "");

        void Error(string message, Exception? exception = null, [CallerFilePath] string callerFilePath = "");

        void SetFileLogLevel(LogLevel level);
    }
}
