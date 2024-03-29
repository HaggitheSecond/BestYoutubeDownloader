﻿using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BestYoutubeDownloader.Common;
using BestYoutubeDownloader.Extensions;
using BestYoutubeDownloader.Services.Import;
using Caliburn.Micro;
using DevExpress.Mvvm.Native;

namespace BestYoutubeDownloader.Views.SupportedSites
{
    public class SupportedSitesViewModel : Screen
    {
        private List<SupportedSiteViewModel> _allItems;
        private BindableCollection<SupportedSiteViewModel> _items;
        private SupportedSiteViewModel _selectedItem;

        private string _searchText;
        private readonly IImportService _importService;

        public BindableCollection<SupportedSiteViewModel> Items
        {
            get { return this._items; }
            set { this.SetProperty(ref this._items, value); }
        }

        public SupportedSiteViewModel SelectedItem
        {
            get { return this._selectedItem; }
            set { this.Set(ref this._selectedItem, value); }
        }

        public string SearchText
        {
            get { return this._searchText; }
            set
            {
                this.SetProperty(ref this._searchText, value);

                this.Items = new BindableCollection<SupportedSiteViewModel>(this._allItems
                    .Where(f => f.Name.ToUpper().Contains(this.SearchText.ToUpper()) || f.Description.ToUpper().Contains(this.SearchText.ToUpper())));
            }
        }

        public BestAsyncCommand CloseCommand { get; }

        public SupportedSitesViewModel(IImportService importService)
        {
            this._importService = importService;

            this.DisplayName = "Supported Sites";

            this.CloseCommand = new BestAsyncCommand(async () => await this.TryCloseAsync(true));
        }

        protected override async Task OnActivateAsync(CancellationToken cancellationToken)
        {
            var lines = await this._importService.GetSupportedSites();

            if (lines is null)
            {
                await this.TryCloseAsync();
                return;
            }

            this._allItems = lines.Select(f => new SupportedSiteViewModel(f.name, f.description)).ToList();
            this.Items = new BindableCollection<SupportedSiteViewModel>(this._allItems);
        }
    }
}