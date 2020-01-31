using System;
using System.Globalization;
using System.Windows.Data;

namespace BestYoutubeDownloader.Converter
{
    public class NullTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (string.IsNullOrWhiteSpace((string) value) == false)
                return value;

            return (string) parameter;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}