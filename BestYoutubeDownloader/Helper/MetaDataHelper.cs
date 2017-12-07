using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using BestYoutubeDownloader.Common;
using BestYoutubeDownloader.Extensions;

namespace BestYoutubeDownloader.Helper
{
    public static class MetaDataHelper
    {
        public static Mp3MetaData GetTitleAndArtist(string input)
        {
            var needsCheck = false;
            var reasons = new List<string>();

            var artist = string.Empty;
            var title = string.Empty;

            var strings = input.Split('-', '|', ':');

            switch (strings.Length)
            {
                case 0:

                case 1:

                    title = strings[0];
                    needsCheck = true;
                    reasons.Add("Unable to split properly");
                    break;
                case 2:

                    artist = strings[0];
                    title = strings[1];

                    break;
                default:
                    artist = strings[0];
                    for (var i = 1; i < strings.Length; i++)
                    {
                        title += strings[i];
                    }

                    needsCheck = true;
                    reasons.Add("Unable to split properly");
                    break;
            }

            title = title.Trim();
            artist = artist.Trim();

            if (ContainsWhitelistedWords(title))
            {
                reasons.Add("Title contains whitelisted text");
                needsCheck = true;
            }

            if (ContainsWhitelistedWords(artist))
            {
                reasons.Add("Artist contains whitelisted text");
                needsCheck = true;
            }

            return new Mp3MetaData
            {
                Artist = artist,
                Title = title,
                NeedCheck = needsCheck,
                CheckReason = string.Join(Environment.NewLine, reasons)
            };

            bool ContainsWhitelistedWords(string text)
            {
                return text.ToUpper().Trim().Contains("Lyrics", ")", "(", "[", "]", "Original", "HD");
            }
        }
    }
}