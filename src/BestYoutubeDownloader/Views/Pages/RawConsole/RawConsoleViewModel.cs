using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using BestYoutubeDownloader.Common;
using BestYoutubeDownloader.Extensions;
using BestYoutubeDownloader.Services.CommandPrompt;
using BestYoutubeDownloader.Services.Settings;
using BestYoutubeDownloader.Services.YoutubeDL;
using Caliburn.Micro;
using DevExpress.Mvvm;

namespace BestYoutubeDownloader.Views.Pages.RawConsole
{
    public class RawConsoleViewModel : Screen, IPage
    {
        public string Name => "Console";
        public ImageSource Icon => new BitmapImage(new Uri("pack://application:,,,/Resources/Icons/Console-64.png"));
        public SolidColorBrush Color => new SolidColorBrush(Colors.Gray);

        private readonly ICommandPromptService _commandPromptService;
        private ObservableCollection<RawConsoleOutputViewModel> _outputs;

        public ObservableCollection<RawConsoleOutputViewModel> Outputs
        {
            get { return this._outputs; }
            set { this.SetProperty(ref this._outputs, value); }
        }

        public BestCommand ClearConsoleCommand { get; }

        public BestCommand OpenConsoleCommand { get; }

        public RawConsoleViewModel(ICommandPromptService commandPromptService)
        {
            this._commandPromptService = commandPromptService;

            this._commandPromptService.RegisterOutputAction(this.AddOutput);

            this.ClearConsoleCommand = new BestCommand(() => { this.Outputs.Clear(); }, () => this.Outputs.Count != 0);
            this.OpenConsoleCommand = new BestCommand(this.OpenConsole);

            this.Outputs = new ObservableCollection<RawConsoleOutputViewModel>();
        }

        private void OpenConsole()
        {
            this._commandPromptService.OpenCommandPrompt(Directory.GetCurrentDirectory());
        }

        private void AddOutput(string s, bool isInput)
        {
            // this can happen when a command is not done executing when the application gets shut down
            if (Application.Current?.Dispatcher == null)
                return;

            Application.Current.Dispatcher.Invoke(() =>
            {
                if (string.IsNullOrEmpty(s))
                    this.Outputs.Add(new RawConsoleOutputViewModel());
                else
                    this.Outputs.Add(new RawConsoleOutputViewModel
                    {
                        Value = s,
                        Time = DateTime.Now,
                        IsInput = isInput
                    });
            });
        }
    }
}