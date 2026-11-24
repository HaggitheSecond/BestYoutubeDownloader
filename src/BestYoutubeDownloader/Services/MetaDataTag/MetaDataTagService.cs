using BestYoutubeDownloader.Common;
using BestYoutubeDownloader.Helper;
using BestYoutubeDownloader.Services.Logging;
using System;
using System.IO;
using System.Windows.Media.Imaging;

namespace BestYoutubeDownloader.Services.MetaDataTag
{
    public class MetaDataTagService : IMetaDataTagService
    {
        private readonly ILogService _logService;

        public MetaDataTagService(ILogService logService)
        {
            this._logService = logService;
        }

        public void TagMetaData(string filePath, Mp3MetaData metaData)
        {
            if (File.Exists(filePath) == false)
            {
                this._logService.Warn($"Could not tag '{filePath}' because the file does not exist");
                return;
            }

            this._logService.Info($"Tagging '{filePath}' with title '{metaData.Title}' and artist '{metaData.Artist}'");

            TagLibHelper.TagMp3(filePath, metaData);
        }

        public void TagCoverImage(string filePath, string imageFilePath)
        {
            if (File.Exists(filePath) == false || File.Exists(imageFilePath) == false)
            {
                this._logService.Warn($"Could not tag '{filePath}' with the cover '{imageFilePath}' because one of the files does not exist");
                return;
            }

            this._logService.Info($"Tagging '{filePath}' with the cover '{imageFilePath}'");

            if (imageFilePath.EndsWith(".webp"))
            {
                this._logService.Debug($"Converting the cover '{imageFilePath}' to jpg");

                var bitmap = new BitmapImage(new System.Uri(imageFilePath));

                var encoder = new JpegBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(bitmap));

                var directory = Path.GetDirectoryName(imageFilePath);

                if (string.IsNullOrWhiteSpace(directory))
                    return;

                var name = Path.GetFileNameWithoutExtension(imageFilePath);
                imageFilePath = Path.Combine(directory, name + ".jpg");

                using var stream = new FileStream(imageFilePath, FileMode.Create);

                encoder.Save(stream);
            }

            TagLibHelper.TagMp3Cover(filePath, imageFilePath);
        }
    }
}