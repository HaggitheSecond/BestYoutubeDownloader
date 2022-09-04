using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using BestYoutubeDownloader.Common;
using BestYoutubeDownloader.Events;
using BestYoutubeDownloader.Exceptions;
using BestYoutubeDownloader.Extensions;
using BestYoutubeDownloader.Helper;
using BestYoutubeDownloader.Services.CommandPrompt;
using BestYoutubeDownloader.Services.Settings;
using Caliburn.Micro;
using Newtonsoft.Json;

namespace BestYoutubeDownloader.Services.YoutubeDL
{
    public class YoutubeDownloaderService : IYoutubeDownloaderService, IHandle<SettingsChanged>
    {
        private readonly ICommandPromptService _commandPromptService;
        private readonly ISettingsService _settingsService;

        private readonly string _exeLocation;
        private readonly string _exeDirectoryLocation;
        private string _downloader;

        public YoutubeDownloaderService(ICommandPromptService commandPromptService, ISettingsService settingsService, IEventAggregator eventAggregator)
        {
            this._commandPromptService = commandPromptService;
            this._settingsService = settingsService;

            eventAggregator.SubscribeOnUIThread(this);

            // this is not changeable by the user currently, but if it ever needs to be, simply change it here
            this._downloader = "yt-dlp";
            this._exeLocation = Directory.GetCurrentDirectory() + $@"\{this._downloader}.exe";
            this._exeDirectoryLocation = Directory.GetCurrentDirectory();
        }

        #region VideoDownload

        public async Task<bool> DownloadVideo(Action<string> output, string url, DownloadSettings settings)
        {
            if (url.IsViableUrl() == false || string.IsNullOrWhiteSpace(this._downloader))
                return false;

            try
            {
                var command = this.BuildDownloadCommand(url, settings);

                await this._commandPromptService.ExecuteCommandPromptCommand(this._exeDirectoryLocation,
                    command,
                    output);
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        private string BuildDownloadCommand(string url, DownloadSettings settings)
        {
            var commandList = new List<string> { this._downloader };

            commandList.AddRange(this.BuildDownloadArguments(settings));

            commandList.Add(this.BuildOutputPath(url, settings));
            commandList.Add(url);

            return string.Join(" ", commandList);
        }

        private IList<string> BuildDownloadArguments(DownloadSettings settings)
        {
            var arguments = new List<string>();

            if (settings.ExtractAudio)
            {
                arguments.Add("--extract-audio");
                arguments.Add("--audio-format " + settings.AudioFormat.ToString().ToLower());
            }

            if (settings.PrintDebugInfo)
                arguments.Add("--verbose");

            if (settings.PrintTraffic)
                arguments.Add("--print-traffic");

            return arguments;
        }

        private string BuildOutputPath(string url, DownloadSettings settings)
        {
            var outputPath = settings.OutputLocation;
            var alternativeLocation = settings.AlternativeOutputLocations.FirstOrDefault(f => url.ToLower().Contains(f.Url.ToLower()));

            if(alternativeLocation is not null)
            {
                outputPath = alternativeLocation.Location;
            }

            return this.BuildOutputPath(outputPath);
        }

        private string BuildOutputPath(string outputPath)
        {
            return "-o " + outputPath + @"\%(title)s.%(ext)s";
        }

        #endregion

        #region MetaData

        public async Task<MetaData?> GetMetaData(string url)
        {
            if (url.IsViableUrl() == false || string.IsNullOrWhiteSpace(this._downloader))
                return null;

            try
            {
                var result = string.Empty;

                await this._commandPromptService.ExecuteCommandPromptCommand(this._exeDirectoryLocation,
                    $"{this._downloader} -j " + url,
                    s =>
                    {
                        if (string.IsNullOrWhiteSpace(s) == false)
                            result = s;
                    });

                var metaData = JsonConvert.DeserializeObject(result, typeof(MetaData));

                if (metaData is null)
                    return null;

                return (MetaData)metaData;
            }
            catch (Exception)
            {
                return null;
            }
        }

        #endregion

        #region ThumbNail

        public async Task<ImageSource?> GetThumbNail(string url)
        {        
            if (url.IsViableUrl() == false || string.IsNullOrWhiteSpace(this._downloader))
                return null;

            try
            {
                var thumbNailDirectory = this.GetThumbNailDirectory();

                if (Directory.Exists(thumbNailDirectory) == false)
                    Directory.CreateDirectory(thumbNailDirectory);

                var imageOutputUrl = string.Empty;

                var command = this.BuildThumbNailDownloadCommand(url, thumbNailDirectory);

                await this._commandPromptService.ExecuteCommandPromptCommand(this._exeDirectoryLocation,
                    command,
                    input =>
                    {
                        if (input != null && input.Contains("Writing video thumbnail"))
                            imageOutputUrl = input;
                    });

                if (string.IsNullOrWhiteSpace(imageOutputUrl))
                    return null;

                var stringParts = imageOutputUrl.Split(':');

                var imagePath = stringParts[stringParts.Length - 2] + ":" + stringParts[stringParts.Length - 1].Trim();

                // adjust the filename to the actual existing file

                // this is need because the filename extracted from the console will not contain non-ascii characters
                // but the actual filename will contain them - this is needed to be able to write metadata

                // could lead to problems when multiple programs/processes are writing to the directory simultaneously

                if (File.Exists(imagePath) == false)
                {
                    var directoryName = Path.GetDirectoryName(imagePath);

                    if (string.IsNullOrWhiteSpace(directoryName) == false)
                    {
                        var directory = new DirectoryInfo(directoryName);

                        var latest = directory.GetFiles().ToList().OrderByDescending(f => f.CreationTime).First();

                        imagePath = latest.FullName;
                    }
                }

                var uri = new Uri(imagePath);

                var image = new BitmapImage(uri);

                return image;
            }
            catch (Exception)
            {
                return null;
            }
        }

        private string BuildThumbNailDownloadCommand(string url, string outputPath)
        {
            var arguments = new List<string>
            {
                this._downloader,
                "--write-thumbnail",
                "--skip-download",
                this.BuildOutputPath(outputPath),
                url
            };

            return string.Join(" ", arguments);
        }

        public void ClearThumbNailsDirectory()
        {
            var directory = this.GetThumbNailDirectory();

            if (Directory.Exists(directory) is false)
                return;

            Directory.Delete(directory, true);
        }

        private string GetThumbNailDirectory()
        {
            return IoC.Get<ISettingsService>().GetDownloadSettings().OutputLocation + @"\ThumbNails";
        }

        #endregion

        #region Version

        public async Task<string> UpdateYoutubeDl()
        {
            if (string.IsNullOrWhiteSpace(this._downloader))
                return "No downloader!";

            var result = string.Empty;

            await this._commandPromptService.ExecuteCommandPromptCommand(this._exeDirectoryLocation, this._downloader + " --update", s =>
            {
                if (string.IsNullOrWhiteSpace(s) == false)
                    result = s;
            });

            return result;
        }

        public async Task<string> GetYoutubeDlVersion()
        {
            if (string.IsNullOrWhiteSpace(this._downloader))
                return "No downloader!";

            var result = string.Empty;

            await this._commandPromptService.ExecuteCommandPromptCommand(this._exeDirectoryLocation, this._downloader + " --version", s =>
            {
                if (string.IsNullOrWhiteSpace(s) == false) result = s;
            });

            return result;
        }

        #endregion

        #region Validation

        private bool ValidateExeLocation()
        {
            return File.Exists(this._exeLocation);
        }

        private async Task<bool> ValidateFfmepg()
        {
            var hasResult = false;

            await this._commandPromptService.ExecuteCommandPromptCommand(this._exeDirectoryLocation, "ffmpeg", s =>
            {
                if (string.IsNullOrWhiteSpace(s) == false && Regex.IsMatch(s, "ffmpeg version"))
                    hasResult = true;
            });

            return hasResult;
        }

        public async Task Validate()
        {
            if (string.IsNullOrWhiteSpace(this._downloader))
                throw new InvalidConfigurationException("No downloader set");

            if (this.ValidateExeLocation() == false)
                throw new InvalidConfigurationException($"Could not locate {this._downloader}.exe. Please ensure it is located in his programs executing directory.{Environment.NewLine}{Environment.NewLine}Visit 'https://youtube-dl.org/' for emore information.");

            if (await this.ValidateFfmepg() == false)
                throw new InvalidConfigurationException($"Could not locate ffmpeg. Please ensure it is properly installed and updated on your system.{Environment.NewLine}{Environment.NewLine}Visit 'https://ffmpeg.org/' for more information.");

        }

        public async Task<bool> IsValid()
        {
            try
            {
                await this.Validate();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task HandleAsync(SettingsChanged message, CancellationToken cancellationToken)
        {
            await this.Validate();
        }

        #endregion
    }
}