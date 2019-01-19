using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using BestYoutubeDownloader.Common;
using BestYoutubeDownloader.Extensions;
using BestYoutubeDownloader.Helper;
using BestYoutubeDownloader.Services.CommandPrompt;
using BestYoutubeDownloader.Services.Settings;
using Caliburn.Micro;
using Newtonsoft.Json;

namespace BestYoutubeDownloader.Services.YoutubeDL
{
    public class YoutubeDownloaderService : IYoutubeDownloaderService
    {
        private readonly ICommandPromptService _commandPromptService;
        private readonly string _exeLocation;
        private readonly string _exeDirectoryLocation;

        public YoutubeDownloaderService(ICommandPromptService commandPromptService)
        {
            this._commandPromptService = commandPromptService;

            this._exeLocation = Directory.GetCurrentDirectory() + @"\youtube-dl.exe";
            this._exeDirectoryLocation = Directory.GetCurrentDirectory();
        }

        #region VideoDownload

        public async Task<bool> DownloadVideo(Action<string> output, string url, DownloadSettings settings)
        {
            if (url.IsViableUrl() == false)
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
            var commandList = new List<string> { "youtube-dl" };

            commandList.AddRange(this.BuildDownloadArguments(settings));

            commandList.Add(this.BuildOutputPath(settings.OutputLocation));
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

        private string BuildOutputPath(string outputPath)
        {
            return "-o " + outputPath + @"\%(title)s.%(ext)s";
        }

        #endregion

        #region MetaData

        public async Task<MetaData> GetMetaData(string url)
        {
            if (url.IsViableUrl() == false)
                return null;

            try
            {
                var result = string.Empty;

                await this._commandPromptService.ExecuteCommandPromptCommand(this._exeDirectoryLocation,
                    "youtube-dl -j " + url,
                    s =>
                    {
                        if (string.IsNullOrWhiteSpace(s) == false)
                            result = s;
                    });

                var metaData = JsonConvert.DeserializeObject(result, typeof(MetaData));

                return (MetaData)metaData;
            }
            catch (Exception)
            {
                return null;
            }
        }

        #endregion

        #region ThumbNail

        public async Task<ImageSource> GetThumbNail(string url)
        {
            if (url.IsViableUrl() == false)
                return null;

            try
            {
                var outputUrl = IoC.Get<ISettingsService>().GetDownloadSettings().OutputLocation + @"\ThumbNails";

                if (Directory.Exists(outputUrl) == false)
                    Directory.CreateDirectory(outputUrl);

                var imageOutputUrl = string.Empty;

                var command = this.BuildThumbNailDownloadCommand(url, outputUrl);

                await this._commandPromptService.ExecuteCommandPromptCommand(this._exeDirectoryLocation,
                    command,
                    (string input) =>
                    {
                        if (input != null && input.Contains("Writing thumbnail to:"))
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
                    // ReSharper disable once AssignNullToNotNullAttribute
                    var directory = new DirectoryInfo(Path.GetDirectoryName(imagePath));

                    var latest = directory.GetFiles().ToList().OrderByDescending(f => f.CreationTime).First();

                    imagePath = latest.FullName;
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
                "youtube-dl",
                "--write-all-thumbnails",
                "--skip-download",
                this.BuildOutputPath(outputPath),
                url
            };

            return string.Join(" ", arguments);
        }

        #endregion

        #region Version

        public async Task<string> UpdateYoutubeDl()
        {
            var result = string.Empty;

            await this._commandPromptService.ExecuteCommandPromptCommand(this._exeDirectoryLocation, "youtube-dl --update", s =>
            {
                if (string.IsNullOrWhiteSpace(s) == false)
                    result = s;
            });

            return result;
        }

        public async Task<string> GetYoutubeDlVersion()
        {
            var result = string.Empty;

            await this._commandPromptService.ExecuteCommandPromptCommand(this._exeDirectoryLocation, "youtube-dl --version", s =>
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
                hasResult = true;
            });

            return hasResult;
        }

        public async Task<string> Validate()
        {
            if (this.ValidateExeLocation() == false)
                return "Couldn't find youtube-dl";

            if (await this.ValidateFfmepg() == false)
                return "ffmpeg not installed";

            return string.Empty;
        }

        #endregion
    }
}