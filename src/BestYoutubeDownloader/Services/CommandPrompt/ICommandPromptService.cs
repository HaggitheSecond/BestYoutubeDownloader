using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BestYoutubeDownloader.Services.CommandPrompt
{
    public interface ICommandPromptService
    {
        Task<int> ExecuteCommand(string directory, string fileName, IEnumerable<string> arguments, Action<string> actionOnStandardOutput);
        
        void OpenCommandPrompt(string directory);
    }
}