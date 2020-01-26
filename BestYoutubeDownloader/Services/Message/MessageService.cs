using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace BestYoutubeDownloader.Services.Message
{
    public class MessageService : IMessageService
    {
        public MessageBoxResult Show(string text, string caption, MessageBoxButton buttons, MessageBoxImage image)
        {
            return MessageBox.Show(text, caption, buttons, image);
        }
    }
}
