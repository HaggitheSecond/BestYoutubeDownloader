using System;
using System.Globalization;
using System.Windows.Data;

namespace BestYoutubeDownloader.Converter
{
    public class TimeSpanToFormatedConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return string.Empty;

            var duration = (TimeSpan) value;
            
            return "(" + duration.ToString(duration.TotalHours > 1 ? @"hh\:mm\:ss" : @"mm\:ss") + ")";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}