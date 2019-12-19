using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;

namespace VVMUI.Core.Command {
    public abstract class BaseCommand : ICommand {
        protected object _parameter;
        protected Func<object, bool> _canExecuteHandler;
        protected Action<object> _noArgExecuteHandler;

        public event Action<bool> CanExecuteChanged;

        public void SetParameter (object parameter) {
            this._parameter = parameter;
        }

        private bool _canExecute;
        public bool CanExecute () {
            return _canExecute;
        }

        public void RefreshCanExecute () {
            if (_canExecuteHandler != null) {
                bool f = _canExecuteHandler.Invoke (this._parameter);
                if (_canExecute != f) {
                    if (CanExecuteChanged != null) {
                        CanExecuteChanged.Invoke (f);
                    }
                    _canExecute = f;
                }
            }
        }

        public void Execute () {
            if (_noArgExecuteHandler != null) {
                _noArgExecuteHandler.Invoke (this._parameter);
            }
        }

        public virtual Type GetEventDelegateType () {
            return typeof (UnityAction);
        }
    }

    public abstract class BaseCommand<T> : BaseCommand, ICommand<T> {
        protected Action<T, object> _executeHandler;

        public void GenericExecute (T arg) {
            if (_executeHandler != null) {
                _executeHandler.Invoke (arg, this._parameter);
            }
        }

        public override Type GetEventDelegateType () {
            return typeof (UnityAction<T>);
        }
    }
}