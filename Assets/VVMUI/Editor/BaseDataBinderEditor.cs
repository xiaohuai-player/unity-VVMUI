using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using VVMUI.Core;
using VVMUI.Core.Binder;
using VVMUI.Core.Command;
using VVMUI.Core.Data;

[CustomEditor (typeof (BaseDataBinder))]
[CanEditMultipleObjects]
public class BaseDataBinderEditor : Editor {
    private List<bool> itemsExpand = new List<bool> ();
    public override void OnInspectorGUI () {
        BaseDataBinder binder = target as BaseDataBinder;
        VMBehaviour vm = binder.GetComponentInParent<VMBehaviour> ();
        Dictionary<string, Type> datas = new Dictionary<string, Type> ();
        Dictionary<Type, List<string>> typeDatas = new Dictionary<Type, List<string>> ();
        if (vm != null) {
            Type type = vm.GetType ();
            FieldInfo[] fields = type.GetFields ();
            for (int i = 0; i < fields.Length; i++) {
                FieldInfo fi = fields[i];
                Type t = fi.FieldType;
                if (t.GetInterface ("IData") != null || t.BaseType == typeof (StructData)) {
                    datas[fi.Name] = t.BaseType;
                }
                if (t.GetInterface ("IData") != null) {
                    Type baseType = t.BaseType;
                    if (baseType.Name.Contains ("BaseData`")) {
                        Type[] gTypes = baseType.GetGenericArguments ();
                        if (gTypes.Length > 0) {
                            if (!typeDatas.ContainsKey (gTypes[0])) {
                                typeDatas[gTypes[0]] = new List<string> ();
                            }
                            typeDatas[gTypes[0]].Add (fi.Name);
                        }
                    }
                }
            }
        }

        List<UIBehaviour> components = new List<UIBehaviour> ();
        binder.GetComponents<UIBehaviour> (components);

        List<string> componentsStr = new List<string> ();
        for (int i = 0; i < components.Count; i++) {
            componentsStr.Add (components[i].GetType ().Name);
        }

        if (itemsExpand.Count == 0) {
            for (int i = 0; i < binder.BindItems.Count; i++) {
                itemsExpand.Add (false);
            }
        }

        for (int i = 0; i < binder.BindItems.Count; i++) {
            BaseDataBinder.DataBinderItem item = binder.BindItems[i];

            string itemLabel = "Item " + i;
            switch (item.Type) {
                case BaseDataBinder.DataBinderItem.BindType.Property:
                    if (item.Component != null && !string.IsNullOrEmpty (item.Property)) {
                        itemLabel = item.Component.GetType ().Name + ":" + item.Property;
                    }
                    break;
                case BaseDataBinder.DataBinderItem.BindType.Active:
                    itemLabel = "Active";
                    break;
                case BaseDataBinder.DataBinderItem.BindType.Animation:
                    itemLabel = "Animation";
                    break;
                case BaseDataBinder.DataBinderItem.BindType.Animator:
                    itemLabel = "Animator:" + item.AnimatorLayer;
                    break;
            }

            EditorGUILayout.Space ();

            GUIStyle style = new GUIStyle (EditorStyles.foldout);
            style.fontStyle = FontStyle.Bold;
            itemsExpand[i] = EditorGUILayout.Foldout (itemsExpand[i], itemLabel, true, style);

            if (itemsExpand[i]) {
                EditorGUILayout.BeginHorizontal ();
                EditorGUILayout.LabelField ("Type:");
                item.Type = (BaseDataBinder.DataBinderItem.BindType) EditorGUILayout.EnumPopup (item.Type);
                EditorGUILayout.EndHorizontal ();

                Type dataType = null;

                switch (item.Type) {
                    case BaseDataBinder.DataBinderItem.BindType.Property:
                        EditorGUILayout.BeginHorizontal ();
                        EditorGUILayout.LabelField ("Component:");
                        int componentIndex = components.IndexOf ((UIBehaviour) item.Component);
                        componentIndex = EditorGUILayout.Popup (componentIndex, componentsStr.ToArray ());
                        if (componentsStr.Count > 0 && componentIndex >= 0 && componentIndex < componentsStr.Count) {
                            item.Component = components[componentIndex];
                        }
                        EditorGUILayout.EndHorizontal ();

                        List<string> propertiesStr = new List<string> ();
                        List<Type> propertiesType = new List<Type> ();
                        if (item.Component != null) {
                            Type componentType = item.Component.GetType ();
                            PropertyInfo[] properties = componentType.GetProperties ();
                            for (int j = 0; j < properties.Length; j++) {
                                if (properties[j].GetSetMethod () != null) {
                                    propertiesStr.Add (properties[j].Name);
                                    propertiesType.Add (properties[j].PropertyType);
                                }
                            }
                        }
                        EditorGUILayout.BeginHorizontal ();
                        EditorGUILayout.LabelField ("Property:");
                        int propertyIndex = propertiesStr.IndexOf (item.Property);
                        propertyIndex = EditorGUILayout.Popup (propertyIndex, propertiesStr.ToArray ());
                        if (propertiesStr.Count > 0 && propertyIndex >= 0 && propertyIndex < propertiesStr.Count) {
                            item.Property = propertiesStr[propertyIndex];
                            dataType = propertiesType[propertyIndex];
                        }
                        EditorGUILayout.EndHorizontal ();

                        break;
                    case BaseDataBinder.DataBinderItem.BindType.Active:
                        dataType = typeof (Boolean);
                        break;
                    case BaseDataBinder.DataBinderItem.BindType.Animation:
                        dataType = typeof (String);
                        break;
                    case BaseDataBinder.DataBinderItem.BindType.Animator:
                        dataType = typeof (String);
                        EditorGUILayout.BeginHorizontal ();
                        EditorGUILayout.LabelField ("AnimatorLayer:");
                        item.AnimatorLayer = EditorGUILayout.IntField (item.AnimatorLayer);
                        EditorGUILayout.EndHorizontal ();
                        break;
                }

                EditorGUILayout.LabelField ("Definder:");
                EditorGUI.indentLevel = 1;

                EditorGUILayout.BeginHorizontal ();
                EditorGUILayout.LabelField ("Key:");
                List<string> datasStr = new List<string> ();
                if (dataType != null && typeDatas.ContainsKey (dataType)) {
                    datasStr.AddRange (typeDatas[dataType].ToArray ());
                }
                int dataIndex = datasStr.IndexOf (item.Definer.Key);
                dataIndex = EditorGUILayout.Popup (dataIndex, datasStr.ToArray (), GUILayout.MinWidth (20), GUILayout.MaxWidth (150));
                if (datasStr.Count > 0 && dataIndex >= 0 && dataIndex < datasStr.Count) {
                    item.Definer.Key = datasStr[dataIndex];
                }
                item.Definer.Key = EditorGUILayout.DelayedTextField (item.Definer.Key, GUILayout.MaxWidth (150));
                EditorGUILayout.EndHorizontal ();

                EditorGUILayout.BeginHorizontal ();
                EditorGUILayout.LabelField ("Converter:");
                item.Definer.Converter = EditorGUILayout.TextField (item.Definer.Converter);
                EditorGUILayout.EndHorizontal ();

                EditorGUILayout.BeginHorizontal ();
                EditorGUILayout.LabelField ("ConverterParameter:");
                item.Definer.ConverterParameter = EditorGUILayout.TextField (item.Definer.ConverterParameter);
                EditorGUILayout.EndHorizontal ();

                EditorGUI.indentLevel = 0;

                if (GUILayout.Button ("Del")) {
                    binder.BindItems.RemoveAt (i);
                    itemsExpand.RemoveAt (i);
                }
            }
        }

        EditorGUILayout.Space ();
        EditorGUILayout.Space ();
        if (GUILayout.Button ("Add")) {
            binder.BindItems.Add (new BaseDataBinder.DataBinderItem ());
            itemsExpand.Add (true);
        }
    }
}