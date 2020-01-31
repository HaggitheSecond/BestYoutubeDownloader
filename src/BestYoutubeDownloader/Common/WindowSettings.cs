using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media.Imaging;

namespace BestYoutubeDownloader.Common
{
    public static class WindowSettings
    {
        public static IDictionary<string, object> GetMainWindowSettings()
        {
            return new Dictionary<string, object>
            {
                {nameof(Window.Width), 850},
                {nameof(Window.Height), 500},
                {nameof(WindowStartupLocation), WindowStartupLocation.CenterScreen},
                {nameof(Window.SizeToContent), SizeToContent.Manual},
                {nameof(Window.Icon), new BitmapImage(new Uri("pack://application:,,,/Resources/Icons/Download-48.png"))}
            };
        }

        public static IDictionary<string, object> GetWindowSettings(int width , int height)
        {
            return new Dictionary<string, object>
            {
                {nameof(Window.Width), width},
                {nameof(Window.Height), height},
                {nameof(WindowStartupLocation), WindowStartupLocation.CenterScreen},
                {nameof(Window.SizeToContent), SizeToContent.Manual},
                {nameof(Window.Icon), new BitmapImage(new Uri("pack://application:,,,/Resources/Icons/Download-48.png"))}
            };
        }
    }
}