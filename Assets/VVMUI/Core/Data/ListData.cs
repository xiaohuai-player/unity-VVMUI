using System;
using System.Collections;
using System.Collections.Generic;

namespace VVMUI.Core.Data {
    public interface IListData {
        IData GetAt (int index);
        void InvokeItemValueChanged (int i);
        void AddItemValueChangedListener (int i, Action h);
        void RemoveItemValueChangedListener (int i, Action h);
        void Parse (object data);
    }

    public sealed class ListData {
        public static ListData<U> Parse<U> (object data) where U : IData {
            return (ListData<U>) Parse (typeof (U), data);
        }

        public static IListData Parse (Type t, object data) {
            if (t.GetInterface ("IData") == null) {
                return null;
            }

            Type instanceType = typeof (ListData<>).MakeGenericType (t);
            IListData obj = (IListData) Activator.CreateInstance (instanceType);
            obj.Parse (data);
            return obj;
        }
    }

    public sealed class ListData<T> : List<T>, IListData, IData where T : IData {
        public Type GetDataType () {
            return typeof (T);
        }

        // 列表数据结构，在列表项数量发生变化时才会触发 ValueChanged，仅数据内容变化时触发 ItemValueChanged
        public event Action ValueChanged;
        public void InvokeValueChanged () {
            if (ValueChanged != null) {
                ValueChanged.Invoke ();
            }
        }

        // 列表数据结构，在列表项数量发生变化时才会触发 ValueChanged，仅数据内容变化时触发 ItemValueChanged
        public Dictionary<int, List<Action>> ItemValueChanged = new Dictionary<int, List<Action>> ();
        public void InvokeItemValueChanged (int i) {
            List<Action> handlers = null;
            if (ItemValueChanged.TryGetValue (i, out handlers) && handlers != null) {
                for (int j = 0; j < handlers.Count; j++) {
                    handlers[j].Invoke ();
                }
            }
        }

        public void AddItemValueChangedListener (int i, Action h) {
            List<Action> handlers = null;
            if (!ItemValueChanged.TryGetValue (i, out handlers)) {
                handlers = new List<Action> ();
                ItemValueChanged[i] = handlers;
            }
            handlers.Add (h);
        }

        public void RemoveItemValueChangedListener (int i, Action h) {
            List<Action> handlers = null;
            if (ItemValueChanged.TryGetValue (i, out handlers)) {
                handlers.Remove (h);
            }
        }

        public new T this [int index] {
            get {
                return (T) base[index];
            }
            set {
                base[index] = value;
                InvokeItemValueChanged (index);
            }
        }

        public IData GetAt (int index) {
            return this [index];
        }

        public new void Add (T item) {
            base.Add (item);
            InvokeValueChanged ();
        }

        public new void AddRange (IEnumerable<T> collection) {
            base.AddRange (collection);
            InvokeValueChanged ();
        }

        public new void Clear () {
            base.Clear ();
            InvokeValueChanged ();
        }

        public new void Insert (int index, T item) {
            int count = this.Count;
            base.Insert (index, item);
            for (int i = index; i < count; i++) {
                InvokeItemValueChanged (i);
            }
            InvokeValueChanged ();
        }

        public new void InsertRange (int index, IEnumerable<T> collection) {
            int count = this.Count;
            base.InsertRange (index, collection);
            for (int i = index; i < count; i++) {
                InvokeItemValueChanged (i);
            }
            InvokeValueChanged ();
        }

        public new bool Remove (T item) {
            if (base.Remove (item)) {
                for (int i = 0; i < this.Count; i++) {
                    InvokeItemValueChanged (i);
                }
                InvokeValueChanged ();
                return true;
            }
            return false;
        }

        public new int RemoveAll (Predicate<T> match) {
            int count = base.RemoveAll (match);
            if (count > 0) {
                for (int i = 0; i < this.Count; i++) {
                    InvokeItemValueChanged (i);
                }
                InvokeValueChanged ();
            }
            return count;
        }

        public new void RemoveAt (int index) {
            base.RemoveAt (index);
            for (int i = 0; i < this.Count; i++) {
                InvokeItemValueChanged (i);
            }
            InvokeValueChanged ();
        }

        public new void RemoveRange (int index, int count) {
            base.RemoveRange (index, count);
            for (int i = 0; i < this.Count; i++) {
                InvokeItemValueChanged (i);
            }
            InvokeValueChanged ();
        }

        public new void Reverse (int index, int count) {
            base.Reverse (index, count);
            for (int i = 0; i < count; i++) {
                InvokeItemValueChanged (i);
            }
        }

        public new void Reverse () {
            base.Reverse ();
            for (int i = 0; i < this.Count; i++) {
                InvokeItemValueChanged (i);
            }
        }

        public new void Sort (Comparison<T> comparison) {
            base.Sort (comparison);
            for (int i = 0; i < this.Count; i++) {
                InvokeItemValueChanged (i);
            }
        }

        public new void Sort (int index, int count, IComparer<T> comparer) {
            base.Sort (index, count, comparer);
            for (int i = 0; i < this.Count; i++) {
                InvokeItemValueChanged (i);
            }
        }

        public new void Sort () {
            base.Sort ();
            for (int i = 0; i < this.Count; i++) {
                InvokeItemValueChanged (i);
            }
        }

        public new void Sort (IComparer<T> comparer) {
            base.Sort (comparer);
            for (int i = 0; i < this.Count; i++) {
                InvokeItemValueChanged (i);
            }
        }

        public void Parse (object data) {
            IList list = data as IList;
            if (list == null) {
                return;
            }

            Type gType = typeof (T);
            if (gType.IsGenericType && gType.GetGenericTypeDefinition () == typeof (ListData<>)) {
                int itr_count = Math.Min (this.Count, list.Count);
                for (int i = 0; i < itr_count; i++) {
                    this [i] = (T) ListData.Parse (gType, list[i]);
                }
                if (list.Count > this.Count) {
                    List<T> new_data = new List<T> ();
                    for (int i = itr_count; i < list.Count; i++) {
                        new_data.Add ((T) ListData.Parse (gType, list[i]));
                    }
                    this.AddRange (new_data);
                } else if (list.Count < this.Count) {
                    this.RemoveRange (itr_count, this.Count - list.Count);
                }
            } else if (gType.BaseType == typeof (StructData)) {
                int itr_count = Math.Min (this.Count, list.Count);
                for (int i = 0; i < itr_count; i++) {
                    this [i] = (T) StructData.Parse (gType, list[i]);
                }
                if (list.Count > this.Count) {
                    List<T> new_data = new List<T> ();
                    for (int i = itr_count; i < list.Count; i++) {
                        new_data.Add ((T) StructData.Parse (gType, list[i]));
                    }
                    this.AddRange (new_data);
                } else if (list.Count < this.Count) {
                    this.RemoveRange (itr_count, this.Count - list.Count);
                }
            } else if (gType.BaseType.IsGenericType && gType.BaseType.GetGenericTypeDefinition () == typeof (BaseData<>)) {
                int itr_count = Math.Min (this.Count, list.Count);
                for (int i = 0; i < itr_count; i++) {
                    (this [i] as IData).Setter.Set (this [i], list[i]);
                }
                if (list.Count > this.Count) {
                    List<T> new_data = new List<T> ();
                    for (int i = itr_count; i < list.Count; i++) {
                        new_data.Add ((T) Activator.CreateInstance (gType, list[i]));
                    }
                    this.AddRange (new_data);
                } else if (list.Count < this.Count) {
                    this.RemoveRange (itr_count, this.Count - list.Count);
                }
            }
        }

        public ISetValue Setter {
            get {
                return null;
            }
        }

        public IGetValue Getter {
            get {
                return null;
            }
        }
    }
}