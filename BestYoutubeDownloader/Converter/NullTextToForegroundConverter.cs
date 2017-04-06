using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace BestYoutubeDownloader.Converter
{
    public class NullTextToForegroundConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (string.IsNullOrWhiteSpace((string)value) == false)
                return new SolidColorBrush(Colors.Black);

            return new SolidColorBrush(Colors.LightGray);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}