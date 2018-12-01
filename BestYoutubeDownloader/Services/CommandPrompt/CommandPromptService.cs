using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace BestYoutubeDownloader.Services.CommandPrompt
{
    public class CommandPromptService : ICommandPromptService
    {
        private Action<string> _rawConsoleAction;

        public Task ExecuteCommandPromptCommand(string directory, string command, Action<string> actionOnDataOutput)
        {
            this._rawConsoleAction.Invoke(command);
            actionOnDataOutput = this.WrapOutput(actionOnDataOutput);

            return Task.Run(() =>
            {
                var process = GetProcess(directory, command);

                process.OutputDataReceived += (sender, args) =>
                {
                    actionOnDataOutput.Invoke(args.Data); 
                };

                process.Start();
                process.BeginOutputReadLine();
                process.WaitForExit();
            });
        }

        public Task<string> ExecuteCommandPromptCommandWithSingleOutput(string directory, string command)
        {
            return Task.Run(() =>
            {
                var output = string.Empty;

                this.ExecuteCommandPromptCommand(directory, command, s => { output = s; });

                return output;
            });
        }

        
        private Action<string> WrapOutput(Action<string> output)
        {
            return s =>
            {
                this._rawConsoleAction?.Invoke(s);

                output(s);
            };
        }

        private Process GetProcess(string directory, string command)
        {
            var processInfo = new ProcessStartInfo
            {
                UseShellExecute = false,
                WorkingDirectory = directory,
                FileName = @"C:\Windows\System32\cmd.exe",
                Verb = "runas",
                Arguments = "/c " + command,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            };

            return new Process { StartInfo = processInfo };
        }
        
        public void RegisterOutputAction(Action<string> output)
        {
            this._rawConsoleAction = output;
        }
    }
}