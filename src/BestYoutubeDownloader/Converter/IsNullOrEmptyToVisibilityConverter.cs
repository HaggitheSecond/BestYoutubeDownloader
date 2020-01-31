using System;
using System.Collections;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace BestYoutubeDownloader.Converter
{
    public class IsNullOrEmptyToVisibilityConverter : IValueConverter
    {
        public bool Inverse { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value == null)
                return this.Inverse 
                    ? Visibility.Visible 
                    : Visibility.Collapsed;

            if (value is ICollection)
            {
                var valueCollection = value as ICollection;

                if (valueCollection.Count == 0)
                    return this.Inverse
                    ? Visibility.Visible
                    : Visibility.Collapsed;
            }

            return this.Inverse
                    ? Visibility.Collapsed
                    : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}