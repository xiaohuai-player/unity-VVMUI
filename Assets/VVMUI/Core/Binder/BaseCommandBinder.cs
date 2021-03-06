using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;
using VVMUI.Core.Command;

namespace VVMUI.Core.Binder
{
    public class BaseCommandBinder : AbstractCommandBinder
    {
        [Serializable]
        public class CommandBinderItem
        {
            public string Event;
            public string Command;
            public string Parameter;

            private ICommand command;
            private Type sourceEventType;
            private object sourceEventObj;
            private object executeDelegate;
            private Action canExecuteHandler;

            public void DoBind(VMBehaviour vm, object parameter, GameObject obj, Component component)
            {
                if (component == null)
                {
                    Debugger.LogError("CommandBinder", obj.name + " component null.");
                    return;
                }
                if (string.IsNullOrEmpty(this.Command))
                {
                    Debugger.LogError("CommandBinder", obj.name + " command key empty.");
                    return;
                }
                if (string.IsNullOrEmpty(this.Event))
                {
                    Debugger.LogError("CommandBinder", obj.name + " event key empty.");
                    return;
                }

                command = vm.GetCommand(this.Command);
                if (command == null)
                {
                    Debugger.LogError("CommandBinder", obj.name + " command null.");
                    return;
                }

                Type commandType = command.GetType().BaseType;
                if (!commandType.FullName.StartsWith("VVMUI.Core.Command.BaseCommand"))
                {
                    Debugger.LogError("CommandBinder", obj.name + " command type error.");
                    return;
                }

                Type componentType = component.GetType();

                // UGUI 组件的 event 有的是 property，有的不是，所以 field 和 property 都判断
                FieldInfo sourceEventFieldInfo = componentType.GetField(this.Event);
                PropertyInfo sourceEventPropertyInfo = componentType.GetProperty(this.Event);
                if (sourceEventFieldInfo != null)
                {
                    sourceEventType = sourceEventFieldInfo.FieldType.BaseType;
                    sourceEventObj = sourceEventFieldInfo.GetValue(component);
                }
                if (sourceEventPropertyInfo != null)
                {
                    sourceEventType = sourceEventPropertyInfo.PropertyType.BaseType;
                    sourceEventObj = sourceEventPropertyInfo.GetValue(component, null);
                }
                if (sourceEventType == null)
                {
                    Debugger.LogError("CommandBinder", obj.name + " event null.");
                    return;
                }
                if (!sourceEventType.FullName.StartsWith("UnityEngine.Events.UnityEvent"))
                {
                    Debugger.LogError("CommandBinder", obj.name + " event type error.");
                    return;
                }

                // 判断 event 和 command 参数类型是否匹配
                bool genericTypeExplicit = true;
                if (sourceEventType.IsGenericType != commandType.IsGenericType)
                {
                    genericTypeExplicit = false;
                }
                Type[] sourceEventGenericTypes = sourceEventType.GetGenericArguments();
                Type[] commandGenericTypes = commandType.GetGenericArguments();
                if (sourceEventGenericTypes.Length != commandGenericTypes.Length)
                {
                    genericTypeExplicit = false;
                }
                for (int j = 0; j < sourceEventGenericTypes.Length; j++)
                {
                    if (sourceEventGenericTypes[j] != commandGenericTypes[j])
                    {
                        genericTypeExplicit = false;
                    }
                }
                if (!genericTypeExplicit)
                {
                    Debugger.LogError("CommandBinder", obj.name + " event type and command type not explicit.");
                    return;
                }

                command.BindVM(vm);

                executeDelegate = command.GetExecuteDelegate(parameter);
                // TODO 性能优化：Type.GetMethod, MethodInfo.Invoke
                sourceEventType.GetMethod("AddListener").Invoke(sourceEventObj, new object[] { executeDelegate });

                Selectable selectableComponent = component as Selectable;
                if (selectableComponent != null)
                {
                    canExecuteHandler = new Action(delegate
                    {
                        selectableComponent.interactable = command.CanExecute(parameter);
                    });
                    command.AddCanExecuteChangedListener(canExecuteHandler);
                    selectableComponent.interactable = command.CanExecute(parameter);
                }
            }

            public void DoUnBind()
            {
                if (command != null && sourceEventType != null && sourceEventObj != null && executeDelegate != null && canExecuteHandler != null)
                {
                    // TODO 性能优化：Type.GetMethod, MethodInfo.Invoke
                    sourceEventType.GetMethod("RemoveListener").Invoke(sourceEventObj, new object[] { executeDelegate });
                    command.RemoveCanExecuteChangedListener(canExecuteHandler);
                }
            }
        }

        public List<CommandBinderItem> BindItems = new List<CommandBinderItem>();

        private Component component;
        public Component GetBindComponent()
        {
            if (component == null)
            {
                component = this.GetComponent<Selectable>();

                // 对 ScrollRect 做特殊处理，绑定 OnValueChanged
                if (component == null)
                {
                    component = this.GetComponent<ScrollRect>();
                }
            }
            return component;
        }

        public override void Bind(VMBehaviour vm)
        {
            for (int i = 0; i < BindItems.Count; i++)
            {
                CommandBinderItem item = BindItems[i];
                item.DoBind(vm, item.Parameter, this.gameObject, GetBindComponent());
            }
        }

        public override void BindListItem(VMBehaviour vm, int index)
        {
            for (int i = 0; i < BindItems.Count; i++)
            {
                CommandBinderItem item = BindItems[i];
                item.DoBind(vm, index, this.gameObject, GetBindComponent());
            }
        }

        public override void UnBind()
        {
            for (int i = 0; i < BindItems.Count; i++)
            {
                CommandBinderItem item = BindItems[i];
                item.DoUnBind();
            }
        }
    }
}