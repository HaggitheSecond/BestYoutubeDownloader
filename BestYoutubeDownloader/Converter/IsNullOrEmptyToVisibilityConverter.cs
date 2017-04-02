using System;
using System.Collections;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace BestYoutubeDownloader.Converter
{
    public class IsNullOrEmptyToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value == null)
                return Visibility.Hidden;

            if (value is ICollection)
            {
                var valueCollection = value as ICollection;

                if (valueCollection.Count == 0)
                    return Visibility.Hidden;
            }

            return Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}