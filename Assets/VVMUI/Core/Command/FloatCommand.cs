using System;

namespace VVMUI.Core.Command {
    public class FloatCommand : BaseCommand<float> {
        public FloatCommand (Func<object, bool> canExecuteHandler, Action<float, object> executeHandler) {
            _canExecuteHandler = canExecuteHandler;
            _executeHandler = executeHandler;
        }
    }
}