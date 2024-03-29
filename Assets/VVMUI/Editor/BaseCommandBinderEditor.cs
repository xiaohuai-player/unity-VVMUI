using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using VVMUI.Core;
using VVMUI.Core.Command;
using VVMUI.Core.Binder;

namespace VVMUI.Inspector
{
    [CustomEditor(typeof(BaseCommandBinder))]
    [CanEditMultipleObjects]
    public class BaseCommandBinderEditor : Editor
    {
        private Dictionary<string, bool> componentEventsExpand = new Dictionary<string, bool>();
        public override void OnInspectorGUI()
        {
            BaseCommandBinder binder = target as BaseCommandBinder;
            binder.EditorBind();

            Component component = binder.GetBindComponent();
            if (component == null)
            {
                GUILayout.Label("No bind component.", EditorStyles.boldLabel);
                return;
            }

            Dictionary<string, Type> commands = new Dictionary<string, Type>();
            foreach (string k in binder.BindVM.GetCommandKeys())
            {
                commands[k] = binder.BindVM.GetCommand(k).GetType().BaseType;
            }

            Type type = binder.BindVM.GetType();
            FieldInfo[] fields = type.GetFields();
            for (int i = 0; i < fields.Length; i++)
            {
                FieldInfo fi = fields[i];
                Type t = fi.FieldType.BaseType;
                if (typeof(ICommand).IsAssignableFrom(t))
                {
                    commands[fi.Name] = t;
                }
            }

            Type componentType = component.GetType();
            FieldInfo[] componentFieldInfos = componentType.GetFields();
            PropertyInfo[] componentPropertyInfos = componentType.GetProperties();
            Dictionary<string, bool> componentEvents = new Dictionary<string, bool>();
            Dictionary<string, string[]> componentEventParamTypes = new Dictionary<string, string[]>();
            Dictionary<string, string[]> componentEventCommands = new Dictionary<string, string[]>();
            Action<string, Type> scanType = delegate (string name, Type sourceEventType)
            {
                if (typeof(UnityEngine.Events.UnityEventBase).IsAssignableFrom(sourceEventType))
                {
                    componentEvents[name] = false;

                    Type[] eventParamTypes = sourceEventType.GetGenericArguments();
                    List<string> eventParamTypesStr = new List<string>();
                    foreach (Type t in eventParamTypes)
                    {
                        eventParamTypesStr.Add(t.Name);
                    }
                    componentEventParamTypes[name] = eventParamTypesStr.ToArray();

                    List<string> explicitCommands = new List<string>();
                    foreach (KeyValuePair<string, Type> command in commands)
                    {
                        // 判断 event 和 command 参数类型是否匹配
                        Type commandType = command.Value;
                        bool genericTypeExplicit = true;
                        if (sourceEventType.IsGenericType != commandType.IsGenericType)
                        {
                            genericTypeExplicit = false;
                            continue;
                        }
                        Type[] sourceEventGenericTypes = sourceEventType.GetGenericArguments();
                        Type[] commandGenericTypes = commandType.GetGenericArguments();
                        if (sourceEventGenericTypes.Length != commandGenericTypes.Length)
                        {
                            genericTypeExplicit = false;
                            continue;
                        }
                        for (int j = 0; j < sourceEventGenericTypes.Length; j++)
                        {
                            if (sourceEventGenericTypes[j] != commandGenericTypes[j])
                            {
                                genericTypeExplicit = false;
                                break;
                            }
                        }
                        if (genericTypeExplicit)
                        {
                            explicitCommands.Add(command.Key);
                        }
                    }
                    componentEventCommands[name] = explicitCommands.ToArray();

                    if (!componentEventsExpand.ContainsKey(name))
                    {
                        componentEventsExpand[name] = true;
                    }
                }
            };
            for (int i = 0; i < componentFieldInfos.Length; i++)
            {
                FieldInfo fi = componentFieldInfos[i];
                scanType(fi.Name, fi.FieldType.BaseType);
            }
            for (int i = 0; i < componentPropertyInfos.Length; i++)
            {
                PropertyInfo pi = componentPropertyInfos[i];
                scanType(pi.Name, pi.PropertyType.BaseType);
            }

            List<BaseCommandBinder.CommandBinderItem> binderItems = new List<BaseCommandBinder.CommandBinderItem>();
            for (int i = 0; i < binder.BindItems.Count; i++)
            {
                if (componentEvents.ContainsKey(binder.BindItems[i].Event))
                {
                    binderItems.Add(binder.BindItems[i]);
                    componentEvents[binder.BindItems[i].Event] = true;
                }
            }
            foreach (KeyValuePair<string, bool> kv in componentEvents)
            {
                if (!kv.Value)
                {
                    BaseCommandBinder.CommandBinderItem item = new BaseCommandBinder.CommandBinderItem();
                    item.Event = kv.Key;
                    binderItems.Add(item);
                }
            }

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Component:");
            EditorGUILayout.LabelField(componentType.ToString().Replace("UnityEngine.UI.", ""), EditorStyles.boldLabel);

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Events:");
            for (int i = 0; i < binderItems.Count; i++)
            {
                BaseCommandBinder.CommandBinderItem item = binderItems[i];

                string eventLabel = item.Event;
                if (componentEventParamTypes[item.Event].Length > 0)
                {
                    eventLabel += " (" + string.Join(",", componentEventParamTypes[item.Event]) + ")";
                }

                if (!string.IsNullOrEmpty(item.Command))
                {
                    eventLabel += " : " + item.Command;
                }

                GUIStyle style = new GUIStyle(EditorStyles.foldout);
                style.fontStyle = FontStyle.Bold;
                componentEventsExpand[item.Event] = EditorGUILayout.Foldout(componentEventsExpand[item.Event], eventLabel, true, style);

                if (componentEventsExpand[item.Event])
                {
                    EditorGUILayout.BeginHorizontal();

                    EditorGUILayout.LabelField("Command:", GUILayout.Width(65));
                    List<string> commandsStr = new List<string>(componentEventCommands[item.Event]);
                    int commandIndex = commandsStr.IndexOf(item.Command);
                    commandIndex = EditorGUILayout.Popup(commandIndex, commandsStr.ToArray(), GUILayout.MinWidth(20), GUILayout.MaxWidth(100));
                    if (commandsStr.Count > 0 && commandIndex >= 0 && commandIndex < commandsStr.Count)
                    {
                        item.Command = commandsStr[commandIndex];
                    }
                    item.Command = EditorGUILayout.DelayedTextField(item.Command, GUILayout.Width(100));

                    if (binder.GetComponentInParent<ListTemplateBinder>(true) == null)
                    {
                        GUILayout.Space(30);
                        EditorGUILayout.LabelField("Parameter:", GUILayout.Width(60));
                        item.Parameter = EditorGUILayout.TextField(item.Parameter, GUILayout.Width(100));
                    }

                    EditorGUILayout.EndHorizontal();
                }
                EditorGUILayout.Space();
            }

            binder.BindItems = binderItems;

            // base.DrawDefaultInspector ();
        }
    }
}