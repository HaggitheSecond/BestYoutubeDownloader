using System.Collections;
using System.Collections.Generic;
using BestYoutubeDownloader.Views.Pages.DownloadList;

namespace BestYoutubeDownloader.Services.Import
{
    public interface IImportService
    {
        IList<DownloadItem> ImportDownloadItemsFromFile(string filePath);

        IList<string> GetSupportedSitesFromFile();
    }
}