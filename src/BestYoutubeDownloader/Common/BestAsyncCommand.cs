using System;
using System.ComponentModel;
using System.Threading.Tasks;
using DevExpress.Mvvm;

namespace BestYoutubeDownloader.Common
{
    public class BestAsyncCommand : AsyncCommand
    {
        // stub for later self implemenation of icommand
        public BestAsyncCommand(Func<Task> executeMethod)
            :base(executeMethod)
        {
            
        }
        public BestAsyncCommand(Func<Task> executeMethod, bool useCommandManager)
           : base(executeMethod, useCommandManager)
        {

        }
        public BestAsyncCommand(Func<Task> executeMethod, Func<bool> canExecuteMethod, bool? useCommandManager = null)
           : base(executeMethod, canExecuteMethod, useCommandManager)
        {

        }
        public BestAsyncCommand(Func<Task> executeMethod, Func<bool> canExecuteMethod, bool allowMultipleExecution, bool? useCommandManager = null)
           : base(executeMethod, canExecuteMethod, allowMultipleExecution, useCommandManager)
        {

        }
    }
}