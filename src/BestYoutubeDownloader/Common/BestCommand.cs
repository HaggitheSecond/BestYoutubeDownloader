using System;
using System.ComponentModel;
using System.Windows.Input;
using Caliburn.Micro;
using DevExpress.Mvvm;
using Action = System.Action;

namespace BestYoutubeDownloader.Common
{
    public class BestCommand : DelegateCommand
    {

        // stub for later self implementation of icommand

        public BestCommand(Action executeMethod)
            : base(executeMethod)
        {
            
        }

        public BestCommand(Action executeMethod, bool useCommandManager)
            : base (executeMethod, useCommandManager)
        {
            
        }

        public BestCommand(Action executeMethod, Func<bool> canExecuteMethod, bool? useCommandManager = null)
            : base(executeMethod, canExecuteMethod, useCommandManager)
        {
            
        }
        
        //    private readonly Predicate<object> _canExecute;

        //    private readonly Action _execute;
        //    private readonly Action<object> _executeWithParam;

        //    public event EventHandler CanExecuteChanged
        //    {
        //        add
        //        {
        //            CommandManager.RequerySuggested += value;
        //        }
        //        remove
        //        {
        //            CommandManager.RequerySuggested -= value;
        //        }
        //    }

        //    public BestCommand(Action executeWithParam)
        //              : this(executeWithParam, null)
        //    {
        //    }

        //    public BestCommand(Action executeWithParam,
        //                   Predicate<object> canExecute)
        //    {
        //        this._execute = executeWithParam;
        //        this._canExecute = canExecute;
        //    }

        //    public BestCommand(Action<object> executeWithParam)
        //               : this(executeWithParam, null)
        //    {
        //    }

        //    public BestCommand(Action<object> executeWithParam,
        //                   Predicate<object> canExecute)
        //    {
        //        this._executeWithParam = executeWithParam;
        //        this._canExecute = canExecute;
        //    }

        //    private bool _latestCanExecute;

        //    public bool CanExecute(object parameter)
        //    {
        //        if (this._canExecute == null)
        //        {
        //            return true;
        //        }

        //        return this._canExecute(parameter);

        //        this.RaiseCanExecuteChanged();

        //    }

        //    public void Execute(object parameter)
        //    {
        //        if (this._execute != null)
        //            this._execute();

        //        if (this._executeWithParam != null)
        //            this._executeWithParam(parameter);
        //    }

        //    public void RaiseCanExecuteChanged()
        //    {

        //    }
    }
}