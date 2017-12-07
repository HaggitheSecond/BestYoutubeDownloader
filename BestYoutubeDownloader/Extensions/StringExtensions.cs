using System;
using System.IO;
using System.Linq;
using System.Text;

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
            return Encoding.UTF8.GetByteCount(self) != self.Length;
        }

        public static bool Contains(this string self, params string[] parameters)
        {
            return parameters.Any(self.ToUpper().Trim().Contains);
        }
    }
}