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
            try
            {
                await CommandPromptHelper.ExecuteCommand(this._exeDirectoryLocation,
                    @"youtube-dl -o "+ this._settings.OutputLocation + @"\%(title)s.%(ext)s  --yes-playlist --extract-audio --audio-format mp3 " + url,
                    output);
            }
            catch (Exception e)
            {
                return false;
            }

            return true;
        }

        public async Task<string> GetThumbNail(string url)
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
    }
}