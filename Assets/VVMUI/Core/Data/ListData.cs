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
        int Count { get; }
        int FocusIndex { get; set; }
        event Action FocusIndexChanged;
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
        private int _focusIndex = 0;
        public int FocusIndex {
            get {
                return _focusIndex;
            }
            set {
                this._focusIndex = Math.Max (0, Math.Min (value, this.Count - 1));
                FocusIndexChanged.Invoke ();
            }
        }

        public event Action FocusIndexChanged;

        public Type GetDataType () {
            return typeof (T);
        }

        // 列表数据结构，在列表项数量发生变化时才会触发 ValueChanged，仅数据内容变化时触发 ItemValueChanged
        private List<Action> _valueChangedHandlers = new List<Action> ();
        public void InvokeValueChanged () {
            for (int i = 0; i < _valueChangedHandlers.Count; i++) {
                _valueChangedHandlers[i].Invoke ();
            }
        }
        public void AddValueChangedListener (Action handler) {
            _valueChangedHandlers.Add (handler);
        }
        public void RemoveValueChangedListener (Action handler) {
            _valueChangedHandlers.Remove (handler);
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
            // for (int i = index; i < count; i++) {
            //     InvokeItemValueChanged (i);
            // }
            InvokeValueChanged ();
        }

        public new void InsertRange (int index, IEnumerable<T> collection) {
            int count = this.Count;
            base.InsertRange (index, collection);
            // for (int i = index; i < count; i++) {
            //     InvokeItemValueChanged (i);
            // }
            InvokeValueChanged ();
        }

        public new bool Remove (T item) {
            if (base.Remove (item)) {
                // for (int i = 0; i < this.Count; i++) {
                //     InvokeItemValueChanged (i);
                // }
                InvokeValueChanged ();
                return true;
            }
            return false;
        }

        public new int RemoveAll (Predicate<T> match) {
            int count = base.RemoveAll (match);
            if (count > 0) {
                // for (int i = 0; i < this.Count; i++) {
                //     InvokeItemValueChanged (i);
                // }
                InvokeValueChanged ();
            }
            return count;
        }

        public new void RemoveAt (int index) {
            base.RemoveAt (index);
            // for (int i = 0; i < this.Count; i++) {
            //     InvokeItemValueChanged (i);
            // }
            InvokeValueChanged ();
        }

        public new void RemoveRange (int index, int count) {
            base.RemoveRange (index, count);
            // for (int i = 0; i < this.Count; i++) {
            //     InvokeItemValueChanged (i);
            // }
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

            Type dType = data.GetType ();
            Type gType = typeof (T);

            bool isList = gType.IsGenericType && gType.GetGenericTypeDefinition () == typeof (ListData<>);
            bool isDict = gType.IsGenericType && gType.GetGenericTypeDefinition () == typeof (DictionaryData<>);
            bool isStruct = gType.BaseType == typeof (StructData);
            bool isBase = gType.BaseType.IsGenericType && gType.BaseType.GetGenericTypeDefinition () == typeof (BaseData<>) && dType.IsGenericType && gType.BaseType.GetGenericArguments () [0] == dType.GetGenericArguments () [0];

            int itr_count = Math.Min (this.Count, list.Count);
            for (int i = 0; i < itr_count; i++) {
                if (this [i] != null) {
                    if (isList) {
                        (this [i] as IListData).Parse (list[i]);
                    } else if (isDict) {
                        (this [i] as IDictionaryData).Parse (list[i]);
                    } else if (isStruct) {
                        (this [i] as StructData).Parse (list[i]);
                    } else if (isBase) {
                        (this [i] as IData).Setter.Set (this [i], list[i] as IData);
                    }
                } else {
                    if (isList) {
                        this [i] = (T) ListData.Parse (gType.GetGenericArguments () [0], list[i]);
                    } else if (isDict) {
                        this [i] = (T) DictionaryData.Parse (gType.GetGenericArguments () [0], list[i]);
                    } else if (isStruct) {
                        this [i] = (T) StructData.Parse (gType, list[i]);
                    } else if (isBase) {
                        this [i] = (T) Activator.CreateInstance (gType, list[i]);
                    }
                }
            }

            if (list.Count > this.Count) {
                List<T> new_data_range = new List<T> ();
                for (int i = itr_count; i < list.Count; i++) {
                    object new_data = null;
                    if (isList) {
                        new_data = ListData.Parse (gType.GetGenericArguments () [0], list[i]);
                    } else if (isDict) {
                        new_data = DictionaryData.Parse (gType.GetGenericArguments () [0], list[i]);
                    } else if (isStruct) {
                        new_data = StructData.Parse (gType, list[i]);
                    } else if (isBase) {
                        new_data = Activator.CreateInstance (gType, list[i]);
                    }
                    new_data_range.Add ((T) new_data);
                }
                this.AddRange (new_data_range);
            } else if (list.Count < this.Count) {
                this.RemoveRange (itr_count, this.Count - list.Count);
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