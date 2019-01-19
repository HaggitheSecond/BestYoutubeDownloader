using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace BestYoutubeDownloader.Services.CommandPrompt
{
    public class CommandPromptService : ICommandPromptService
    {
        private Action<string, bool> _rawConsoleAction;

        public Task ExecuteCommandPromptCommand(string directory, string command, Action<string> actionOnDataOutput)
        {
            this._rawConsoleAction.Invoke(command, true);
            actionOnDataOutput = this.WrapOutput(actionOnDataOutput);

            return Task.Run(() =>
            {
                var process = GetProcess(directory, command);

                process.OutputDataReceived += (sender, args) =>
                {
                    actionOnDataOutput.Invoke(args.Data); 
                };

                process.ErrorDataReceived += (sender, args) =>
                {
                    actionOnDataOutput.Invoke(args.Data);
                };
                
                process.Start();
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();
                process.WaitForExit();
            });
        }
        
        private Action<string> WrapOutput(Action<string> output)
        {
            return s =>
            {
                this._rawConsoleAction?.Invoke(s, false);

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
                RedirectStandardInput = true,
                CreateNoWindow = true
            };

            return new Process { StartInfo = processInfo };
        }
        
        public void RegisterOutputAction(Action<string, bool> output)
        {
            this._rawConsoleAction = output;
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