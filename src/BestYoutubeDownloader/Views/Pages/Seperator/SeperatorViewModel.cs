using System.Windows.Media;
using BestYoutubeDownloader.Extensions;
using Caliburn.Micro;

namespace BestYoutubeDownloader.Views.Pages.Seperator
{
    public class SeperatorViewModel : Screen, ISeperator
    {
        public string Name => "Does not matter here";
        public ImageSource? Icon => null;
        public SolidColorBrush? Color => null;

        private bool _isEnabled;

        public bool IsEnabled
        {
            get { return this._isEnabled; }
            set { this.SetProperty(ref this._isEnabled, value); }
        }

        public SeperatorViewModel()
        {
            this.IsEnabled = false;
        }
    }
}