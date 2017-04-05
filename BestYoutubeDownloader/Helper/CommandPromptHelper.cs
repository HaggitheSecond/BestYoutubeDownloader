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
                var process = GetProcess(directory, command);
                
                process.OutputDataReceived += (sender, args) => { actionOnDataOutput.Invoke(args.Data); };

                process.Start();
                process.BeginOutputReadLine();
                process.WaitForExit();
            });
        }

        public static Task<string> ExecuteCommandWithSingleOutput(string directory, string command)
        {
            return Task.Run(() =>
            {
                var process = GetProcess(directory, command);

                var output = string.Empty;

                process.OutputDataReceived += (sender, args) =>
                {
                    if (string.IsNullOrWhiteSpace(args.Data) == false)
                        output = args.Data;
                };
                
                process.Start();
                process.BeginOutputReadLine();
                process.WaitForExit();

                return output;
            });
        }

        private static Process GetProcess(string directory, string command)
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

            return new Process { StartInfo = processInfo };
        }
    }
}