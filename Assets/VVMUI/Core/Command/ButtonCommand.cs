using System;

namespace VVMUI.Core.Command
{
    public class ButtonCommand : BaseCommand
    {
        public ButtonCommand(Func<object, bool> canExecuteHandler, Action<object> executeHandler)
        {
            _canExecuteHandler = canExecuteHandler;
            _noArgExecuteHandler = executeHandler;
        }
    }
}