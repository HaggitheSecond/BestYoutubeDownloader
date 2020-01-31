using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Caliburn.Micro;

namespace BestYoutubeDownloader.Extensions
{
    public static class PropertyChangedExtensions
    {
        public static bool SetProperty<T>(this PropertyChangedBase self,ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;

            field = value;

            self.NotifyOfPropertyChange(propertyName);
            return true;
        }
    }
}