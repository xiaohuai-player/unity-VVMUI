using System;

namespace VVMUI.Core.Command {
    public class IntCommand : BaseCommand<int> {
        public IntCommand (Func<bool> canExecuteHandler, Action<int> executeHandler) {
            _canExecuteHandler = canExecuteHandler;
            _executeHandler = executeHandler;
        }
    }
}