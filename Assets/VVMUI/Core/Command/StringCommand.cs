using System;

namespace VVMUI.Core.Command {
    public class StringCommand : BaseCommand<string> {
        public StringCommand (Func<object, bool> canExecuteHandler, Action<string, object> executeHandler) {
            _canExecuteHandler = canExecuteHandler;
            _executeHandler = executeHandler;
        }
    }
}