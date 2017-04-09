using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using BestYoutubeDownloader.Common;
using BestYoutubeDownloader.Extensions;
using BestYoutubeDownloader.Helper;
using BestYoutubeDownloader.Services.MetaDataTag;
using BestYoutubeDownloader.Services.YoutubeDL;
using Caliburn.Micro;

namespace BestYoutubeDownloader.Views.EditMetaData
{
    public class EditMetaDataViewModel : Screen
    {
        private readonly IYoutubeDownloaderService _downloaderService;
        private readonly IMetaDataTagService _metaDataTagService;

        private bool _hasChanges;

        private Mp3MetaData _mp3MetaData;
        private MetaData _metaData;

        private string _filePath;
        private string _url;

        private string _title;
        private string _artist;

        private ImageSource _image;

        public Mp3MetaData Mp3MetaData
        {
            get { return this._mp3MetaData; }
            set { this.SetProperty(ref this._mp3MetaData, value); }
        }

        public MetaData MetaData
        {
            get { return this._metaData; }
            set { this.SetProperty(ref this._metaData, value); }
        }

        public string FilePath
        {
            get { return this._filePath; }
            set { this.SetProperty(ref this._filePath, value); }
        }

        public string Url
        {
            get { return this._url; }
            set { this.SetProperty(ref this._url, value); }
        }

        public string Title
        {
            get { return this._title; }
            set
            {
                this.SetProperty(ref this._title, value);

                if (this.Mp3MetaData == null)
                    return;

                this.Mp3MetaData.Title = value;
            }
        }

        public string Artist
        {
            get { return this._artist; }
            set
            {
                this.SetProperty(ref this._artist, value);

                if (this.Mp3MetaData == null)
                    return;

                this.Mp3MetaData.Artist = value;

            }
        }

        public ImageSource Image
        {
            get { return this._image; }
            set { this.SetProperty(ref this._image, value); }
        }

        public BestCommand SaveCommand { get; }

        public BestAsyncCommand LoadCoverImageCommand { get; }

        public  BestCommand OpenDirectoryCommand { get; }
        
        public EditMetaDataViewModel(IYoutubeDownloaderService downloaderService, IMetaDataTagService metaDataTagService)
        {
            this.DisplayName = "Edit metadata";

            this._downloaderService = downloaderService;
            this._metaDataTagService = metaDataTagService;

            this.SaveCommand = new BestCommand(this.Save, this._hasChanges);
            this.LoadCoverImageCommand = new BestAsyncCommand(this.LoadCoverImage, this.CanLoadCover);

            this.OpenDirectoryCommand = new BestCommand(this.OpenDirectory, this.CanOpenDirectory);

            this.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == nameof(this._hasChanges)
                || args.PropertyName == nameof(this.Parent)
                || args.PropertyName == nameof(this.IsInitialized)
                || args.PropertyName == nameof(this.IsActive))
                    return;

                this._hasChanges = true;
            };
        }

        private bool CanOpenDirectory()
        {
            return this.Image != null;
        }

        private void OpenDirectory()
        {
            var bitmapImage = this.Image as BitmapImage;

            var path = bitmapImage?.UriSource.LocalPath;

            var directory = Path.GetDirectoryName(path);

            if(directory == null)
                return;

            Process.Start(directory);
        }

        private bool CanLoadCover()
        {
            return this.Image == null;
        }

        private async Task LoadCoverImage()
        {
            var result = await this._downloaderService.GetThumbNail(this.Url);

            if (result == null)
                return;

            this.Image = result;
        }

        public void Initialize(Mp3MetaData mp3MetaData, MetaData metaData, string filePath, string url, ImageSource image = null)
        {
            if(mp3MetaData == null)
                mp3MetaData = new Mp3MetaData();

            if(metaData == null)
                metaData = new MetaData();

            this.Mp3MetaData = mp3MetaData;

            this.Title = mp3MetaData.Title;
            this.Artist = mp3MetaData.Artist;

            this.MetaData = metaData;

            this.FilePath = filePath;
            this.Url = url;

            if (image != null)
                this.Image = image;
        }

        private void Save()
        {
            this.Mp3MetaData.Artist = this.Artist;
            this.Mp3MetaData.Title = this.Title;

            this._metaDataTagService.TagMetaData(this._filePath, this.Mp3MetaData);

            if (this.Image != null)
            {
                var bitmapImage = this.Image as BitmapImage;

                var path = bitmapImage?.UriSource.LocalPath;

                if(path == null)
                    return;

                if(File.Exists(path) == false)
                    return;
                
                this._metaDataTagService.TagCoverImage(this.FilePath, path);
            }

            this.TryClose(true);
        }
    }
}