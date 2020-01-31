using BestYoutubeDownloader.Common;

namespace BestYoutubeDownloader.Services.MetaDataTag
{
    public interface IMetaDataTagService
    {
        void TagMetaData(string filePath, Mp3MetaData metaData);

        void TagCoverImage(string filePath, string imageFilePath);
    }
}