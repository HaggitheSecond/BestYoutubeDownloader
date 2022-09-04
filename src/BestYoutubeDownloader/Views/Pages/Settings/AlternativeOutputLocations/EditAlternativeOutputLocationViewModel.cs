using BestYoutubeDownloader.Common;
using BestYoutubeDownloader.Services.Settings;
using BestYoutubeDownloader.Views.SupportedSites;
using Caliburn.Micro;
using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using Screen = Caliburn.Micro.Screen;

namespace BestYoutubeDownloader.Views.Pages.Settings.AlternativeOutputLocations;
public class EditAlternativeOutputLocationViewModel : Screen
{
    private AlternativeOutputLocationViewModel _alternativeOutputLocationViewModel;

    private string _url;
    private string _location;

    public string Url
    {
        get { return this._url; }
        set { this.Set(ref this._url, value); }
    }

    public string Location
    {
        get { return this._location; }
        set { this.Set(ref this._location, value); }
    }

    public BestAsyncCommand OkCommand { get; }
    public BestAsyncCommand CancelCommand { get; }
    public BestCommand SelectDirectoryCommand { get; }
    public BestAsyncCommand UseSupportedSitesCommand { get; }

    public EditAlternativeOutputLocationViewModel(AlternativeOutputLocationViewModel alternativeOutputLocationViewModel)
    {
        this._alternativeOutputLocationViewModel = alternativeOutputLocationViewModel;
        this.Url = alternativeOutputLocationViewModel.Url;
        this.Location = alternativeOutputLocationViewModel.Location;

        this.DisplayName = "Alternative output directory";

        this.OkCommand = new BestAsyncCommand(async () =>
        {
            this._alternativeOutputLocationViewModel.Url = this.Url;
            this._alternativeOutputLocationViewModel.Location = this.Location;

            await this.TryCloseAsync(true);
        });
        this.CancelCommand = new BestAsyncCommand(async () => await this.TryCloseAsync(false));

        this.SelectDirectoryCommand = new BestCommand(this.SelectFolder);
        this.UseSupportedSitesCommand = new BestAsyncCommand(this.UseSupportedSites);

    }

    private async Task UseSupportedSites()
    {
        var viewModel = IoC.Get<SupportedSitesViewModel>();

        var result = await IoC.Get<IWindowManager>().ShowDialogAsync(viewModel, null, WindowSettings.GetWindowSettings(400, 600));

        if (result.GetValueOrDefault() is false || string.IsNullOrWhiteSpace(viewModel.SelectedItem))
            return;

        this.Url = viewModel.SelectedItem;
    }

    private void SelectFolder()
    {
        var dialog = new FolderBrowserDialog();

        dialog.InitialDirectory = IoC.Get<ISettingsService>().GetDownloadSettings().OutputLocation;

        if (dialog.ShowDialog() != DialogResult.OK)
            return;

        this.Location = dialog.SelectedPath;
    }
}
