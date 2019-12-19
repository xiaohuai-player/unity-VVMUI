using System;
using System.Collections;
using System.Collections.Generic;

namespace VVMUI.Core.Data {
    public interface IListData {
        IData GetAt (int index);
        void InvokeItemValueChanged (int i);
        void AddItemValueChangedListener (int i, Action h);
        void RemoveItemValueChangedListener (int i, Action h);
    }

    public class ListData<T> : List<T>, IListData, IData where T : IData {
        public event Action ValueChanged;

        public void InvokeValueChanged () {
            if (ValueChanged != null) {
                ValueChanged.Invoke ();
            }
        }

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
    }
}