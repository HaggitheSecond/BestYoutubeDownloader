using BestYoutubeDownloader.Common;
using BestYoutubeDownloader.Helper;

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
            TagLibHelper.TagMp3Cover(filePath, imageFilePath);

        }
    }
}