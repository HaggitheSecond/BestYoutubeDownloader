using Caliburn.Micro;

namespace BestYoutubeDownloader.Views.SupportedSites
{
    public class SupportedSiteViewModel : PropertyChangedBase
    {
        private string _name;
        private string _description;
        private bool _isBroken;

        public string Name
        {
            get { return this._name; }
            set { this.Set(ref this._name, value); }
        }

        public string Description
        {
            get { return this._description; }
            set { this.Set(ref this._description, value); }
        }

        public bool IsBroken
        {
            get { return this._isBroken; }
            set { this.Set(ref this._isBroken, value); }
        }

        public SupportedSiteViewModel(string name, string description)
        {
            this.Name = name;
            this.Description = description;

            this.IsBroken = description?.Contains("Currently broken") ?? false;
        }
    }
}