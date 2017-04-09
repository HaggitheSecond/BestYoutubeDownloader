using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using BestYoutubeDownloader.Common;
using BestYoutubeDownloader.Extensions;
using BestYoutubeDownloader.Services.Import;
using Caliburn.Micro;
using DevExpress.Mvvm.Native;

namespace BestYoutubeDownloader.Views.SupportedSites
{
    public class SupportedSitesViewModel : Screen
    {
        private List<string> _allItems;
        private BindableCollection<string> _items;

        private string _searchText;

        public BindableCollection<string> Items
        {
            get { return this._items; }
            set { this.SetProperty(ref this._items, value); }
        }

        public string SearchText
        {
            get { return this._searchText; }
            set
            {
                this.SetProperty(ref this._searchText, value);

                this.Items = new BindableCollection<string>(this._allItems.Where(f => f.ToUpper().Contains(this.SearchText.ToUpper())));
            }
        }

        public BestCommand CloseCommand { get; }
        public BestCommand GoToUpdatedListCommand { get; }

        public SupportedSitesViewModel(IImportService importService)
        {
            this.DisplayName = "Supported Sites";

            this.CloseCommand = new BestCommand(() => this.TryClose());

            this.GoToUpdatedListCommand = new BestCommand(() =>
            {
                Process.Start("https://github.com/rg3/youtube-dl/blob/master/docs/supportedsites.md");
            });

            var lines = importService.GetSupportedSitesFromFile();

            if (lines == null)
            {
                this.TryClose();
                return;
            }

            // remove the first line of the file "# Supported sites"
            lines.RemoveAt(0);

            this._allItems = lines.Select(line => line.Replace("-", "").Replace("*", "").Trim()).ToList();
            this.Items = new BindableCollection<string>(this._allItems);
        }
    }
}