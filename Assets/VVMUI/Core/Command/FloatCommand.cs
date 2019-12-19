using System;

namespace VVMUI.Core.Command {
    public class FloatCommand : BaseCommand<float> {
        public FloatCommand (Func<bool> canExecuteHandler, Action<float> executeHandler) {
            _canExecuteHandler = canExecuteHandler;
            _executeHandler = executeHandler;
        }
    }
}