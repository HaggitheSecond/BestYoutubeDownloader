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
    }
}