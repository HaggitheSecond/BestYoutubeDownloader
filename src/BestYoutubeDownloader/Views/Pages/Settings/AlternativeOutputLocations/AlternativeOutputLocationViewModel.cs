using BestYoutubeDownloader.Common;
using Caliburn.Micro;

namespace BestYoutubeDownloader.Views.Pages.Settings.AlternativeOutputLocations
{
    public class AlternativeOutputLocationViewModel : PropertyChangedBase
    {
        private string _url;
        private string _location;

        public string Url
        {
            get { return _url; }
            set { Set(ref _url, value); }
        }

        public string Location
        {
            get { return _location; }
            set { Set(ref _location, value); }
        }

        public AlternativeOutputLocationViewModel()
        {

        }

        public AlternativeOutputLocationViewModel(AlternativeOutputLocation location)
             : this()
        {
            this.Url = location.Url;
            this.Location = location.Location;
        }
    }
}