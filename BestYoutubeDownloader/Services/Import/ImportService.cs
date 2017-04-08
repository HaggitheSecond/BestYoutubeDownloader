using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Reflection;
using System.Windows;
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

        public IList<string> GetSupportedSitesFromFile()
        {
            var path = Directory.GetCurrentDirectory() + @"\supportedsites.md";

            return File.ReadAllLines(path).ToList();
        }
    }
}