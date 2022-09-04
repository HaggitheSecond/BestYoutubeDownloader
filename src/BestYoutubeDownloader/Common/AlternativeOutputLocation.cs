namespace BestYoutubeDownloader.Common
{
    public class AlternativeOutputLocation
    {
        public string Url { get; set; }
        public string Location { get; set; }

        public AlternativeOutputLocation()
        {

        }

        public AlternativeOutputLocation(string url, string location)
        {
            this.Url = url;
            this.Location = location;
        }
    }
}