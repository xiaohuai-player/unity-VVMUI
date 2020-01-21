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
            // 参考 wpf 命令刷新通知机制，在执行命令时通知当前 vm 的所有命令刷新
            if (this._vm != null) {
                this._vm.NotifyCommandsCanExecute ();
            }
        }

        private UnityAction _eventDelegate;
        public virtual object GetEventDelegate (object parameter) {
            if (_eventDelegate == null) {
                _eventDelegate = new UnityAction (delegate () {
                    this.Execute (parameter);
                });
            }
            return _eventDelegate;
        }
    }

    public abstract class BaseCommand<T> : BaseCommand, ICommand<T> {
        protected Action<T, object> _executeHandler;

        public void Execute (T arg, object parameter) {
            if (_executeHandler != null) {
                _executeHandler.Invoke (arg, parameter);
            }
            // 参考 wpf 命令刷新通知机制，在执行命令时通知当前 vm 的所有命令刷新
            if (this._vm != null) {
                this._vm.NotifyCommandsCanExecute ();
            }
        }

        private UnityAction<T> _eventDelegate;
        public override object GetEventDelegate (object parameter) {
            if (_eventDelegate == null) {
                _eventDelegate = new UnityAction<T> (delegate (T arg) {
                    this.Execute (arg, parameter);
                });
            }
            return _eventDelegate;
        }
    }
}