using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using BestYoutubeDownloader.Common;
using BestYoutubeDownloader.Events;
using BestYoutubeDownloader.Extensions;
using BestYoutubeDownloader.Helper;
using BestYoutubeDownloader.Services.Logging;
using BestYoutubeDownloader.Services.Settings;
using BestYoutubeDownloader.Services.Download;
using Caliburn.Micro;
using DevExpress.Mvvm;

namespace BestYoutubeDownloader.Views.Pages.RawConsole
{
    public class RawConsoleViewModel : Screen, IPage, IHandle<LogEntryAdded>
    {
        public string Name => "Console";
        public ImageSource Icon => new BitmapImage(new Uri("pack://application:,,,/Resources/Icons/Console-64.png"));
        public SolidColorBrush Color => new SolidColorBrush(Colors.Gray);

        private ObservableCollection<RawConsoleOutputViewModel> _outputs;

        public ObservableCollection<RawConsoleOutputViewModel> Outputs
        {
            get { return this._outputs; }
            set { this.SetProperty(ref this._outputs, value); }
        }

        public BestCommand ClearConsoleCommand { get; }

        public BestCommand OpenConsoleCommand { get; }

        public RawConsoleViewModel(IEventAggregator eventAggregator)
        {
            eventAggregator.SubscribeOnPublishedThread(this);

            this.ClearConsoleCommand = new BestCommand(() => { this.Outputs.Clear(); }, () => this.Outputs.Count != 0);
            this.OpenConsoleCommand = new BestCommand(this.OpenConsole);

            this.Outputs = new ObservableCollection<RawConsoleOutputViewModel>();
        }

        private void OpenConsole()
        {
            ProcessHelper.OpenCommandPrompt(AppContext.BaseDirectory);
        }

        public Task HandleAsync(LogEntryAdded message, CancellationToken cancellationToken)
        {
            this.AddOutput(message);

            return Task.CompletedTask;
        }

        private void AddOutput(LogEntryAdded entry)
        {
            var s = this.GetOutputText(entry);
            var isInput = entry.Level != LogLevel.Debug;

            // this can happen when a command is not done executing when the application gets shut down
            if (Application.Current?.Dispatcher == null)
                return;

            Application.Current.Dispatcher.Invoke(() =>
            {
                if (this.Outputs.Count > 200)
                    this.Outputs.RemoveAt(0);

                if (string.IsNullOrEmpty(s))
                    this.Outputs.Add(new RawConsoleOutputViewModel());
                else
                    this.Outputs.Add(new RawConsoleOutputViewModel
                    {
                        Value = s,
                        Time = entry.Time,
                        IsInput = isInput
                    });
            });
        }

        private string GetOutputText(LogEntryAdded entry)
        {
            var text = entry.Message;

            if (entry.Level == LogLevel.Warn && text.StartsWith("WARNING") == false)
                text = "WARNING: " + text;

            if (entry.Level == LogLevel.Error && text.StartsWith("ERROR") == false)
                text = "ERROR: " + text;

            if (entry.Exception is not null)
                text += " (" + entry.Exception.Message + ")";

            return text;
        }
    }
}