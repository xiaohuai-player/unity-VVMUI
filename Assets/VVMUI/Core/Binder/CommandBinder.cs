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
                Type eventType = null;
                object eventObj = null;
                FieldInfo eventFieldInfo = componentType.GetField (item.Event);
                PropertyInfo eventPropertyInfo = componentType.GetProperty (item.Event);
                if (eventFieldInfo != null) {
                    eventType = eventFieldInfo.FieldType.BaseType;
                    eventObj = eventFieldInfo.GetValue (item.Component);
                }
                if (eventPropertyInfo != null) {
                    eventType = eventPropertyInfo.PropertyType.BaseType;
                    eventObj = eventPropertyInfo.GetValue (item.Component, null);
                }
                if (eventType == null) {
                    Debugger.LogError ("CommandBinder", this.name + " event null.");
                    continue;
                }
                if (!eventType.FullName.StartsWith ("UnityEngine.Events.UnityEvent")) {
                    Debugger.LogError ("CommandBinder", this.name + " event type error.");
                    continue;
                }

                bool genericTypeExplicit = true;
                if (eventType.IsGenericType != commandType.IsGenericType) {
                    genericTypeExplicit = false;
                }
                Type[] eventGenericTypes = eventType.GetGenericArguments ();
                Type[] commandGenericTypes = commandType.GetGenericArguments ();
                if (eventGenericTypes.Length != commandGenericTypes.Length) {
                    genericTypeExplicit = false;
                }
                for (int j = 0; j < eventGenericTypes.Length; j++) {
                    if (eventGenericTypes[j] != commandGenericTypes[j]) {
                        genericTypeExplicit = false;
                    }
                }
                if (!genericTypeExplicit) {
                    Debugger.LogError ("CommandBinder", this.name + " event type and command type not explicit.");
                    continue;
                }

                Type executeDelegateType = command.GetExecuteDelegateType ();
                MethodInfo executeMethod = null;
                if (eventType.IsGenericType) {
                    executeMethod = commandType.GetMethod ("GenericExecute");
                } else {
                    executeMethod = commandType.GetMethod ("Execute");
                }
                Delegate executeDelegate = Delegate.CreateDelegate (executeDelegateType, command, executeMethod);
                eventType.GetMethod ("AddListener").Invoke (eventObj, new object[] { executeDelegate });

                command.CanExecuteChanged += new Action<bool> (delegate (bool f) {
                    item.Component.interactable = f;
                });
                command.RefreshCanExecute ();
                item.Component.interactable = command.CanExecute ();
            }
        }
    }
}