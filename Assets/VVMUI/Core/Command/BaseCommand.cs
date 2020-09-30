using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;

namespace VVMUI.Core.Command
{

    //TODO 需要硬写支持绑定的组件类型

    public abstract class BaseCommand : ICommand
    {
        protected Func<object, bool> _canExecuteHandler;
        protected Action<object> _noArgExecuteHandler;
        protected VMBehaviour _vm;
        private List<Action> _canExecuteChangedHandlers = new List<Action>();

        public void NotifyCanExecute()
        {
            for (int i = 0; i < _canExecuteChangedHandlers.Count; i++)
            {
                _canExecuteChangedHandlers[i].Invoke();
            }
        }

        public void AddCanExecuteChangedListener(Action handler)
        {
            _canExecuteChangedHandlers.Add(handler);
        }

        public void RemoveCanExecuteChangedListener(Action handler)
        {
            _canExecuteChangedHandlers.Remove(handler);
        }

        public void BindVM(VMBehaviour vm)
        {
            _vm = vm;
        }

        public bool CanExecute(object parameter)
        {
            if (_canExecuteHandler != null)
            {
                return _canExecuteHandler.Invoke(parameter);
            }
            else
            {
                return true;
            }
        }

        public void Execute(object parameter)
        {
            if (_noArgExecuteHandler != null)
            {
                _noArgExecuteHandler.Invoke(parameter);
            }
            // 参考 wpf 命令刷新通知机制，在执行命令时通知当前 vm 的所有命令刷新
            if (this._vm != null)
            {
                this._vm.NotifyCommandsCanExecute();
            }
        }

        public virtual object GetExecuteDelegate(object parameter)
        {
            UnityAction executeDelegate = new UnityAction(delegate ()
            {
                this.Execute(parameter);
            });
            return executeDelegate;
        }
    }

    public abstract class BaseCommand<T> : BaseCommand, ICommand<T>
    {
        protected Action<T, object> _executeHandler;

        public void Execute(T arg, object parameter)
        {
            if (_executeHandler != null)
            {
                _executeHandler.Invoke(arg, parameter);
            }
            // 参考 wpf 命令刷新通知机制，在执行命令时通知当前 vm 的所有命令刷新
            if (this._vm != null)
            {
                this._vm.NotifyCommandsCanExecute();
            }
        }

        public override object GetExecuteDelegate(object parameter)
        {
            UnityAction<T> executeDelegate = new UnityAction<T>(delegate (T arg)
            {
                this.Execute(arg, parameter);
            });
            return executeDelegate;
        }
    }
}