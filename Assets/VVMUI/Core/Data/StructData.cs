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
                if (objfi.FieldType.IsGenericType && objfi.FieldType.GetGenericTypeDefinition () == typeof (ListData<>)) {
                    IListData v = (IListData) objv;
                    if (v == null) {
                        Type genericTypee = objfi.FieldType.GetGenericArguments () [0];
                        objfi.SetValue (this, ListData.Parse (genericTypee, datav));
                    } else {
                        v.Parse (datav);
                    }
                } else if (objfi.FieldType.BaseType == typeof (StructData)) {
                    StructData v = (StructData) objv;
                    if (v == null) {
                        objfi.SetValue (this, StructData.Parse (objfi.FieldType, datav));
                    } else {
                        v.Parse (datafi.GetValue (data));
                    }
                } else if (objfi.FieldType.BaseType.IsGenericType && objfi.FieldType.BaseType.GetGenericTypeDefinition () == typeof (BaseData<>)) {
                    Type[] gTypes = objfi.FieldType.BaseType.GetGenericArguments ();
                    if (gTypes.Length > 0 && gTypes[0] == datafi.FieldType) {
                        if (objv == null) {
                            objfi.SetValue (this, Activator.CreateInstance (objfi.FieldType, datav));
                        } else {
                            (objv as IData).Setter.Set (objv, datav);
                        }
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