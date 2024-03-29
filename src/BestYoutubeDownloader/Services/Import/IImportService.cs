﻿using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using BestYoutubeDownloader.Views.Pages.DownloadList;

namespace BestYoutubeDownloader.Services.Import
{
    public interface IImportService
    {
        IList<DownloadItem> ImportDownloadItemsFromFile(string filePath);

        Task<IList<(string name, string description)>> GetSupportedSites();
    }
}