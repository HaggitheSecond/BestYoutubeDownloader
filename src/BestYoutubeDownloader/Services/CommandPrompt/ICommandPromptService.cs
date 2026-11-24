using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BestYoutubeDownloader.Services.CommandPrompt
{
    public interface ICommandPromptService
    {
        Task ExecuteCommand(string directory, string fileName, IEnumerable<string> arguments, Action<string> actionOnDataOutput);
        
        void RegisterOutputAction(Action<string, bool> output);

        void OpenCommandPrompt(string directory);
    }
}