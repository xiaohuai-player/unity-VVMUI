using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace VVMUI.Core.Data
{
    // 没有实际用处，仅用来做类型匹配检查
    public sealed class StructType
    { 
    }

    public class StructData : IData
    {
        protected Dictionary<string, IData> allData = new Dictionary<string, IData>();

        protected bool fieldsInit = false;

        protected void InitFields()
        {
            if (fieldsInit)
            {
                return;
            }

            Type type = this.GetType();
            FieldInfo[] fields = type.GetFields();
            for (int i = 0; i < fields.Length; i++)
            {
                FieldInfo fi = fields[i];
                if (typeof(IData).IsAssignableFrom(fi.FieldType))
                {
                    IData data = fields[i].GetValue(this) as IData;
                    if (data != null)
                    {
                        allData[fields[i].Name] = data;
                        data.AddValueChangedListener(delegate (IData source)
                        {
                            this.InvokeValueChanged();
                        });
                    }
                }
            }

            fieldsInit = true;
        }

        public IData this[string key]
        {
            get
            {
                InitFields();

                if (string.IsNullOrEmpty(key))
                {
                    return null;
                }

                IData v = null;
                allData.TryGetValue(key, out v);
                return v;
            }
        }

        public Dictionary<string, IData> Fields
        {
            get
            {
                InitFields();
                return allData;
            }
        }

        public void AddField(string key, IData data)
        {
            if (!string.IsNullOrEmpty(key) && data != null && !this.allData.ContainsKey(key))
            {
                allData[key] = data;
                data.AddValueChangedListener(delegate (IData source)
                {
                    this.InvokeValueChanged();
                });
            }
        }

        private List<DataChangedHandler> valueChangedHandlers = new List<DataChangedHandler>();
        public void InvokeValueChanged()
        {
            for (int i = 0; i < valueChangedHandlers.Count; i++)
            {
                valueChangedHandlers[i].Invoke(this);
            }
        }
        public void AddValueChangedListener(DataChangedHandler handler)
        {
            valueChangedHandlers.Add(handler);
        }
        public void RemoveValueChangedListener(DataChangedHandler handler)
        {
            valueChangedHandlers.Remove(handler);
        }

        public Type GetBindDataType()
        {
            return typeof(StructType);
        }

        public DataType GetDataType()
        {
            return DataType.Struct;
        }

        public void CopyFrom(IData data)
        {
            if (!typeof(StructData).IsAssignableFrom(data.GetType()))
            {
                Debugger.LogError("BaseData", "can not copy data with not the same type");
                return;
            }

            StructData strct = (StructData)data;
            foreach (KeyValuePair<string, IData> kv in strct.Fields)
            {
                if (this[kv.Key] != null)
                {
                    this[kv.Key].CopyFrom(kv.Value);
                }
            }
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
                if (objfi == null || !typeof(IData).IsAssignableFrom(objfi.FieldType))
                {
                    continue;
                }

                bool isList = objfi.FieldType.IsGenericType && objfi.FieldType.GetGenericTypeDefinition() == typeof(ListData<>);
                bool isDict = objfi.FieldType.IsGenericType && objfi.FieldType.GetGenericTypeDefinition() == typeof(DictionaryData<>);
                bool isStruct = typeof(StructData).IsAssignableFrom(objfi.FieldType);
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
                        (objv as IBaseData).FastSetValue(parseStruct.Value);
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