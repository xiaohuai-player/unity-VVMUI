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
            typeof(Texture2D),
            typeof(Vector2),
            typeof(Vector3)
        };
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

        private List<Action> _valueChangedHandlers = new List<Action>();

        public void InvokeValueChanged()
        {
            for (int i = 0; i < _valueChangedHandlers.Count; i++)
            {
                _valueChangedHandlers[i].Invoke();
            }
        }

        public void AddValueChangedListener(Action handler)
        {
            _valueChangedHandlers.Add(handler);
        }

        public void RemoveValueChangedListener(Action handler)
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
                        _setter = SetterWrapper.CreateMethodSetterWrapper(setMethod);
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
                        _getter = GetterWrapper.CreateMethodGetterWrapper(getMethod);
                    }
                }
                return _getter;
            }
        }
    }
}