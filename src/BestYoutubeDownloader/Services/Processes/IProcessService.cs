using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BestYoutubeDownloader.Services.Processes
{
    public interface IProcessService
    {
        Task<int> Execute(string directory, string fileName, IEnumerable<string> arguments, Action<string> actionOnStandardOutput);
    }
}