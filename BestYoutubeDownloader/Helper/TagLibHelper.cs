using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using BestYoutubeDownloader.Common;
using Caliburn.Micro;
using TagLib;

namespace BestYoutubeDownloader.Helper
{
    public static class TagLibHelper
    {
        public static void TagMp3(string file, Mp3MetaData metaData)
        {
            var f = TagLib.File.Create(file);
            f.Tag.Title = metaData.Title;
            f.Tag.Performers = new[] { metaData.Artist };
            f.Save();
        }

        public static void TagMp3Cover(string file, string imagePath)
        {
            var picture = new TagLib.Picture(imagePath);

            var f = TagLib.File.Create(file);
            f.Tag.Pictures = new TagLib.IPicture[]
            {
                picture
            };
            f.Save();
        }

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