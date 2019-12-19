using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;

namespace VVMUI.Core.Command {
    public abstract class BaseCommand : ICommand {
        protected Func<bool> _canExecuteHandler;
        protected Action _noArgExecuteHandler;

        public event Action<bool> CanExecuteChanged;

        private bool _canExecute;
        public bool CanExecute () {
            return _canExecute;
        }

        public void RefreshCanExecute () {
            if (_canExecuteHandler != null) {
                bool f = _canExecuteHandler.Invoke ();
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
                _noArgExecuteHandler.Invoke ();
            }
        }

        public virtual Type GetExecuteDelegateType () {
            return typeof (UnityAction);
        }
    }

    public abstract class BaseCommand<T> : BaseCommand, ICommand<T> {
        protected Action<T> _executeHandler;

        public void GenericExecute (T arg) {
            if (_executeHandler != null) {
                _executeHandler.Invoke (arg);
            }
        }

        public override Type GetExecuteDelegateType () {
            return typeof (UnityAction<T>);
        }
    }
}