using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;

namespace VVMUI.Core.Command {
    public abstract class BaseCommand : ICommand {
        protected Func<object, bool> _canExecuteHandler;
        protected Action<object> _noArgExecuteHandler;
        protected VMBehaviour _vm;

        public event Action CanExecuteChanged;

        public void NotifyCanExecute () {
            if (CanExecuteChanged != null) {
                CanExecuteChanged.Invoke ();
            }
        }

        public void BindVM (VMBehaviour vm) {
            _vm = vm;
        }

        public bool CanExecute (object parameter) {
            if (_canExecuteHandler != null) {
                return _canExecuteHandler.Invoke (parameter);
            } else {
                return true;
            }
        }

        public void Execute (object parameter) {
            if (_noArgExecuteHandler != null) {
                _noArgExecuteHandler.Invoke (parameter);
            }
            if (this._vm != null) {
                this._vm.NotifyCommandsCanExecute ();
            }
        }

        public virtual object GetEventDelegate (object parameter) {
            UnityAction act = new UnityAction (delegate () {
                this.Execute (parameter);
            });
            return act;
        }
    }

    public abstract class BaseCommand<T> : BaseCommand, ICommand<T> {
        protected Action<T, object> _executeHandler;

        public void Execute (T arg, object parameter) {
            if (_executeHandler != null) {
                _executeHandler.Invoke (arg, parameter);
            }
            if (this._vm != null) {
                this._vm.NotifyCommandsCanExecute ();
            }
        }

        public override object GetEventDelegate (object parameter) {
            UnityAction<T> act = new UnityAction<T> (delegate (T arg) {
                this.Execute (arg, parameter);
            });
            return act;
        }
    }
}