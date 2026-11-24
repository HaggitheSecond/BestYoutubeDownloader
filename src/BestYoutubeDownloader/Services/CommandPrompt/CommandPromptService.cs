using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using BestYoutubeDownloader.Services.Logging;

namespace BestYoutubeDownloader.Services.CommandPrompt
{
    public class CommandPromptService : ICommandPromptService
    {
        private readonly ILogService _logService;

        public CommandPromptService(ILogService logService)
        {
            this._logService = logService;
        }

        public Task ExecuteCommand(string directory, string fileName, IEnumerable<string> arguments, Action<string> actionOnDataOutput)
        {
            var argumentList = arguments.ToList();

            this._logService.Info(this.GetDisplayText(fileName, argumentList));
            actionOnDataOutput = this.WrapOutput(actionOnDataOutput);

            return Task.Run(() =>
            {
                using var process = this.GetProcess(directory, fileName, argumentList);

                process.OutputDataReceived += (sender, args) =>
                {
                    if (args?.Data is null)
                        return;

                    actionOnDataOutput.Invoke(args.Data);
                };

                process.ErrorDataReceived += (sender, args) =>
                {
                    if (args?.Data is null)
                        return;

                    actionOnDataOutput.Invoke(args.Data);
                };

                try
                {
                    process.Start();
                }
                catch (Win32Exception e)
                {
                    actionOnDataOutput.Invoke($"Could not start '{fileName}': {e.Message}");
                    return;
                }

                process.BeginOutputReadLine();
                process.BeginErrorReadLine();
                process.WaitForExit();
            });
        }

        private Action<string> WrapOutput(Action<string> output)
        {
            return s =>
            {
                if (s.StartsWith("ERROR"))
                    this._logService.Error(s);
                else if (s.StartsWith("WARNING"))
                    this._logService.Warn(s);
                else
                    this._logService.Debug(s);

                output(s);
            };
        }

        private Process GetProcess(string directory, string fileName, IEnumerable<string> arguments)
        {
            var processInfo = new ProcessStartInfo
            {
                UseShellExecute = false,
                WorkingDirectory = directory,
                FileName = fileName,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                RedirectStandardInput = true,
                CreateNoWindow = true
            };

            foreach (var argument in arguments)
            {
                processInfo.ArgumentList.Add(argument);
            }

            return new Process { StartInfo = processInfo };
        }

        private string GetDisplayText(string fileName, IEnumerable<string> arguments)
        {
            var parts = new[] { System.IO.Path.GetFileNameWithoutExtension(fileName) }
                .Concat(arguments.Select(f => f.Contains(' ') ? $"\"{f}\"" : f));

            return string.Join(" ", parts);
        }

        public void OpenCommandPrompt(string directory)
        {
            new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    WorkingDirectory = directory,
                    FileName = @"C:\Windows\System32\cmd.exe",
                }
            }.Start();
        }
    }
}
