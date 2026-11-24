using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BestYoutubeDownloader.Services.Logging;

namespace BestYoutubeDownloader.Services.CommandPrompt
{
    public class CommandPromptService : ICommandPromptService
    {
        private const int CouldNotStartExitCode = -1;

        private readonly ILogService _logService;

        public CommandPromptService(ILogService logService)
        {
            this._logService = logService;
        }

        public Task<int> ExecuteCommand(string directory, string fileName, IEnumerable<string> arguments, Action<string> actionOnStandardOutput)
        {
            var argumentList = arguments.ToList();
            var displayText = this.GetDisplayText(fileName, argumentList);

            this._logService.Info(displayText);

            return Task.Run(() =>
            {
                var errorOutput = new ConcurrentQueue<string>();

                using var process = this.GetProcess(directory, fileName, argumentList);

                process.OutputDataReceived += (sender, args) =>
                {
                    if (args?.Data is null)
                        return;

                    this._logService.Debug(args.Data);

                    actionOnStandardOutput.Invoke(args.Data);
                };

                process.ErrorDataReceived += (sender, args) =>
                {
                    if (args?.Data is null)
                        return;

                    this._logService.Debug(args.Data);

                    errorOutput.Enqueue(args.Data);
                };

                try
                {
                    process.Start();
                }
                catch (Win32Exception e)
                {
                    this._logService.Error($"Could not start '{fileName}'", e);

                    return CouldNotStartExitCode;
                }

                process.BeginOutputReadLine();
                process.BeginErrorReadLine();
                process.WaitForExit();

                if (process.ExitCode != 0)
                    this._logService.Warn($"'{displayText}' exited with code {process.ExitCode}: {string.Join(Environment.NewLine, errorOutput)}");

                return process.ExitCode;
            });
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
                StandardOutputEncoding = Encoding.UTF8,
                StandardErrorEncoding = Encoding.UTF8,
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
