using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace BestYoutubeDownloader.Helper
{
    public static class CommandPromptHelper
    {
        public static Task ExecuteCommand(string directory, string command, Action<string> actionOnDataOutput)
        {
            return Task.Run(() =>
            {
                var processInfo = new ProcessStartInfo
                {
                    UseShellExecute = false,
                    WorkingDirectory = directory,
                    FileName = @"C:\Windows\System32\cmd.exe",
                    Verb = "runas",
                    Arguments = "/c " + command,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                };

                var process = new Process {StartInfo = processInfo};
                process.OutputDataReceived += (sender, args) => { actionOnDataOutput.Invoke(args.Data); };

                process.Start();
                process.BeginOutputReadLine();
                process.WaitForExit();
            });
        }
    }
}