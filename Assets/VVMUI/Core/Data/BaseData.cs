using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VVMUI.Core.Data {
    public abstract class BaseData<T> : IData<T>, IData {
        private T _value;

        public event Action ValueChanged;

        public void InvokeValueChanged () {
            if (ValueChanged != null) {
                ValueChanged.Invoke ();
            }
        }

        public Type GetDataType () {
            return typeof (T);
        }

        public T Get () {
            return _value;
        }

        private Func<T> _getterDelegate;
        public object GetGetterDelegate () {
            if (_getterDelegate == null) {
                _getterDelegate = new Func<T> (this.Get);
            }
            return _getterDelegate;
        }

        public void Set (T arg) {
            _value = arg;
            InvokeValueChanged ();
        }

        private Action<T> _setterDelegate;
        public object GetSetterDelegate () {
            if (_setterDelegate == null) {
                _setterDelegate = new Action<T> (this.Set);
            }
            return _setterDelegate;
        }
    }
}