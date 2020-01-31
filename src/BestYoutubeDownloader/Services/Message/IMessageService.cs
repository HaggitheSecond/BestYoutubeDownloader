using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace BestYoutubeDownloader.Services.Message
{
    public interface IMessageService
    {
        MessageBoxResult Show(string caption, string content, MessageBoxButton buttons, MessageBoxImage image);
    }
}
