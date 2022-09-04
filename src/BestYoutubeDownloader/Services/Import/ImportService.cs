using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mime;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using BestYoutubeDownloader.Extensions;
using BestYoutubeDownloader.Services.ExceptionHandling;
using BestYoutubeDownloader.Views.Pages.DownloadList;
using Caliburn.Micro;

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

        public async Task<IList<(string name, string description)>> GetSupportedSites()
        {
            var sites = new List<(string name, string description)>();
            var url = @"https://raw.githubusercontent.com/yt-dlp/yt-dlp/master/supportedsites.md";

            using (var client = new HttpClient())
            {
                try
                {
                    var response = await client.GetStringAsync(url);

                    sites.AddRange(response
                        .Split(Environment.NewLine.ToCharArray())
                        .Skip(1)
                        .Where(f => string.IsNullOrWhiteSpace(f) == false)
                        .Select(f =>
                        {
                            var parts = f.Split("**", StringSplitOptions.RemoveEmptyEntries).Skip(1).ToArray();

                            var name = parts[0];
                            var description = parts.Length == 1
                                ? string.Empty
                                : GetDescription(string.Join("", parts[1..]));

                            return (name, description);
                        }));
                }
                catch (Exception e)
                {
                    IoC.Get<IExceptionHandler>().Handle(e);
                }

                string GetDescription(string parts)
                {
                    return parts
                        .TrimStart(':')
                        .Replace("[<abbr title=\"netrc machine\"><em>", "")
                        .Replace("</em></abbr>]", "")
                        .Trim();
                }
            }

            return sites;
        }
    }
}