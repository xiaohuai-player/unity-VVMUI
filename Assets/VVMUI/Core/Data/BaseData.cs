using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace VVMUI.Core.Data {
    public abstract class BaseData<T> : IData<T>, IData {
        public BaseData () {

        }

        public BaseData (T value) {
            _value = value;
        }

        [SerializeField]
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

        private ISetValue _setter;
        public ISetValue Setter {
            get {
                if (_setter == null) {
                    Type dataType = this.GetType ();
                    MethodInfo setMethod = dataType.GetMethod ("Set");
                    if (setMethod != null) {
                        _setter = SetterWrapper.CreateMethodSetterWrapper (setMethod);
                    }
                }
                return _setter;
            }
        }

        private IGetValue _getter;
        public IGetValue Getter {
            get {
                if (_getter == null) {
                    Type dataType = this.GetType ();
                    MethodInfo getMethod = dataType.GetMethod ("Get");
                    if (getMethod != null) {
                        _getter = GetterWrapper.CreateMethodGetterWrapper (getMethod);
                    }
                }
                return _getter;
            }
        }
    }
}