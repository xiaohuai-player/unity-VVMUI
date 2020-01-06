using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace VVMUI.Core.Data {
    public abstract class StructData : IData {
        protected Dictionary<string, IData> _allData = new Dictionary<string, IData> ();
        private bool _fieldsInit = false;

        private void InitFields () {
            Type type = this.GetType ();
            FieldInfo[] fields = type.GetFields ();
            for (int i = 0; i < fields.Length; i++) {
                FieldInfo fi = fields[i];
                if (fi.FieldType.GetInterface ("IData") != null) {
                    IData data = fields[i].GetValue (this) as IData;
                    if (data != null) {
                        _allData[fields[i].Name] = data;
                        data.ValueChanged += InvokeValueChanged;
                    }
                }
            }
            _fieldsInit = true;
        }

        public event Action ValueChanged;
        public void InvokeValueChanged () {
            if (ValueChanged != null) {
                ValueChanged.Invoke ();
            }
        }

        public IData this [string key] {
            get {
                if (!_fieldsInit) {
                    InitFields ();
                }
                IData v = null;
                _allData.TryGetValue (key, out v);
                return v;
            }
        }

        public Type GetDataType () {
            return typeof (object);
        }

        public object GetGetterDelegate () {
            return null;
        }

        public object GetSetterDelegate () {
            return null;
        }
    }
}