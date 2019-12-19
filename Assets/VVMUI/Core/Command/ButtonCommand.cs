using System;

namespace VVMUI.Core.Command {
    public class ButtonCommand : BaseCommand {
        public ButtonCommand (Func<bool> canExecuteHandler, Action executeHandler) {
            _canExecuteHandler = canExecuteHandler;
            _noArgExecuteHandler = executeHandler;
        }
    }
}