using System;
using System.Threading.Tasks;

namespace BestYoutubeDownloader.Services.CommandPrompt
{
    public interface ICommandPromptService
    {
        Task ExecuteCommandPromptCommand(string directory, string command, Action<string> actionOnDataOutput);
        
        void RegisterOutputAction(Action<string, bool> output);

        void OpenCommandPrompt(string directory);
    }
}