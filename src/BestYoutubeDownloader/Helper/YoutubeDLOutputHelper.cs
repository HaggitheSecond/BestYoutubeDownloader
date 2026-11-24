using System;
using System.Collections.Generic;
using System.Globalization;
using BestYoutubeDownloader.Views.Pages.DownloadList;

namespace BestYoutubeDownloader.Helper
{
    public static class YoutubeDlOutputHelper
    {
        private const string ProgressPrefix = "[byd-progress] ";
        private const string PostProcessPrefix = "[byd-postprocess] ";
        private const string FilePathPrefix = "[byd-file] ";
        private const string DownloadedPrefix = "[byd-downloaded] ";

        private const char Separator = '|';

        private const string ExtractAudioPostProcessor = "ExtractAudio";
        private const string PostProcessorStarted = "started";
        private const string DownloadedValue = "True";

        public static IList<string> GetOutputArguments()
        {
            return new List<string>
            {
                "--no-quiet",
                "--newline",
                "--progress-template",
                "download:" + ProgressPrefix + "%(progress.downloaded_bytes)s|%(progress.total_bytes)s|%(progress.total_bytes_estimate)s|%(progress.speed)s",
                "--progress-template",
                "postprocess:" + PostProcessPrefix + "%(progress.postprocessor)s|%(progress.status)s",
                "--print",
                "after_move:" + FilePathPrefix + "%(filepath)s",
                "--print",
                "after_move:" + DownloadedPrefix + "%(__real_download)s"
            };
        }

        public static bool TryReadFilePath(string input, out string result)
        {
            result = string.Empty;

            if (input.StartsWith(FilePathPrefix) == false)
                return false;

            result = input.Substring(FilePathPrefix.Length);

            return true;
        }

        public static bool TryReadAlreadyDownloaded(string input, out bool result)
        {
            result = false;

            if (input.StartsWith(DownloadedPrefix) == false)
                return false;

            result = input.Substring(DownloadedPrefix.Length) != DownloadedValue;

            return true;
        }

        public static bool TryReadDownloadStatus(string input, out DownloadStatus result)
        {
            result = new DownloadStatus();

            if (input.StartsWith(PostProcessPrefix))
            {
                var postProcessParts = input.Substring(PostProcessPrefix.Length).Split(Separator);

                if (postProcessParts.Length != 2 || postProcessParts[0] != ExtractAudioPostProcessor || postProcessParts[1] != PostProcessorStarted)
                    return false;

                result.IsExtractingAudio = true;

                return true;
            }

            if (input.StartsWith(ProgressPrefix) == false)
                return false;

            var parts = input.Substring(ProgressPrefix.Length).Split(Separator);

            if (parts.Length != 4)
                return false;

            var downloadedBytes = ReadNumber(parts[0]);
            var totalBytes = ReadNumber(parts[1]) ?? ReadNumber(parts[2]);
            var bytesPerSecond = ReadNumber(parts[3]);

            if (downloadedBytes.HasValue && totalBytes.HasValue && totalBytes.Value > 0)
                result.PercentDone = Math.Round((decimal)(downloadedBytes.Value / totalBytes.Value * 100), 1);

            if (totalBytes.HasValue)
                result.TotalSize = FormatBytes(totalBytes.Value);

            if (bytesPerSecond.HasValue)
                result.CurrentDownloadSpeed = FormatBytes(bytesPerSecond.Value) + "/s";

            return true;
        }

        private static double? ReadNumber(string input)
        {
            if (double.TryParse(input, NumberStyles.Float, CultureInfo.InvariantCulture, out var result))
                return result;

            return null;
        }

        private static string FormatBytes(double bytes)
        {
            var units = new[] { "B", "KiB", "MiB", "GiB" };
            var unitIndex = 0;

            while (bytes >= 1024 && unitIndex < units.Length - 1)
            {
                bytes /= 1024;
                unitIndex++;
            }

            return bytes.ToString("0.00", CultureInfo.InvariantCulture) + units[unitIndex];
        }
    }
}
