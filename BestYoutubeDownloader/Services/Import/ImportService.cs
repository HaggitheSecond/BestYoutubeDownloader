using System.Collections.Generic;
using System.IO;
using System.Linq;
using BestYoutubeDownloader.Extensions;
using BestYoutubeDownloader.Views.Pages.DownloadList;

namespace BestYoutubeDownloader.Services.Import
{
    public class ImportService : IImportService
    {
        public IList<DownloadItem> ImportDownloadItemsFromFile(string filePath)
        {
            var rawItems = File.ReadAllLines(filePath);

            var items = rawItems.Where(currentItem => currentItem.IsViableUrl() == true).ToList();

            var list = new List<DownloadItem>();

            list.AddRange(items.Select(f => new DownloadItem(f)));

            return list;
        }
    }
}