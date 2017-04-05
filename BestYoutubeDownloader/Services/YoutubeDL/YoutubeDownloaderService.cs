using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Media.Effects;
using BestYoutubeDownloader.Extensions;
using BestYoutubeDownloader.Helper;
using BestYoutubeDownloader.Services.Settings;
using Caliburn.Micro;

namespace BestYoutubeDownloader.Services.YoutubeDL
{
    public class YoutubeDownloaderService : IYoutubeDownloaderService
    {
        private readonly string _exeLocation;
        private readonly string _exeDirectoryLocation;
        
        private Action<string> _rawConsoleAction;

        public YoutubeDownloaderService()
        {
            this._exeLocation = Directory.GetCurrentDirectory() + @"\youtube-dl.exe";
            this._exeDirectoryLocation = Directory.GetCurrentDirectory();
        }

        public async Task<bool> DownloadVideo(Action<string> output, string url, DownloadSettings settings)
        {
            if (url.IsViableUrl() == false)
                return false;

            output = this.WrapOutput(output);

            try
            {
                var command = this.BuildCommand(url, settings);

                this._rawConsoleAction?.Invoke(command);

                await CommandPromptHelper.ExecuteCommand(this._exeDirectoryLocation,
                    command,
                    output);

                // --yes-playlist --extract-audio --audio-format mp3 
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        private string BuildCommand(string url, DownloadSettings settings)
        {
            var commandList = new List<string> {"youtube-dl"};

            commandList.AddRange(this.BuildDownloadArguments(settings));

            commandList.Add("-o " + settings.OutputLocation + @"\%(title)s.%(ext)s");
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

            if(settings.PrintDebugInfo)
                arguments.Add("--verbose");

            if(settings.PrintTraffic)
                arguments.Add("--print-traffic");

            return arguments;
        }

        public async Task<bool> ExecuteCommand(Action<string> output, string command)
        {
            output = this.WrapOutput(output);

            try
            {
                this._rawConsoleAction?.Invoke(command);

                await CommandPromptHelper.ExecuteCommand(this._exeDirectoryLocation,
                    command,
                    output);
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        public async Task GetMetaData(string url)
        {
            //if (url.IsViableUrl() == false)
            //    return;

            //try
            //{
            //    await CommandPromptHelper.ExecuteCommand(this._exeDirectoryLocation,
            //        "youtube-dl -j"
            //        output);
            //}
            //catch (Exception e)
            //{
            //    return;
            //}

            return;
        }

        private bool ValidateExeLocation()
        {
            return File.Exists(this._exeLocation);
        }

        private async Task<bool> ValidateFfmepg()
        {
            var hasResult = false;

            await CommandPromptHelper.ExecuteCommand(this._exeDirectoryLocation, "ffmpeg -h", s =>
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

        private Action<string> WrapOutput(Action<string> output)
        {
            return s =>
            {
                this._rawConsoleAction?.Invoke(s);

                output(s);
            };
        }

        public void RegisterOutputAction(Action<string> output)
        {
            this._rawConsoleAction = output;
        }
    }
}