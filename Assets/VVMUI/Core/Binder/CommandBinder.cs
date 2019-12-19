using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;
using VVMUI.Core.Command;

namespace VVMUI.Core.Binder {
    [ExecuteInEditMode]
    public class CommandBinder : BaseCommandBinder {
        [Serializable]
        public class CommandBinderItem {
            public Selectable Component;
            public string Event;
            public string Command;
        }

        public List<CommandBinderItem> BindItems = new List<CommandBinderItem> ();

        public override void Bind (VMBehaviour vm) {
            for (int i = 0; i < BindItems.Count; i++) {
                CommandBinderItem item = BindItems[i];
                if (item.Component == null) {
                    Debugger.LogError ("CommandBinder", this.name + " component null.");
                    continue;
                }
                if (string.IsNullOrEmpty (item.Command)) {
                    Debugger.LogError ("CommandBinder", this.name + " command key empty.");
                    continue;
                }
                if (string.IsNullOrEmpty (item.Event)) {
                    Debugger.LogError ("CommandBinder", this.name + " event key empty.");
                    continue;
                }

                ICommand command = vm.GetCommand (item.Command);
                if (command == null) {
                    Debugger.LogError ("CommandBinder", this.name + " command null.");
                    continue;
                }

                Type commandType = command.GetType ().BaseType;
                if (!commandType.FullName.StartsWith ("VVMUI.Core.Command.BaseCommand")) {
                    Debugger.LogError ("CommandBinder", this.name + " command type error.");
                    continue;
                }

                Type componentType = item.Component.GetType ();
                Type sourceEventType = null;
                object sourceEventObj = null;

                // UGUI 组件的 event 有的是 property，有的不是，所以 field 和 property 都判断
                FieldInfo sourceEventFieldInfo = componentType.GetField (item.Event);
                PropertyInfo sourceEventPropertyInfo = componentType.GetProperty (item.Event);
                if (sourceEventFieldInfo != null) {
                    sourceEventType = sourceEventFieldInfo.FieldType.BaseType;
                    sourceEventObj = sourceEventFieldInfo.GetValue (item.Component);
                }
                if (sourceEventPropertyInfo != null) {
                    sourceEventType = sourceEventPropertyInfo.PropertyType.BaseType;
                    sourceEventObj = sourceEventPropertyInfo.GetValue (item.Component, null);
                }
                if (sourceEventType == null) {
                    Debugger.LogError ("CommandBinder", this.name + " event null.");
                    continue;
                }
                if (!sourceEventType.FullName.StartsWith ("UnityEngine.Events.UnityEvent")) {
                    Debugger.LogError ("CommandBinder", this.name + " event type error.");
                    continue;
                }

                bool genericTypeExplicit = true;
                if (sourceEventType.IsGenericType != commandType.IsGenericType) {
                    genericTypeExplicit = false;
                }
                Type[] sourceEventGenericTypes = sourceEventType.GetGenericArguments ();
                Type[] commandGenericTypes = commandType.GetGenericArguments ();
                if (sourceEventGenericTypes.Length != commandGenericTypes.Length) {
                    genericTypeExplicit = false;
                }
                for (int j = 0; j < sourceEventGenericTypes.Length; j++) {
                    if (sourceEventGenericTypes[j] != commandGenericTypes[j]) {
                        genericTypeExplicit = false;
                    }
                }
                if (!genericTypeExplicit) {
                    Debugger.LogError ("CommandBinder", this.name + " event type and command type not explicit.");
                    continue;
                }

                command.SetParameter (null);

                Type executeDelegateType = command.GetEventDelegateType ();
                MethodInfo executeMethod = null;
                if (sourceEventType.IsGenericType) {
                    executeMethod = commandType.GetMethod ("GenericExecute");
                } else {
                    executeMethod = commandType.GetMethod ("Execute");
                }
                Delegate executeDelegate = Delegate.CreateDelegate (executeDelegateType, command, executeMethod);
                sourceEventType.GetMethod ("AddListener").Invoke (sourceEventObj, new object[] { executeDelegate });

                command.CanExecuteChanged += new Action<bool> (delegate (bool f) {
                    item.Component.interactable = f;
                });
                command.RefreshCanExecute ();
                item.Component.interactable = command.CanExecute ();
            }
        }
    }
}