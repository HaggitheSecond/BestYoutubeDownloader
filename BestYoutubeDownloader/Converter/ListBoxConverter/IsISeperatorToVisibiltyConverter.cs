using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using BestYoutubeDownloader.Views.Pages;

namespace BestYoutubeDownloader.Converter
{
    public class IsISeperatorToVisibiltyConverter : IValueConverter
    {
        public bool Inverse { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (this.Inverse)
            {
                if (value is ISeperator)
                    return Visibility.Hidden;

                return Visibility.Visible;
            }
            else
            {
                if (value is ISeperator)
                    return Visibility.Visible;

                return Visibility.Hidden;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}