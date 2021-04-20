using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace VVMUI.Core.Command
{
    public abstract class BaseCommand : ICommand
    {
        protected Func<object, bool> _canExecuteHandler;
        protected List<Action> _canExecuteChangedHandlers = new List<Action>();
        protected VMBehaviour _vm;
        protected Dictionary<int, object> _executeDelegatesCache = new Dictionary<int, object>();

        private Action<object> _noArgExecuteHandler;
        private Action<UnityEvent, UnityAction> _addListenerDelegate = (Action<UnityEvent, UnityAction>)Delegate.CreateDelegate(typeof(Action<UnityEvent, UnityAction>), null, ReflectionCache.Singleton[typeof(UnityEvent)].GetMethod("AddListener"));
        private Action<UnityEvent, UnityAction> _removeListenerDelegate = (Action<UnityEvent, UnityAction>)Delegate.CreateDelegate(typeof(Action<UnityEvent, UnityAction>), null, ReflectionCache.Singleton[typeof(UnityEvent)].GetMethod("RemoveListener"));

        protected BaseCommand(Func<object, bool> canExecuteHandler, Action<object> executeHandler)
        {
            _canExecuteHandler = canExecuteHandler;
            _noArgExecuteHandler = executeHandler;
        }

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

        private object GetExecuteDelegate(object parameter)
        {
            int hashCode = 0;
            if (parameter != null)
            {
                hashCode = parameter.GetHashCode();
            }
            if (!_executeDelegatesCache.TryGetValue(hashCode, out object executeDelegate))
            {
                executeDelegate = new UnityAction(delegate ()
                {
                    this.Execute(parameter);
                });
                _executeDelegatesCache[hashCode] = executeDelegate;
            }
            return executeDelegate;
        }

        public virtual object AddListenerToEvent(object eventTarget, object parameter)
        {
            UnityAction action = (UnityAction)GetExecuteDelegate(parameter);
            _addListenerDelegate.Invoke((UnityEvent)eventTarget, action);
            return action;
        }

        public virtual void RemoveListenerFromEvent(object eventTarget, object action)
        {
            _removeListenerDelegate.Invoke((UnityEvent)eventTarget, (UnityAction)action);
        }
    }

    public abstract class BaseCommand<T> : BaseCommand, ICommand<T>
    {
        protected Action<T, object> _executeHandler;

        private Action<UnityEvent<T>, UnityAction<T>> _addListenerDelegate = (Action<UnityEvent<T>, UnityAction<T>>)Delegate.CreateDelegate(typeof(Action<UnityEvent<T>, UnityAction<T>>), null, ReflectionCache.Singleton[typeof(UnityEvent<T>)].GetMethod("AddListener"));
        private Action<UnityEvent<T>, UnityAction<T>> _removeListenerDelegate = (Action<UnityEvent<T>, UnityAction<T>>)Delegate.CreateDelegate(typeof(Action<UnityEvent<T>, UnityAction<T>>), null, ReflectionCache.Singleton[typeof(UnityEvent<T>)].GetMethod("RemoveListener"));

        protected BaseCommand(Func<object, bool> canExecuteHandler, Action<T, object> executeHandler) : base(canExecuteHandler, null)
        {
            _canExecuteHandler = canExecuteHandler;
            _executeHandler = executeHandler;
        }

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

        private object GetExecuteDelegate(object parameter)
        {
            int hashCode = 0;
            if (parameter != null)
            {
                hashCode = parameter.GetHashCode();
            }
            if (!_executeDelegatesCache.TryGetValue(hashCode, out object executeDelegate))
            {
                executeDelegate = new UnityAction<T>(delegate (T arg)
                {
                    this.Execute(arg, parameter);
                });
                _executeDelegatesCache[hashCode] = executeDelegate;
            }
            return executeDelegate;
        }

        public override object AddListenerToEvent(object eventTarget, object parameter)
        {
            UnityAction<T> action = (UnityAction<T>)GetExecuteDelegate(parameter);
            _addListenerDelegate.Invoke((UnityEvent<T>)eventTarget, action);
            return action;
        }

        public override void RemoveListenerFromEvent(object eventTarget, object action)
        {
            _removeListenerDelegate.Invoke((UnityEvent<T>)eventTarget, (UnityAction<T>)action);
        }
    }

    public class VoidCommand : BaseCommand
    {
        public VoidCommand(Func<object, bool> canExecuteHandler, Action<object> executeHandler) : base(canExecuteHandler, executeHandler)
        {
        }
    }

    [Obsolete("please use VoidCommand instead.")]
    public class ButtonCommand : BaseCommand
    {
        public ButtonCommand(Func<object, bool> canExecuteHandler, Action<object> executeHandler) : base(canExecuteHandler, executeHandler)
        {
        }
    }

    public class BoolCommand : BaseCommand<bool>
    {
        public BoolCommand(Func<object, bool> canExecuteHandler, Action<bool, object> executeHandler) : base(canExecuteHandler, executeHandler)
        {
        }
    }

    public class FloatCommand : BaseCommand<float>
    {
        public FloatCommand(Func<object, bool> canExecuteHandler, Action<float, object> executeHandler) : base(canExecuteHandler, executeHandler)
        {
        }
    }

    public class IntCommand : BaseCommand<int>
    {
        public IntCommand(Func<object, bool> canExecuteHandler, Action<int, object> executeHandler) : base(canExecuteHandler, executeHandler)
        {
        }
    }

    public class StringCommand : BaseCommand<string>
    {
        public StringCommand(Func<object, bool> canExecuteHandler, Action<string, object> executeHandler) : base(canExecuteHandler, executeHandler)
        {
        }
    }

    public class Vector2Command : BaseCommand<Vector2>
    {
        public Vector2Command(Func<object, bool> canExecuteHandler, Action<Vector2, object> executeHandler) : base(canExecuteHandler, executeHandler)
        {
        }
    }
}