using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace BestYoutubeDownloader.Converter
{
    public class BooleanToVisibilityConverter : IValueConverter
    {
        public Visibility TrueVisibility { get; set; } = Visibility.Visible;

        public Visibility FalseVisibility { get; set;  } = Visibility.Collapsed;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return this.FalseVisibility;

            if (!(value is bool booleanValue))
                throw new ArgumentException();

            return booleanValue ? this.TrueVisibility : this.FalseVisibility;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}