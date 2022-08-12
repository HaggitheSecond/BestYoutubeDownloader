using System;
using System.Linq;
using BestYoutubeDownloader.Views.Pages.DownloadList;

namespace BestYoutubeDownloader.Helper
{
    public static class YoutubeDlOutputHelper
    {
        public static bool TryGetFilePath(string input, out string result)
        {
            var splitter = "SPLITHERE";

            result = string.Empty;

            input = input.Replace("Merging formats into", splitter);
            input = input.Replace("Destination", splitter);


            if (input.Contains(splitter) == false)
                return false;

            var inputParts = input.Split(' ').ToList();

            if (inputParts.Count < 3)
                return false;

            // remove "[download]"/"[ffmepg]" and "Destination"
            inputParts.RemoveAt(0);
            inputParts.RemoveAt(0);

            var tempResult = string.Join(" ", inputParts);

            result = tempResult.Replace('"', ' ');

            return true;
        }

        public static bool HasBeenDownloaded(string input)
        {
            return input.Contains("has already been downloaded");
        }

        public static bool TryReadDownloadStatus(string input, out DownloadStatus result)
        {
            result = new DownloadStatus();

            if (input.Contains("[download]") == false)
                return false;

            if (input.Contains("Destination:") || input.Contains("merged."))
                return false;

            var parts = input.Split(char.Parse(" "));

            if (decimal.TryParse(parts.FirstOrDefault(f => f.Contains("%"))?.Replace("%", "")?.Replace(".", ","), out var decimalPercent))
                result.PercentDone = decimalPercent;

            result.TotalSize = parts.FirstOrDefault(f => f.Contains("KiB") || f.Contains("MiB") || f.Contains("GiB"));

            result.CurrentDownloadSpeed = parts.FirstOrDefault(f => f.Contains("KiB/s") || f.Contains("MiB/s") || f.Contains("GiB/s"));

            return true;
        }

        public static bool IsExtractingAudio(string input)
        {
            return (input.Contains(@"[ffmpeg] Destination:") || input.Contains(@"[ExtractAudio]")) 
                && input.Contains("file is already in target format mp3") == false;
        }
    }
}