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

        public void Set (T arg) {
            _value = arg;
            InvokeValueChanged ();
        }
    }
}