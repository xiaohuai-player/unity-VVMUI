using System;

namespace VVMUI.Core.Command {
    public class StringCommand : BaseCommand<string> {
        public StringCommand (Func<bool> canExecuteHandler, Action<string> executeHandler) {
            _canExecuteHandler = canExecuteHandler;
            _executeHandler = executeHandler;
        }
    }
}