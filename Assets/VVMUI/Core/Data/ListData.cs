using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VVMUI.Core.Data
{
    public interface IListData : IData
    {
        void AddItem(IData data);
        IData GetAt(int index);
        void ParseObject(object data, Action<object, object> onParseItem = null);
        int Count { get; }
        int FocusIndex { get; set; }
        event Action FocusIndexChanged;
    }

    public sealed class ListData
    {
        public static ListData<U> Parse<U>(object data, Action<U, object> onParseItem = null) where U : IData, new()
        {
            ListData<U> obj = new ListData<U>();
            obj.Parse(data, onParseItem);
            return obj;
        }

        public static IListData Parse(Type t, object data, Action<object, object> onParseItem = null)
        {
            if (!typeof(IData).IsAssignableFrom(t))
            {
                return null;
            }

            Type instanceType = typeof(ListData<>).MakeGenericType(t);
            IListData obj = (IListData)Activator.CreateInstance(instanceType);
            obj.ParseObject(data, onParseItem);
            return obj;
        }
    }

    public sealed class ListData<T> : List<T>, IListData where T : IData, new()
    {
        private int _focusIndex = 0;
        public int FocusIndex
        {
            get
            {
                return _focusIndex;
            }
            set
            {
                this._focusIndex = Math.Max(0, Math.Min(value, this.Count - 1));
                FocusIndexChanged.Invoke();
            }
        }

        public event Action FocusIndexChanged;

        public object FastGetValue()
        {
            Debugger.LogError("ListData", "ListData should not call FastGetValue.");
            return null;
        }

        public void FastSetValue(object value)
        {
            Debugger.LogError("ListData", "ListData should not call FastSetValue.");
        }

        public Type GetBindDataType()
        {
            return typeof(IList);
        }

        public DataType GetDataType()
        {
            return DataType.List;
        }

        // 如果是元素内部数据发生改变不能通知到列表数据本身的改变事件
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

        public new T this[int index]
        {
            get
            {
                return base[index];
            }
            set
            {
                base[index] = value;
                InvokeValueChanged();
            }
        }

        public IData GetAt(int index)
        {
            return this[index];
        }

        public void AddItem(IData data)
        {
            this.Add((T)data);
        }

        public new void Add(T item)
        {
            base.Add(item);
            InvokeValueChanged();
        }

        public new void AddRange(IEnumerable<T> collection)
        {
            base.AddRange(collection);
            InvokeValueChanged();
        }

        public new void Clear()
        {
            base.Clear();
            InvokeValueChanged();
        }

        public new void Insert(int index, T item)
        {
            base.Insert(index, item);
            InvokeValueChanged();
        }

        public new void InsertRange(int index, IEnumerable<T> collection)
        {
            base.InsertRange(index, collection);
            InvokeValueChanged();
        }

        public new bool Remove(T item)
        {
            if (base.Remove(item))
            {
                InvokeValueChanged();
                return true;
            }
            return false;
        }

        public new int RemoveAll(Predicate<T> match)
        {
            int count = base.RemoveAll(match);
            if (count > 0)
            {
                InvokeValueChanged();
            }
            return count;
        }

        public new void RemoveAt(int index)
        {
            base.RemoveAt(index);
            InvokeValueChanged();
        }

        public new void RemoveRange(int index, int count)
        {
            base.RemoveRange(index, count);
            InvokeValueChanged();
        }

        public new void Reverse(int index, int count)
        {
            base.Reverse(index, count);
            InvokeValueChanged();
        }

        public new void Reverse()
        {
            base.Reverse();
            InvokeValueChanged();
        }

        public new void Sort(Comparison<T> comparison)
        {
            base.Sort(comparison);
            InvokeValueChanged();
        }

        public new void Sort(int index, int count, IComparer<T> comparer)
        {
            base.Sort(index, count, comparer);
            InvokeValueChanged();
        }

        public new void Sort()
        {
            base.Sort();
            InvokeValueChanged();
        }

        public new void Sort(IComparer<T> comparer)
        {
            base.Sort(comparer);
            InvokeValueChanged();
        }

        public void Instantiate(int count, Action<T> onItemInstaniated)
        {
            List<T> data = new List<T>();
            for (int i = 0; i < count; i++)
            {
                T obj = new T();
                data.Add(obj);
                if (onItemInstaniated != null)
                {
                    onItemInstaniated(obj);
                }
            }
            base.AddRange(data);
            InvokeValueChanged();
        }

        public void CopyFrom(IData data)
        {
            if (!this.GetType().IsAssignableFrom(data.GetType()))
            {
                Debugger.LogError("BaseData", "can not copy data with not the same type");
                return;
            }

            ListData<T> list = (ListData<T>)data;
            if (list == null)
            {
                Debugger.LogError("BaseData", "can not copy data with not the same type");
                return;
            }

            this.Clear();

            List<T> tmp = new List<T>();
            for (int i = 0; i < list.Count; i++)
            {
                T obj = new T();
                obj.CopyFrom(list[i]);
                tmp.Add(obj);
            }
            this.AddRange(tmp);
        }

        public void ParseObject(object data, Action<object, object> onParseItem = null)
        {
            this.Parse(data, onParseItem as Action<T, object>);
        }

        public void Parse(object data, Action<T, object> onParseItem = null)
        {
            IList list = data as IList;
            if (list == null)
            {
                return;
            }

            Type dType = data.GetType();
            Type gType = typeof(T);

            bool isList = gType.IsGenericType && gType.GetGenericTypeDefinition() == typeof(ListData<>);
            bool isDict = gType.IsGenericType && gType.GetGenericTypeDefinition() == typeof(DictionaryData<>);
            bool isStruct = typeof(StructData).IsAssignableFrom(gType);
            bool isBase = gType.BaseType.IsGenericType && gType.BaseType.GetGenericTypeDefinition() == typeof(BaseData<>) && dType.IsGenericType && gType.BaseType.GetGenericArguments()[0] == dType.GetGenericArguments()[0];

            int itr_count = Math.Min(this.Count, list.Count);
            for (int i = 0; i < itr_count; i++)
            {
                if (this[i] != null)
                {
                    if (isList)
                    {
                        (this[i] as IListData).ParseObject(list[i]);
                    }
                    else if (isDict)
                    {
                        (this[i] as IDictionaryData).ParseObject(list[i]);
                    }
                    else if (isStruct)
                    {
                        (this[i] as StructData).Parse(list[i]);
                    }
                    else if (isBase)
                    {
                        (this[i] as IBaseData).FastSetValue(list[i]);
                    }
                }
                else
                {
                    if (isList)
                    {
                        this[i] = (T)ListData.Parse(gType.GetGenericArguments()[0], list[i]);
                    }
                    else if (isDict)
                    {
                        this[i] = (T)DictionaryData.Parse(gType.GetGenericArguments()[0], list[i]);
                    }
                    else if (isStruct)
                    {
                        this[i] = (T)StructData.Parse(gType, list[i]);
                    }
                    else if (isBase)
                    {
                        this[i] = (T)Activator.CreateInstance(gType, list[i]);
                    }
                }
                onParseItem?.Invoke(this[i], list[i]);
            }

            if (list.Count > this.Count)
            {
                List<T> new_data_range = new List<T>();
                for (int i = itr_count; i < list.Count; i++)
                {
                    object new_data = null;
                    if (isList)
                    {
                        new_data = ListData.Parse(gType.GetGenericArguments()[0], list[i]);
                    }
                    else if (isDict)
                    {
                        new_data = DictionaryData.Parse(gType.GetGenericArguments()[0], list[i]);
                    }
                    else if (isStruct)
                    {
                        new_data = StructData.Parse(gType, list[i]);
                    }
                    else if (isBase)
                    {
                        new_data = Activator.CreateInstance(gType, list[i]);
                    }
                    new_data_range.Add((T)new_data);
                    onParseItem?.Invoke((T)new_data, list[i]);
                }
                this.AddRange(new_data_range);
            }
            else if (list.Count < this.Count)
            {
                this.RemoveRange(itr_count, this.Count - list.Count);
            }
        }
    }
}