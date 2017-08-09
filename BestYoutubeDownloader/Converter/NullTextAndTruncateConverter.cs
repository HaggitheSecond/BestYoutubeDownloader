using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace BestYoutubeDownloader.Converter
{
    public class NullTextAndTruncateConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var text = (string)value;

            if (string.IsNullOrWhiteSpace(text))
                return (string)parameter;
            
            var maxLenght = 50;

            if (text.Length > maxLenght)
                return text.Remove(maxLenght, text.Length - maxLenght) + "...";
            else
                return value;

        }

        public object ConvertBack(object value, Type targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}