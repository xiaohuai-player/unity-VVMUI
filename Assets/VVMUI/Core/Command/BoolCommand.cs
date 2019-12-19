using System;

namespace VVMUI.Core.Command {
    public class BoolCommand : BaseCommand<bool> {
        public BoolCommand (Func<bool> canExecuteHandler, Action<bool> executeHandler) {
            _canExecuteHandler = canExecuteHandler;
            _executeHandler = executeHandler;
        }
    }
}