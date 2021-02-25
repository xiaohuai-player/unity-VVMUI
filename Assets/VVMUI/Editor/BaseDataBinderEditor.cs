using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using VVMUI.Core;
using VVMUI.Core.Binder;
using VVMUI.Core.Data;

namespace VVMUI.Inspector
{
    [CustomEditor(typeof(BaseDataBinder))]
    [CanEditMultipleObjects]
    public class BaseDataBinderEditor : Editor
    {
        private Dictionary<DataDefiner, DataDefinerDrawer> definerDrawers = new Dictionary<DataDefiner, DataDefinerDrawer>();
        private List<bool> itemsExpand = new List<bool>();
        public override void OnInspectorGUI()
        {
            BaseDataBinder binder = target as BaseDataBinder;
            binder.EditorBind();

            if (binder.GetComponentInParent<ListTemplateBinder>(true) != null)
            {
                ListTemplateBinder templateBinder = binder.GetComponentInParent<ListTemplateBinder>(true);
                IData data = templateBinder.ItemSource.GetData(binder.BindVM);
                IListData list = data as IListData;
                if (list == null)
                {
                    GUIStyle style = new GUIStyle(EditorStyles.boldLabel);
                    style.normal.textColor = Color.red;
                    EditorGUILayout.LabelField("This is a binder within a list template. The template must have source list data bind.", style);
                    return;
                }
            }

            List<Component> components = new List<Component>();
            List<Component> validComponents = new List<Component>();
            binder.GetComponents<Component>(components);
            for (int i = 0; i < components.Count; i++)
            {
                Component cpt = components[i];
                Type cptType = cpt.GetType();
                if (cptType != typeof(BaseCommandBinder) && cptType != typeof(BaseDataBinder) && cptType != typeof(ListDataBinder) && cptType != typeof(ListTemplateBinder) && cptType != typeof(VMDataBinder))
                {
                    validComponents.Add(cpt);
                }
            }
            components = validComponents;

            List<string> componentsStr = new List<string>();
            for (int i = 0; i < components.Count; i++)
            {
                componentsStr.Add(components[i].GetType().Name);
            }

            if (itemsExpand.Count == 0)
            {
                for (int i = 0; i < binder.BindItems.Count; i++)
                {
                    itemsExpand.Add(false);
                }
            }

            for (int i = 0; i < binder.BindItems.Count; i++)
            {
                BaseDataBinder.DataBinderItem item = binder.BindItems[i];

                string itemLabel = "Item " + i;
                switch (item.Type)
                {
                    case BaseDataBinder.DataBinderItem.BindType.Property:
                        if (item.Component != null && !string.IsNullOrEmpty(item.Property))
                        {
                            itemLabel = item.Component.GetType().Name + "(" + item.Property + ")";
                        }
                        break;
                    case BaseDataBinder.DataBinderItem.BindType.Active:
                        itemLabel = "Active";
                        break;
                    case BaseDataBinder.DataBinderItem.BindType.Animation:
                        itemLabel = "Animation";
                        break;
                    case BaseDataBinder.DataBinderItem.BindType.Animator:
                        itemLabel = "Animator(" + item.AnimatorLayer + ")";
                        break;
                }

                if (!string.IsNullOrEmpty(item.Definer.Key))
                {
                    itemLabel += " : " + item.Definer.Key;
                }

                EditorGUILayout.Space();

                GUIStyle style = new GUIStyle(EditorStyles.foldout);
                style.fontStyle = FontStyle.Bold;
                itemsExpand[i] = EditorGUILayout.Foldout(itemsExpand[i], itemLabel, true, style);

                if (itemsExpand[i])
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Type:");
                    item.Type = (BaseDataBinder.DataBinderItem.BindType)EditorGUILayout.EnumPopup(item.Type);
                    EditorGUILayout.EndHorizontal();

                    Type propertyType = null;

                    switch (item.Type)
                    {
                        case BaseDataBinder.DataBinderItem.BindType.Property:
                            EditorGUILayout.BeginHorizontal();
                            EditorGUILayout.LabelField("Component:");
                            int componentIndex = components.IndexOf(item.Component);
                            componentIndex = EditorGUILayout.Popup(componentIndex, componentsStr.ToArray());
                            if (componentsStr.Count > 0 && componentIndex >= 0 && componentIndex < componentsStr.Count)
                            {
                                item.Component = components[componentIndex];
                            }
                            EditorGUILayout.EndHorizontal();

                            List<string> propertiesStr = new List<string>();
                            List<Type> propertiesType = new List<Type>();
                            if (item.Component != null)
                            {
                                Type componentType = item.Component.GetType();
                                PropertyInfo[] properties = componentType.GetProperties();
                                for (int j = 0; j < properties.Length; j++)
                                {
                                    PropertyInfo property = properties[j];
                                    MethodInfo setMethod = property.GetSetMethod();
                                    if (setMethod != null && setMethod.IsPublic)
                                    {
                                        foreach (Type t in BaseData.SupportDataType)
                                        {
                                            if (property.PropertyType.IsAssignableFrom(t))
                                            {
                                                propertiesStr.Add(property.Name);
                                                propertiesType.Add(property.PropertyType);
                                                break;
                                            }
                                        }
                                    }
                                }
                            }
                            EditorGUILayout.BeginHorizontal();
                            EditorGUILayout.LabelField("Property:");
                            int propertyIndex = propertiesStr.IndexOf(item.Property);
                            propertyIndex = EditorGUILayout.Popup(propertyIndex, propertiesStr.ToArray());
                            if (propertiesStr.Count > 0 && propertyIndex >= 0 && propertyIndex < propertiesStr.Count)
                            {
                                item.Property = propertiesStr[propertyIndex];
                                propertyType = propertiesType[propertyIndex];
                            }
                            EditorGUILayout.EndHorizontal();

                            break;
                        case BaseDataBinder.DataBinderItem.BindType.Active:
                            propertyType = typeof(Boolean);
                            break;
                        case BaseDataBinder.DataBinderItem.BindType.Animation:
                            propertyType = typeof(String);
                            break;
                        case BaseDataBinder.DataBinderItem.BindType.Animator:
                            propertyType = typeof(String);
                            EditorGUILayout.BeginHorizontal();
                            EditorGUILayout.LabelField("AnimatorLayer:");
                            item.AnimatorLayer = EditorGUILayout.IntField(item.AnimatorLayer);
                            EditorGUILayout.EndHorizontal();
                            break;
                    }

                    if (!definerDrawers.ContainsKey(item.Definer))
                    {
                        definerDrawers[item.Definer] = new DataDefinerDrawer(item.Definer);
                    }
                    definerDrawers[item.Definer].Draw(binder.BindVM, binder.BindData, propertyType);

                    if (GUILayout.Button("Del"))
                    {
                        binder.BindItems.RemoveAt(i);
                        itemsExpand.RemoveAt(i);
                    }
                }
            }

            EditorGUILayout.Space();
            EditorGUILayout.Space();
            if (GUILayout.Button("Add"))
            {
                binder.BindItems.Add(new BaseDataBinder.DataBinderItem());
                itemsExpand.Add(true);
            }
        }
    }
}