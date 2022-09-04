using System.Security.Cryptography.X509Certificates;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace BestYoutubeDownloader.Views.Pages
{
    public interface IPage
    {
        string Name { get; }

        ImageSource? Icon { get; }

        SolidColorBrush? Color { get; }
    }
}