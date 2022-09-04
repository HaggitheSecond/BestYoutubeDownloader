using BestYoutubeDownloader.Common;
using BestYoutubeDownloader.Services.Settings;
using BestYoutubeDownloader.Views.Pages.Settings.AlternativeOutputLocations;
using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Screen = Caliburn.Micro.Screen;

namespace BestYoutubeDownloader.Views.Pages.DownloadList.OutputLocationSelection;
public class OutputLocationSelectionViewModel : Screen
{
    private string _location;
    private BindableCollection<AlternativeOutputLocationViewModel> _alternativeOutputLocation;
    private AlternativeOutputLocationViewModel _selectedAlternativeOutputLocation;

    public string Location
    {
        get { return this._location; }
        set { this.Set(ref this._location, value); }
    }

    public BindableCollection<AlternativeOutputLocationViewModel> AlternativeOutputLocations
    {
        get { return this._alternativeOutputLocation; }
        set { this.Set(ref this._alternativeOutputLocation, value); }
    }

    public AlternativeOutputLocationViewModel SelectedAlternativeOutputLocation
    {
        get { return this._selectedAlternativeOutputLocation; }
        set
        {
            this.Set(ref this._selectedAlternativeOutputLocation, value);

            if (string.IsNullOrWhiteSpace(value?.Location) == false)
                this.Location = value.Location;
        }
    }

    public BestAsyncCommand OkCommand { get; }
    public BestAsyncCommand CancelCommand { get; }
    public BestCommand SelectDirectoryCommand { get; }

    public OutputLocationSelectionViewModel(ISettingsService settingsService)
    {
        this.OkCommand = new BestAsyncCommand(async () => { await this.TryCloseAsync(true); }, () => string.IsNullOrWhiteSpace(this.Location) == false);
        this.CancelCommand = new BestAsyncCommand(async () => await this.TryCloseAsync(false));

        this.SelectDirectoryCommand = new BestCommand(this.SelectFolder);

        this.AlternativeOutputLocations = new BindableCollection<AlternativeOutputLocationViewModel>(settingsService.GetDownloadSettings().AlternativeOutputLocations.Select(f => new AlternativeOutputLocationViewModel(f)));
    }

    private void SelectFolder()
    {
        var dialog = new FolderBrowserDialog
        {
            InitialDirectory = IoC.Get<ISettingsService>().GetDownloadSettings().OutputLocation
        };

        if (dialog.ShowDialog() != DialogResult.OK)
            return;

        this.Location = dialog.SelectedPath;
    }
}
