using System;
using System.Threading.Tasks;

namespace BestYoutubeDownloader.Services.CommandPrompt
{
    public interface ICommandPromptService
    {
        Task ExecuteCommandPromptCommand(string directory, string command, Action<string> actionOnDataOutput);

        Task<string> ExecuteCommandPromptCommandWithSingleOutput(string directory, string command);

        void RegisterOutputAction(Action<string> output);
    }
}