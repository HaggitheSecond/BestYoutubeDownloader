using System;
using System.IO;

namespace BestYoutubeDownloader.Extensions
{
    public static class StringExtensions
    {
        public static bool IsViableUrl(this string self)
        {
            var result = Uri.TryCreate(self, UriKind.Absolute, out Uri uriResult) && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);

            return result;
        }

        public static bool IsViableDirectory(this string self)
        {
            return Directory.Exists(self);
        }
    }
}