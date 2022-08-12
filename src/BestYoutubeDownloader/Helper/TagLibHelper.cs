using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using BestYoutubeDownloader.Common;
using Caliburn.Micro;
using TagLib;
using File = System.IO.File;

namespace BestYoutubeDownloader.Helper
{
    public static class TagLibHelper
    {
        public static void TagMp3(string file, Mp3MetaData metaData)
        {
            if (File.Exists(file) == false || metaData == null)
                return;

            var f = TagLib.File.Create(file);
            f.Tag.Title = metaData.Title;
            f.Tag.Performers = new[] { metaData.Artist };
            f.Tag.Comment = metaData.SourceUrl;
            f.Save();
        }

        public static void TagMp3Cover(string file, string imagePath)
        {
            if (File.Exists(imagePath) == false || File.Exists(file) == false)
                return;

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