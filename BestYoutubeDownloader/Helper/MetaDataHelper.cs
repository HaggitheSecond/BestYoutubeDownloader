using BestYoutubeDownloader.Common;

namespace BestYoutubeDownloader.Helper
{
    public static class MetaDataHelper
    {
        public static Mp3MetaData GetTitleAndArtist(string input)
        {
            var needsCheck = false;
            var artist = string.Empty;
            var title = string.Empty;

            var strings = input.Split('-');

            switch (strings.Length)
            {
                case 0:

                case 1:

                    title = strings[0];
                    needsCheck = true;
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
                    break;
            }

            title = title.Trim();
            artist = artist.Trim();

            return new Mp3MetaData
            {
                Artist = artist,
                Title = title,
                NeedCheck = needsCheck
            };
        }
    }
}