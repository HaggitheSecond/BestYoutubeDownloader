using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using BestYoutubeDownloader.Common;
using BestYoutubeDownloader.Events;
using BestYoutubeDownloader.Extensions;
using BestYoutubeDownloader.Services.Settings;
using BestYoutubeDownloader.Services.Storage;
using BestYoutubeDownloader.Services.YoutubeDL;
using BestYoutubeDownloader.Views.Pages.Settings.AlternativeOutputLocations;
using Caliburn.Micro;
using DevExpress.Mvvm.UI;
using Screen = Caliburn.Micro.Screen;

namespace BestYoutubeDownloader.Views.Pages.Settings
{
    public class SettingsViewModel : Screen, IPage
    {
        public string Name => "Settings";
        public ImageSource Icon => new BitmapImage(new Uri("pack://application:,,,/Resources/Icons/Settings-64.png"));
        public SolidColorBrush Color => new SolidColorBrush(Colors.Aquamarine);

        private readonly ISettingsService _settingsService;
        private readonly IEventAggregator _eventAggregator;
        private readonly IYoutubeDownloaderService _downloaderService;
        private readonly IWindowManager _windowManager;

        private bool _hasChanges;

        private string _outputDirectoryPath;

        private bool _extractAudio;
        private bool _tagAudio;
        private bool _tagCoverImage;
        private bool _adjustFileName;

        private BindableCollection<string> _availableAudioFormats;
        private string _selectedAudioFormat;

        private bool _showConsole;

        private string _youtubeDlVersion;

        private BindableCollection<AlternativeOutputLocationViewModel> _alternativeOutputLocations;
        private AlternativeOutputLocationViewModel _selectedAlternativeOutputLocation;

        public string OutputDirectoryPath
        {
            get { return this._outputDirectoryPath; }
            set { this.SetProperty(ref this._outputDirectoryPath, value); }
        }

        public bool ExtractAudio
        {
            get { return this._extractAudio; }
            set { this.SetProperty(ref this._extractAudio, value); }
        }

        public BindableCollection<string> AvailableAudioFormats
        {
            get { return this._availableAudioFormats; }
            set { this.SetProperty(ref this._availableAudioFormats, value); }
        }

        public string SelectedAudioFormat
        {
            get { return this._selectedAudioFormat; }
            set { this.SetProperty(ref this._selectedAudioFormat, value); }
        }

        public bool TagAudio
        {
            get { return this._tagAudio; }
            set { this.SetProperty(ref this._tagAudio, value); }
        }

        public bool TagCoverImage
        {
            get { return this._tagCoverImage; }
            set { this.SetProperty(ref this._tagCoverImage, value); }
        }

        public bool AdjustFileName
        {
            get { return this._adjustFileName; }
            set { this.SetProperty(ref this._adjustFileName, value); }
        }

        public bool ShowConsole
        {
            get { return this._showConsole; }
            set { this.SetProperty(ref this._showConsole, value); }
        }

        public string YoutubeDlVersion
        {
            get { return this._youtubeDlVersion; }
            set { this.Set(ref this._youtubeDlVersion, value); }
        }

        public BindableCollection<AlternativeOutputLocationViewModel> AlternativeOutputLocations
        {
            get { return this._alternativeOutputLocations; }
            set { this.Set(ref this._alternativeOutputLocations, value); }
        }

        public AlternativeOutputLocationViewModel SelectedAlternativeOutputLocation
        {
            get { return this._selectedAlternativeOutputLocation; }
            set { this.Set(ref this._selectedAlternativeOutputLocation, value); }
        }

        public BestCommand ChangeDirectoryCommand { get; }

        public BestAsyncCommand SaveCommand { get; }

        public BestAsyncCommand UpdateYoutubeDlVersionCommand { get; }

        public BestAsyncCommand NewAlternativeOutputLocationCommand { get; }
        public BestAsyncCommand EditAlternativeOutputLocationCommand { get; }
        public BestCommand DeleteAlternativeOutputLocationCommand { get; }

        public SettingsViewModel(ISettingsService settingsService, IEventAggregator eventAggregator, IYoutubeDownloaderService downloaderService, IWindowManager windowManager)
        {
            this._settingsService = settingsService;
            this._eventAggregator = eventAggregator;
            this._downloaderService = downloaderService;
            this._windowManager = windowManager;

            this.ChangeDirectoryCommand = new BestCommand(this.ChangeDirectory);
            this.SaveCommand = new BestAsyncCommand(this.Save, this.CanSave);
            this.UpdateYoutubeDlVersionCommand = new BestAsyncCommand(this.UpdateYoutubeDlVersion);

            this.NewAlternativeOutputLocationCommand = new BestAsyncCommand(this.NewAlternativeOutputLocation);
            this.EditAlternativeOutputLocationCommand = new BestAsyncCommand(this.EditAlternativeOutputLocation, () => this.SelectedAlternativeOutputLocation is not null);
            this.DeleteAlternativeOutputLocationCommand = new BestCommand(this.DeleteAlternativeOutputLocation, () => this.SelectedAlternativeOutputLocation is not null);

            this.AvailableAudioFormats = new BindableCollection<string>(Enum.GetNames(typeof(FileFormats)));

            this.LoadSettings();

            this.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == nameof(this._hasChanges)
                || args.PropertyName == nameof(this.Parent)
                || args.PropertyName == nameof(this.IsInitialized)
                || args.PropertyName == nameof(this.IsActive)
                || args.PropertyName == nameof(this.YoutubeDlVersion))
                    return;

                this._hasChanges = true;
            };
        }

        protected override async Task OnActivateAsync(CancellationToken cancellationToken)
        {
            await this.LoadYoutubeDlVersion();
        }

        private void LoadSettings()
        {
            var settings = this._settingsService.GetDownloadSettings();

            this.OutputDirectoryPath = settings.OutputLocation;
            this.ExtractAudio = settings.ExtractAudio;
            this.TagAudio = settings.TagAudio;
            this.ShowConsole = settings.ShowConsole;
            this.TagCoverImage = settings.TagCoverImage;
            this.AdjustFileName = settings.AdjustFileName;

            this.SelectedAudioFormat = this.AvailableAudioFormats.FirstOrDefault(f => f == settings.AudioFormat.ToString()) ?? this.AvailableAudioFormats.First();

            this.AlternativeOutputLocations = new BindableCollection<AlternativeOutputLocationViewModel>(settings.AlternativeOutputLocations.Select(f => new AlternativeOutputLocationViewModel(f)));
        }

        private void ChangeDirectory()
        {
            var dialog = new FolderBrowserDialog();

            if (dialog.ShowDialog() != DialogResult.OK)
                return;

            this.OutputDirectoryPath = dialog.SelectedPath;
        }

        private bool CanSave()
        {
            return this._hasChanges;
        }

        private async Task Save()
        {
            if (Enum.TryParse(this.SelectedAudioFormat, out FileFormats format) == false)
                return;

            var settings = new DownloadSettings
            {
                OutputLocation = this.OutputDirectoryPath,
                ExtractAudio = this.ExtractAudio,
                AudioFormat = format,
                TagAudio = this.TagAudio,
                ShowConsole = this.ShowConsole,
                TagCoverImage = this.TagCoverImage,
                AdjustFileName = this.AdjustFileName,
                AlternativeOutputLocations = this.AlternativeOutputLocations.Select(f => new AlternativeOutputLocation(f.Url, f.Location)).ToList()
            };

            this._settingsService.UpdateDownloadSettings(settings);

            await this._eventAggregator.PublishOnUIThreadAsync(new SettingsChanged(settings));

            this._hasChanges = false;
        }

        private async Task UpdateYoutubeDlVersion()
        {
            await this._downloaderService.UpdateYoutubeDl();
            await this.LoadYoutubeDlVersion();
        }

        private async Task LoadYoutubeDlVersion()
        {
            this.YoutubeDlVersion = await this._downloaderService.GetYoutubeDlVersion();
        }

        private async Task NewAlternativeOutputLocation()
        {
            var viewModel = new AlternativeOutputLocationViewModel();

            this.AlternativeOutputLocations.Add(viewModel);
            this.SelectedAlternativeOutputLocation = viewModel;

            await this.EditAlternativeOutputLocationCommand.ExecuteAsync(null);
        }

        private void DeleteAlternativeOutputLocation()
        {
            this.AlternativeOutputLocations.Remove(this.SelectedAlternativeOutputLocation);
        }

        private async Task EditAlternativeOutputLocation()
        {
            await this._windowManager.ShowDialogAsync(new EditAlternativeOutputLocationViewModel(this.SelectedAlternativeOutputLocation), null, WindowSettings.GetWindowSettings(400, 150));
        }
    }
}