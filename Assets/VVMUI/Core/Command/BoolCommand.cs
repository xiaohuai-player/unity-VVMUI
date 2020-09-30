using System;

namespace VVMUI.Core.Command
{
    public class BoolCommand : BaseCommand<bool>
    {
        public BoolCommand(Func<object, bool> canExecuteHandler, Action<bool, object> executeHandler)
        {
            _canExecuteHandler = canExecuteHandler;
            _executeHandler = executeHandler;
        }
    }
}