using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VVMUI.Core.Data
{
    public interface IDictionaryData : IData
    {
        IData Get(string key);
        void ParseObject(object data, Action<object, object> onParseItem = null);
    }

    public sealed class DictionaryData
    {
        public static DictionaryData<T> Parse<T>(object data, Action<T, object> onParseItem = null) where T : IData, new()
        {
            DictionaryData<T> obj = new DictionaryData<T>();
            obj.Parse(data, onParseItem);
            return obj;
        }

        public static object Parse(Type t, object data, Action<object, object> onParseItem = null)
        {
            if (!typeof(IData).IsAssignableFrom(t))
            {
                return null;
            }

            Type instanceType = typeof(DictionaryData<>).MakeGenericType(t);
            IDictionaryData obj = (IDictionaryData)Activator.CreateInstance(instanceType);
            obj.ParseObject(data, onParseItem);
            return obj;
        }
    }

    public sealed class DictionaryData<T> : Dictionary<string, T>, IDictionaryData where T : IData, new()
    {
        public IData Get(string key)
        {
            return this[key];
        }

        public Type GetBindDataType()
        {
            return typeof(IDictionary);
        }

        public DataType GetDataType()
        {
            return DataType.Dictionary;
        }

        // 如果是元素内部数据发生改变不能通知到字典数据本身的改变事件
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

        public object FastGetValue()
        {
            Debugger.LogError("DictionaryData", "DictionaryData should not call FastGetValue.");
            return null;
        }

        public void FastSetValue(object value)
        {
            Debugger.LogError("DictionaryData", "DictionaryData should not call FastSetValue.");
        }

        public new T this[string key]
        {
            get
            {
                return base[key];
            }
            set
            {
                base[key] = value;
                InvokeValueChanged();
            }
        }

        public new void Add(string key, T value)
        {
            base.Add(key, value);
            InvokeValueChanged();
        }

        public new void Clear()
        {
            base.Clear();
            InvokeValueChanged();
        }

        public new bool Remove(string key)
        {
            bool f = base.Remove(key);
            if (f)
            {
                InvokeValueChanged();
            }
            return f;
        }

        public void CopyFrom(IData data)
        {
            if (!this.GetType().IsAssignableFrom(data.GetType()))
            {
                Debugger.LogError("BaseData", "can not copy data with not the same type");
                return;
            }

            DictionaryData<T> dict = (DictionaryData<T>)data;
            if (dict == null)
            {
                Debugger.LogError("BaseData", "can not copy data with not the same type");
                return;
            }

            this.Clear();
            foreach (KeyValuePair<string, T> kv in dict)
            {
                T obj = new T();
                obj.CopyFrom(kv.Value);
                this[kv.Key] = obj;
            }
        }

        public void ParseObject(object data, Action<object, object> onParseItem = null)
        {
            this.Parse(data, onParseItem as Action<T, object>);
        }

        public void Parse(object data, Action<T, object> onParseItem = null)
        {
            IDictionary dict = data as IDictionary;
            if (dict == null)
            {
                return;
            }

            Type dType = data.GetType();
            Type gType = typeof(T);

            if (!dType.IsGenericType || dType.GetGenericArguments().Length != 2 || dType.GetGenericArguments()[0] != typeof(string))
            {
                return;
            }

            bool isList = gType.IsGenericType && gType.GetGenericTypeDefinition() == typeof(ListData<>);
            bool isDict = gType.IsGenericType && gType.GetGenericTypeDefinition() == typeof(DictionaryData<>);
            bool isStruct = typeof(StructData).IsAssignableFrom(gType);
            bool isBase = gType.BaseType.IsGenericType && gType.BaseType.GetGenericTypeDefinition() == typeof(BaseData<>) && dType.IsGenericType && gType.BaseType.GetGenericArguments()[0] == dType.GetGenericArguments()[1];

            foreach (string key in dict.Keys)
            {
                if (this.ContainsKey(key) && this[key] != null)
                {
                    if (isList)
                    {
                        (this[key] as IListData).ParseObject(dict[key]);
                    }
                    else if (isDict)
                    {
                        (this[key] as IDictionaryData).ParseObject(dict[key]);
                    }
                    else if (isStruct)
                    {
                        (this[key] as StructData).Parse(dict[key]);
                    }
                    else if (isBase)
                    {
                        (this[key] as IBaseData).FastSetValue(dict[key]);
                    }
                }
                else
                {
                    if (isList)
                    {
                        this[key] = (T)ListData.Parse(gType.GetGenericArguments()[0], dict[key]);
                    }
                    else if (isDict)
                    {
                        this[key] = (T)DictionaryData.Parse(gType.GetGenericArguments()[0], dict[key]);
                    }
                    else if (isStruct)
                    {
                        this[key] = (T)StructData.Parse(gType, dict[key]);
                    }
                    else if (isBase)
                    {
                        this[key] = (T)Activator.CreateInstance(gType, dict[key]);
                    }
                }
                if (onParseItem != null)
                {
                    onParseItem(this[key], dict[key]);
                }
            }
        }
    }
}