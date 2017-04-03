using System;
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

        private DownloadSettings _settings;

        private Action<string> _rawConsoleAction;

        public YoutubeDownloaderService()
        {
            this._exeLocation = Directory.GetCurrentDirectory() + @"\youtube-dl.exe";
            this._exeDirectoryLocation = Directory.GetCurrentDirectory();

            this._settings = IoC.Get<ISettingsService>().GetDownloadSettings();
        }

        public async Task<bool> DownloadVideo(Action<string> output, string url)
        {
            if (url.IsViableYoutubeUrl() == false)
                return false;

            output = this.WrapOutput(output);

            try
            {
                var command = @"youtube-dl -o " + this._settings.OutputLocation + @"\%(title)s.%(ext)s " + url;

                this._rawConsoleAction?.Invoke(command);

                await CommandPromptHelper.ExecuteCommand(this._exeDirectoryLocation,
                    command,
                    output);

                // --yes-playlist --extract-audio --audio-format mp3 
            }
            catch (Exception e)
            {
                return false;
            }

            return true;
        }

        public async Task<string> GetMetaData(string url)
        {
            //if (url.IsViableYoutubeUrl() == false)
            //    return false;
            //try
            //{
            //    await CommandPromptHelper.ExecuteCommand(@"C:\Users\Admin\Desktop\youtubedl",
            //        $@"youtube-dl -o C:\Users\Admin\Desktop\youtubedl\Downloads\%(title)s.%(ext)s  --yes-playlist --extract-audio --audio-format mp3 {url}",
            //        output);
            //}
            //catch (Exception e)
            //{
            //    return false;
            //}

            //return true;

            return null;
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

        public void ReloadSettings()
        {
            this._settings = IoC.Get<ISettingsService>().GetDownloadSettings();
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