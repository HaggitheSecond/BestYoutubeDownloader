using System;
using BestYoutubeDownloader.Extensions;
using Caliburn.Micro;

namespace BestYoutubeDownloader.Views.Pages.RawConsole
{
    public class RawConsoleOutputViewModel : PropertyChangedBase
    {
        private string _value;
        private DateTime _time;
        private bool _isInput;

        public string Value
        {
            get { return this._value; }
            set { this.SetProperty(ref this._value, value); }
        }

        public DateTime Time
        {
            get { return this._time; }
            set { this.SetProperty(ref this._time, value); }
        }

        public bool IsInput
        {
            get { return this._isInput; }
            set { this.SetProperty(ref this._isInput, value); }
        }
    }
}