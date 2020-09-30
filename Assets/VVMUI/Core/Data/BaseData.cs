using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace VVMUI.Core.Data
{
    public sealed class BaseData
    {
        public static readonly List<Type> SupportDataType = new List<Type>() {
            typeof(bool),
            typeof(Color),
            typeof(double),
            typeof(float),
            typeof(int),
            typeof(long),
            typeof(Sprite),
            typeof(string),
            typeof(Texture),
            typeof(Vector2),
            typeof(Vector3)
        };

        //TODO 需要硬写支持绑定的组件类型
    }

    public abstract class BaseData<T> : IData<T>, IData
    {
        public BaseData()
        {

        }

        public BaseData(T value)
        {
            _value = value;
        }

        [SerializeField]
        private T _value;

        private List<DataChangedHandler> _valueChangedHandlers = new List<DataChangedHandler>();

        public void InvokeValueChanged()
        {
            for (int i = 0; i < _valueChangedHandlers.Count; i++)
            {
                _valueChangedHandlers[i].Invoke(this);
            }
        }

        public void AddValueChangedListener(DataChangedHandler handler)
        {
            _valueChangedHandlers.Add(handler);
        }

        public void RemoveValueChangedListener(DataChangedHandler handler)
        {
            _valueChangedHandlers.Remove(handler);
        }

        public Type GetDataType()
        {
            return typeof(T);
        }

        public T Get()
        {
            return _value;
        }

        public void Set(T arg)
        {
            _value = arg;
            InvokeValueChanged();
        }

        public void CopyFrom(IData data)
        {
            if (!this.GetType().IsAssignableFrom(data.GetType()))
            {
                Debug.Log("can not copy data with not the same type");
                return;
            }

            BaseData<T> d = (BaseData<T>)data;
            if (d == null)
            {
                Debug.Log("can not copy data with not the same type");
                return;
            }

            this.Set(d.Get());
        }

        private ISetValue _setter;
        public ISetValue Setter
        {
            get
            {
                if (_setter == null)
                {
                    Type dataType = this.GetType();
                    MethodInfo setMethod = dataType.GetMethod("Set");
                    if (setMethod != null)
                    {
                        _setter = SetterWrapper.CreateMethodSetterWrapper<BaseData<T>, T>(setMethod);
                    }
                }
                return _setter;
            }
        }

        private IGetValue _getter;
        public IGetValue Getter
        {
            get
            {
                if (_getter == null)
                {
                    Type dataType = this.GetType();
                    MethodInfo getMethod = dataType.GetMethod("Get");
                    if (getMethod != null)
                    {
                        _getter = GetterWrapper.CreateMethodGetterWrapper<BaseData<T>, T>(getMethod);
                    }
                }
                return _getter;
            }
        }
    }
}