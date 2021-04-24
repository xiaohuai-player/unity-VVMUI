using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using VVMUI.Core;
using VVMUI.Core.Data;
using VVMUI.Core.Binder;

namespace VVMUI.Inspector
{
    public class DataDefinerDrawer
    {
        private DataDefiner definer;

        private int fieldIndex;
        private int dataIndex;
        private string hashKey;
        private int converterIndex;

        public DataDefinerDrawer(DataDefiner definer)
        {
            this.definer = definer;
        }

        public void Draw(VMBehaviour vm, IData sourceData, Type propertyType)
        {
            if (vm == null || definer == null || propertyType == null)
            {
                return;
            }

            definer.ParseKey(true);

            // 获取当前链下的可选数据
            IData currentChainData = sourceData != null ? definer.GetData(sourceData) : definer.GetData(vm);
            Dictionary<DataType, Dictionary<string, IData>> datas = new Dictionary<DataType, Dictionary<string, IData>>();

            if (definer.KeyChain.Count <= 0)
            {
                if (sourceData != null && sourceData.GetDataType() == DataType.Struct)
                {
                    StructData strct = sourceData as StructData;
                    foreach (string k in strct.Fields.Keys)
                    {
                        IData d = strct.Fields[k];
                        if (!datas.TryGetValue(d.GetDataType(), out Dictionary<string, IData> dict))
                        {
                            dict = new Dictionary<string, IData>();
                            datas[d.GetDataType()] = dict;
                        }
                        dict[k] = d;
                    }
                }
                else if (sourceData == null)
                {
                    foreach (string k in vm.GetDataKeys())
                    {
                        IData d = vm.GetData(k);
                        if (!datas.TryGetValue(d.GetDataType(), out Dictionary<string, IData> dict))
                        {
                            dict = new Dictionary<string, IData>();
                            datas[d.GetDataType()] = dict;
                        }
                        dict[k] = d;
                    }
                }
            }

            if (currentChainData != null && currentChainData.GetDataType() == DataType.Struct)
            {
                StructData strct = currentChainData as StructData;
                foreach (string k in strct.Fields.Keys)
                {
                    IData d = strct.Fields[k];
                    if (!datas.TryGetValue(d.GetDataType(), out Dictionary<string, IData> dict))
                    {
                        dict = new Dictionary<string, IData>();
                        datas[d.GetDataType()] = dict;
                    }
                    dict[k] = d;
                }
            }

            List<string> convertersStr = new List<string>();
            convertersStr.Add("");
            convertersStr.AddRange(vm.GetConverterKeys());
            converterIndex = convertersStr.IndexOf(definer.Converter);
            if (converterIndex < 0) converterIndex = 0;

            bool typeFit = currentChainData != null && currentChainData.GetBindDataType().IsAssignableFrom(propertyType);
            if (!string.IsNullOrEmpty(definer.Converter)) typeFit = true;

            EditorGUILayout.LabelField("Definder:");
            EditorGUI.indentLevel++;

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Key: " + (string.IsNullOrEmpty(definer.Key) ? "NULL" : definer.Key), EditorStyles.boldLabel);
            if (typeFit)
            {
                GUIStyle style = new GUIStyle(EditorStyles.boldLabel);
                style.normal.textColor = Color.green;
                EditorGUILayout.LabelField("√", style);
            }
            else
            {
                GUIStyle style = new GUIStyle(EditorStyles.boldLabel);
                style.normal.textColor = Color.red;
                EditorGUILayout.LabelField("x", style);
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Select:");

            bool add = false;
            DefinerKey addkey = new DefinerKey();

            if (datas.Count > 0)
            {
                List<string> fields = new List<string>();
                foreach (var catData in datas)
                {
                    if (catData.Key == DataType.Base)
                    {
                        foreach (var kv in catData.Value)
                        {
                            if (kv.Value.GetBindDataType().IsAssignableFrom(propertyType) || !string.IsNullOrEmpty(definer.Converter))
                            {
                                fields.Add(kv.Key);
                            }
                        }
                    }
                    else
                    {
                        fields.AddRange(catData.Value.Keys);
                    }
                }
                fields.Sort(delegate (string e1, string e2)
                {
                    return e1.CompareTo(e2);
                });
                if (fields.Count > 0)
                {
                    fieldIndex = EditorGUILayout.Popup(fieldIndex, fields.ToArray());
                    fieldIndex = Mathf.Clamp(fieldIndex, 0, fields.Count - 1);
                    add = true;
                    addkey = new DefinerKey(fields[fieldIndex]);
                }
            }
            else if (currentChainData != null)
            {
                if (currentChainData.GetDataType() == DataType.Dictionary)
                {
                    hashKey = EditorGUILayout.TextField(hashKey);
                    if (!string.IsNullOrEmpty(hashKey))
                    {
                        add = true;
                        addkey = new DefinerKey(null, -1, hashKey);
                    }
                }
                else if (currentChainData.GetDataType() == DataType.List)
                {
                    dataIndex = Mathf.Max(0, EditorGUILayout.IntField(dataIndex));
                    add = true;
                    addkey = new DefinerKey(null, dataIndex, null);
                }
            }

            if (definer.KeyChain.Count > 0)
            {
                if (GUILayout.Button("Prev"))
                {
                    definer.RemoveKeyChain();
                    definer.SerializeKey();
                }
            }

            if (add)
            {
                if (GUILayout.Button("Next"))
                {
                    definer.AddKeyChain(addkey);
                    definer.SerializeKey();
                }
            }

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Input:");
            definer.Key = EditorGUILayout.DelayedTextField(definer.Key);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Converter:");
            converterIndex = EditorGUILayout.Popup(converterIndex, convertersStr.ToArray());
            definer.Converter = convertersStr[converterIndex];
            if (!string.IsNullOrEmpty(definer.Converter))
            {
                definer.ConverterParameter = EditorGUILayout.DelayedTextField(definer.ConverterParameter);
                if (GUILayout.Button("Clear"))
                {
                    definer.Converter = "";
                }
            }
            EditorGUILayout.EndHorizontal();

            EditorGUI.indentLevel--;
        }
    }
}