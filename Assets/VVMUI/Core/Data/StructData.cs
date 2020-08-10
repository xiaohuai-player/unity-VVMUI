using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace VVMUI.Core.Data
{
    public abstract class StructData : IData
    {
        protected Dictionary<string, IData> _allData = new Dictionary<string, IData>();
        private bool _fieldsInit = false;

        private void InitFields()
        {
            if (_fieldsInit)
            {
                return;
            }

            Type type = this.GetType();
            FieldInfo[] fields = type.GetFields();
            for (int i = 0; i < fields.Length; i++)
            {
                FieldInfo fi = fields[i];
                if (fi.FieldType.GetInterface("IData") != null)
                {
                    IData data = fields[i].GetValue(this) as IData;
                    if (data != null)
                    {
                        _allData[fields[i].Name] = data;
                        data.AddValueChangedListener(this.InvokeValueChanged);
                    }
                }
            }

            _fieldsInit = true;
        }

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

        public IData this[string key]
        {
            get
            {
                InitFields();
                IData v = null;
                _allData.TryGetValue(key, out v);
                return v;
            }
        }

        public Dictionary<string, IData> Fields
        {
            get
            {
                InitFields();
                return _allData;
            }
        }

        public Type GetDataType()
        {
            return typeof(object);
        }

        private class ParseStruct
        {
            public string Name;
            public Type Type;
            public object Value;
        }

        public void Parse(object data)
        {
            Type objtype = this.GetType();
            Type datatype = data.GetType();

            List<ParseStruct> all = new List<ParseStruct>();

            FieldInfo[] datafields = datatype.GetFields();
            for (int i = 0; i < datafields.Length; i++)
            {
                FieldInfo datafi = datafields[i];
                object datav = datafi.GetValue(data);
                if (datav == null)
                {
                    continue;
                }

                all.Add(new ParseStruct()
                {
                    Name = datafi.Name,
                    Type = datafi.FieldType,
                    Value = datav
                });
            }

            PropertyInfo[] dataproperties = datatype.GetProperties();
            for (int i = 0; i < dataproperties.Length; i++)
            {
                PropertyInfo property = dataproperties[i];
                object datav = property.GetValue(data);
                if (datav == null)
                {
                    continue;
                }

                all.Add(new ParseStruct()
                {
                    Name = property.Name,
                    Type = property.PropertyType,
                    Value = datav
                });
            }

            for (int i = 0; i < all.Count; i++)
            {
                ParseStruct parseStruct = all[i];

                FieldInfo objfi = objtype.GetField(parseStruct.Name);
                if (objfi == null || objfi.FieldType.GetInterface("IData") == null)
                {
                    continue;
                }

                bool isList = objfi.FieldType.IsGenericType && objfi.FieldType.GetGenericTypeDefinition() == typeof(ListData<>);
                bool isDict = objfi.FieldType.IsGenericType && objfi.FieldType.GetGenericTypeDefinition() == typeof(DictionaryData<>);
                bool isStruct = objfi.FieldType.BaseType == typeof(StructData);
                bool isBase = objfi.FieldType.BaseType.IsGenericType && objfi.FieldType.BaseType.GetGenericTypeDefinition() == typeof(BaseData<>) && objfi.FieldType.BaseType.GetGenericArguments()[0] == parseStruct.Type;

                object objv = objfi.GetValue(this);
                if (objv != null)
                {
                    if (isList)
                    {
                        (objv as IListData).ParseObject(parseStruct.Value);
                    }
                    else if (isDict)
                    {
                        (objv as IDictionaryData).ParseObject(parseStruct.Value);
                    }
                    else if (isStruct)
                    {
                        (objv as StructData).Parse(parseStruct.Value);
                    }
                    else if (isBase)
                    {
                        (objv as IData).Setter.Set(objv, parseStruct.Value);
                    }
                }
                else
                {
                    if (isList)
                    {
                        objfi.SetValue(this, ListData.Parse(objfi.FieldType.GetGenericArguments()[0], parseStruct.Value));
                    }
                    else if (isDict)
                    {
                        objfi.SetValue(this, DictionaryData.Parse(objfi.FieldType.GetGenericArguments()[0], parseStruct.Value));
                    }
                    else if (isStruct)
                    {
                        objfi.SetValue(this, StructData.Parse(objfi.FieldType, parseStruct.Value));
                    }
                    else if (isBase)
                    {
                        objfi.SetValue(this, Activator.CreateInstance(objfi.FieldType, parseStruct.Value));
                    }
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

        public static T Parse<T>(object data) where T : StructData, new()
        {
            return (T)Parse(typeof(T), data);
        }

        public static object Parse(Type t, object data)
        {
            if (t.BaseType != typeof(StructData))
            {
                return null;
            }

            StructData obj = (StructData)Activator.CreateInstance(t);
            obj.Parse(data);
            return obj;
        }
    }
}