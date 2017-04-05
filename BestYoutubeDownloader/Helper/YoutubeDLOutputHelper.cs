using System.Linq;

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
    }
}