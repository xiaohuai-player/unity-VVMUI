using System;

namespace VVMUI.Core.Command {
    public class IntCommand : BaseCommand<int> {
        public IntCommand (Func<object, bool> canExecuteHandler, Action<int, object> executeHandler) {
            _canExecuteHandler = canExecuteHandler;
            _executeHandler = executeHandler;
        }
    }
}