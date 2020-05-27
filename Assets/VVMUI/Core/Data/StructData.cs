using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace VVMUI.Core.Data {
    public abstract class StructData : IData {
        protected Dictionary<string, IData> _allData = new Dictionary<string, IData> ();
        private bool _fieldsInit = false;

        private void InitFields () {
            if (_fieldsInit) {
                return;
            }

            Type type = this.GetType ();
            FieldInfo[] fields = type.GetFields ();
            for (int i = 0; i < fields.Length; i++) {
                FieldInfo fi = fields[i];
                if (fi.FieldType.GetInterface ("IData") != null) {
                    IData data = fields[i].GetValue (this) as IData;
                    if (data != null) {
                        _allData[fields[i].Name] = data;
                        data.AddValueChangedListener(this.InvokeValueChanged);
                    }
                }
            }

            _fieldsInit = true;
        }

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

        public IData this [string key] {
            get {
                InitFields ();
                IData v = null;
                _allData.TryGetValue (key, out v);
                return v;
            }
        }

        public Dictionary<string, IData> Fields {
            get {
                InitFields ();
                return _allData;
            }
        }

        public Type GetDataType () {
            return typeof (object);
        }

        public void Parse (object data) {
            Type objtype = this.GetType ();
            Type datatype = data.GetType ();
            FieldInfo[] datafields = datatype.GetFields ();
            for (int i = 0; i < datafields.Length; i++) {
                FieldInfo datafi = datafields[i];
                object datav = datafi.GetValue (data);
                if (datav == null) {
                    continue;
                }

                FieldInfo objfi = objtype.GetField (datafi.Name);
                if (objfi == null || objfi.FieldType.GetInterface ("IData") == null) {
                    continue;
                }

                object objv = objfi.GetValue (this);

                bool isList = objfi.FieldType.IsGenericType && objfi.FieldType.GetGenericTypeDefinition () == typeof (ListData<>);
                bool isDict = objfi.FieldType.IsGenericType && objfi.FieldType.GetGenericTypeDefinition () == typeof (DictionaryData<>);
                bool isStruct = objfi.FieldType.BaseType == typeof (StructData);
                bool isBase = objfi.FieldType.BaseType.IsGenericType && objfi.FieldType.BaseType.GetGenericTypeDefinition () == typeof (BaseData<>) && objfi.FieldType.BaseType.GetGenericArguments()[0] == datafi.FieldType;

                if (objv != null) {
                    if (isList) {
                        (objv as IListData).Parse (datav);
                    } else if (isDict) {
                        (objv as IDictionaryData).Parse (datav);
                    } else if (isStruct) {
                        (objv as StructData).Parse (datav);
                    } else if (isBase) {
                        (objv as IData).Setter.Set (objv, datav);
                    }
                } else {
                    if (isList) {
                        objfi.SetValue (this, ListData.Parse (objfi.FieldType.GetGenericArguments () [0], datav));
                    } else if (isDict) {
                        objfi.SetValue (this, DictionaryData.Parse (objfi.FieldType.GetGenericArguments () [0], datav));
                    } else if (isStruct) {
                        objfi.SetValue (this, StructData.Parse (objfi.FieldType, datav));
                    } else if (isBase) {
                        objfi.SetValue (this, Activator.CreateInstance (objfi.FieldType, datav));
                    }
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

        public static T Parse<T> (object data) where T : StructData, new () {
            return (T) Parse (typeof (T), data);
        }

        public static object Parse (Type t, object data) {
            if (t.BaseType != typeof (StructData)) {
                return null;
            }

            StructData obj = (StructData) Activator.CreateInstance (t);
            obj.Parse (data);
            return obj;
        }
    }
}