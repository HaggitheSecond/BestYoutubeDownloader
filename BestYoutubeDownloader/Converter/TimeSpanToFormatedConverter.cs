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

            var formatedDuration = string.Empty;

            if(duration.TotalHours > 1)
                formatedDuration = duration.ToString(@"h\:m\:s");
            else
                formatedDuration = duration.ToString(@"m\:s");

            return "(" + formatedDuration + ")";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}