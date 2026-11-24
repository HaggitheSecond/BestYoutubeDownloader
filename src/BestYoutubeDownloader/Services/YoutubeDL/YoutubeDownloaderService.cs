using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
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
        private string _downloaderName;

        public YoutubeDownloaderService(ICommandPromptService commandPromptService, ISettingsService settingsService, IEventAggregator eventAggregator)
        {
            this._commandPromptService = commandPromptService;
            this._settingsService = settingsService;

            eventAggregator.SubscribeOnUIThread(this);

            // this is not changeable by the user currently, but if it ever needs to be, simply change it here
            this._downloaderName = "yt-dlp";
            this._exeDirectoryLocation = AppContext.BaseDirectory;
            this._exeLocation = Path.Combine(this._exeDirectoryLocation, $"{this._downloaderName}.exe");
        }

        #region VideoDownload

        public async Task<bool> DownloadVideo(Action<string> output, string url, DownloadSettings settings, string? outputDirectory = null)
        {
            if (url.IsViableUrl() == false || string.IsNullOrWhiteSpace(this._downloaderName))
                return false;

            try
            {
                var arguments = this.BuildDownloadCommandArguments(url, settings, outputDirectory);

                await this._commandPromptService.ExecuteCommand(this._exeDirectoryLocation,
                    this._exeLocation,
                    arguments,
                    output);
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        private IList<string> BuildDownloadCommandArguments(string url, DownloadSettings settings, string? outputDirectory = null)
        {
            var arguments = new List<string>();

            arguments.AddRange(this.BuildDownloadArguments(settings));

            arguments.AddRange(this.BuildOutputPath(url, settings, outputDirectory));
            arguments.Add(url);

            return arguments;
        }

        private IList<string> BuildDownloadArguments(DownloadSettings settings)
        {
            var arguments = new List<string>();

            if (settings.ExtractAudio)
            {
                arguments.Add("--extract-audio");
                arguments.Add("--audio-format");
                arguments.Add(settings.AudioFormat.ToString().ToLower());
            }

            if (settings.PrintDebugInfo)
                arguments.Add("--verbose");

            if (settings.PrintTraffic)
                arguments.Add("--print-traffic");

            return arguments;
        }

        private IList<string> BuildOutputPath(string url, DownloadSettings settings, string? outputDirectory = null)
        {
            var outputPath = settings.OutputLocation;

            if (string.IsNullOrWhiteSpace(outputDirectory) == false)
            {
                outputPath = outputDirectory;
            }
            else
            {
                var alternativeLocation = settings.AlternativeOutputLocations.FirstOrDefault(f => url.ToLower().Contains(f.Url.ToLower()));

                if (alternativeLocation is not null)
                {
                    outputPath = alternativeLocation.Location;
                }
            }

            return this.BuildOutputPath(outputPath);
        }

        private IList<string> BuildOutputPath(string outputPath)
        {
            return new List<string> { "-o", Path.Combine(outputPath, "%(title)s.%(ext)s") };
        }

        #endregion

        #region MetaData

        public async Task<MetaData?> GetMetaData(string url)
        {
            if (url.IsViableUrl() == false || string.IsNullOrWhiteSpace(this._downloaderName))
                return null;

            try
            {
                var result = string.Empty;

                await this._commandPromptService.ExecuteCommand(this._exeDirectoryLocation,
                    this._exeLocation,
                    new[] { "-j", url },
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
            if (url.IsViableUrl() == false || string.IsNullOrWhiteSpace(this._downloaderName))
                return null;

            try
            {
                var thumbNailDirectory = this.GetThumbNailDirectory();

                if (Directory.Exists(thumbNailDirectory) == false)
                    Directory.CreateDirectory(thumbNailDirectory);

                var thumbNailName = Guid.NewGuid().ToString("N");

                var arguments = this.BuildThumbNailDownloadArguments(url, thumbNailDirectory, thumbNailName);

                await this._commandPromptService.ExecuteCommand(this._exeDirectoryLocation,
                    this._exeLocation,
                    arguments,
                    _ => { });

                var imagePath = Directory.EnumerateFiles(thumbNailDirectory, thumbNailName + ".*").FirstOrDefault();

                if (imagePath is null)
                    return null;

                var uri = new Uri(imagePath);

                var image = new BitmapImage(uri);

                return image;
            }
            catch (Exception)
            {
                return null;
            }
        }

        private IList<string> BuildThumbNailDownloadArguments(string url, string outputPath, string fileName)
        {
            return new List<string>
            {
                "--write-thumbnail",
                "--skip-download",
                "-o",
                Path.Combine(outputPath, fileName + ".%(ext)s"),
                url
            };
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
            if (string.IsNullOrWhiteSpace(this._downloaderName))
                return "No downloader!";

            if (File.Exists(this._exeLocation) == false)
                await this.DownloadYoutubeDl();

            var result = string.Empty;

            await this._commandPromptService.ExecuteCommand(this._exeDirectoryLocation, this._exeLocation, new[] { "--update" }, s =>
            {
                if (string.IsNullOrWhiteSpace(s) == false)
                    result = s;
            });

            return result;
        }

        private async Task DownloadYoutubeDl()
        {
            var url = $"https://github.com/yt-dlp/yt-dlp/releases/latest/download/{this._downloaderName}.exe";
            var temporaryLocation = this._exeLocation + ".download";

            try
            {
                using (var client = new HttpClient())
                using (var response = await client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead))
                {
                    response.EnsureSuccessStatusCode();

                    using (var fileStream = new FileStream(temporaryLocation, FileMode.Create, FileAccess.Write, FileShare.None))
                    {
                        await response.Content.CopyToAsync(fileStream);
                    }
                }

                File.Move(temporaryLocation, this._exeLocation, true);
            }
            catch (Exception e) when (e is HttpRequestException or IOException or UnauthorizedAccessException or TaskCanceledException)
            {
                try
                {
                    File.Delete(temporaryLocation);
                }
                catch (Exception)
                {
                }

                throw new InvalidConfigurationException($"Could not download {this._downloaderName}.exe: {e.Message}{Environment.NewLine}{Environment.NewLine}Please check your internet connection and restart the program, or download it manually from '{url}' and place it in '{this._exeDirectoryLocation}'.", e);
            }
        }

        public async Task<string> GetYoutubeDlVersion()
        {
            if (string.IsNullOrWhiteSpace(this._downloaderName))
                return "No downloader!";

            var result = string.Empty;

            await this._commandPromptService.ExecuteCommand(this._exeDirectoryLocation, this._exeLocation, new[] { "--version" }, s =>
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

            await this._commandPromptService.ExecuteCommand(this._exeDirectoryLocation, "ffmpeg", new[] { "-version" }, s =>
            {
                if (string.IsNullOrWhiteSpace(s) == false && Regex.IsMatch(s, "ffmpeg version"))
                    hasResult = true;
            });

            return hasResult;
        }

        public async Task Validate()
        {
            if (string.IsNullOrWhiteSpace(this._downloaderName))
                throw new InvalidConfigurationException("No downloader set");

            if (this.ValidateExeLocation() == false)
                throw new InvalidConfigurationException($"Could not locate {this._downloaderName}.exe. Please ensure it is located in this program's directory.{Environment.NewLine}{Environment.NewLine}Visit 'https://github.com/yt-dlp/yt-dlp' for more information.");

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