using System;
using System.IO;
using System.Linq;

namespace BestYoutubeDownloader.Extensions
{
    public static class StringExtensions
    {
        public static bool IsViableUrl(this string self)
        {
            var result = Uri.TryCreate(self, UriKind.Absolute, out Uri uriResult) &&
                         (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);

            return result;
        }

        public static bool IsViableDirectory(this string self)
        {
            return Directory.Exists(self);
        }

        public static bool ContainsNonAscii(this string self)
        {
            foreach (var currentChar in self)
            {
                if (currentChar > 128)
                    return true;
            }

            return false;
        }

        public static bool Contains(this string self, params string[] parameters)
        {
            return parameters.Any(self.ToUpper().Trim().Contains);
        }
    }
}