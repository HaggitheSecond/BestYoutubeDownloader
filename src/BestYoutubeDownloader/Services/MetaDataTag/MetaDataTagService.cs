using BestYoutubeDownloader.Common;
using BestYoutubeDownloader.Helper;
using System.IO;
using System.Windows.Media.Imaging;

namespace BestYoutubeDownloader.Services.MetaDataTag
{
    public class MetaDataTagService : IMetaDataTagService
    {
        public void TagMetaData(string filePath, Mp3MetaData metaData)
        {
            TagLibHelper.TagMp3(filePath, metaData);
        }

        public void TagCoverImage(string filePath, string imageFilePath)
        {
            if (imageFilePath.EndsWith(".webp"))
            {
                var bitmap = new BitmapImage(new System.Uri(imageFilePath));

                var encoder = new JpegBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(bitmap));

                var directory = Path.GetDirectoryName(imageFilePath);
                var name = Path.GetFileNameWithoutExtension(imageFilePath);
                imageFilePath = Path.Combine(directory, name + ".jpg");

                using (var stream = new FileStream(imageFilePath, FileMode.Create))
                {
                    encoder.Save(stream);
                }
            }

            TagLibHelper.TagMp3Cover(filePath, imageFilePath);

        }
    }
}