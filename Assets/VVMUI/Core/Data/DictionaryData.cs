using System;
using System.Collections;
using System.Collections.Generic;

namespace VVMUI.Core.Data
{
    public interface IDictionaryData
    {
        IData Get(string key);
        void InvokeItemValueChanged(string key);
        void AddItemValueChangedListener(string key, Action h);
        void RemoveItemValueChangedListener(string key, Action h);
        void ParseObject(object data, Action<object, object> onParseItem = null);
    }

    public sealed class DictionaryData
    {
        public static DictionaryData<T> Parse<T>(object data, Action<T, object> onParseItem = null) where T : IData
        {
            DictionaryData<T> obj = new DictionaryData<T>();
            obj.Parse(data, onParseItem);
            return obj;
        }

        public static object Parse(Type t, object data, Action<object, object> onParseItem = null)
        {
            if (t.GetInterface("IData") == null)
            {
                return null;
            }

            Type instanceType = typeof(DictionaryData<>).MakeGenericType(t);
            IDictionaryData obj = (IDictionaryData)Activator.CreateInstance(instanceType);
            obj.ParseObject(data, onParseItem);
            return obj;
        }
    }

    public sealed class DictionaryData<T> : Dictionary<string, T>, IDictionaryData, IData where T : IData
    {
        public IData Get(string key)
        {
            return this[key];
        }

        public Type GetDataType()
        {
            return typeof(T);
        }

        // 字典数据结构，在字典项数量发生变化时才会触发 ValueChanged，仅数据内容变化时触发 ItemValueChanged
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

        // 字典数据结构，在字典项数量发生变化时才会触发 ValueChanged，仅数据内容变化时触发 ItemValueChanged
        public Dictionary<string, List<Action>> ItemValueChanged = new Dictionary<string, List<Action>>();
        public void InvokeItemValueChanged(string k)
        {
            List<Action> handlers = null;
            if (ItemValueChanged.TryGetValue(k, out handlers) && handlers != null)
            {
                for (int j = 0; j < handlers.Count; j++)
                {
                    handlers[j].Invoke();
                }
            }
        }

        public void AddItemValueChangedListener(string k, Action h)
        {
            List<Action> handlers = null;
            if (!ItemValueChanged.TryGetValue(k, out handlers))
            {
                handlers = new List<Action>();
                ItemValueChanged[k] = handlers;
            }
            handlers.Add(h);
        }

        public void RemoveItemValueChangedListener(string k, Action h)
        {
            List<Action> handlers = null;
            if (ItemValueChanged.TryGetValue(k, out handlers))
            {
                handlers.Remove(h);
            }
        }

        public new T this[string key]
        {
            get
            {
                return base[key];
            }
            set
            {
                int oldcount = this.Keys.Count;
                base[key] = value;
                int newcount = this.Keys.Count;
                if (newcount > oldcount)
                {
                    InvokeValueChanged();
                }
                else
                {
                    InvokeItemValueChanged(key);
                }
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
            bool isStruct = gType.BaseType == typeof(StructData);
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
                        (this[key] as IData).Setter.Set(this[key], dict[key]);
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

        public ISetValue Setter
        {
            get
            {
                return null;
            }
        }

        public IGetValue Getter
        {
            get
            {
                return null;
            }
        }
    }
}