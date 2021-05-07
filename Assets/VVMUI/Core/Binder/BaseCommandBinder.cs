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
            private Type commandType;
            private ReflectionCacheData commandReflection;

            private Type componentType;
            private ReflectionCacheData componentReflection;

            private Type sourceEventType;
            private ReflectionCacheData sourceEventReflection;
            private object sourceEventObj;
            private object sourceEventAction;

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

                // component type reflection
                componentType = component.GetType();
                componentReflection = ReflectionCache.Singleton[componentType];

                // command type reflection
                command = vm.GetCommand(this.Command);
                if (command == null)
                {
                    Debugger.LogError("CommandBinder", obj.name + " command null.");
                    return;
                }
                commandType = command.GetType().BaseType;
                if (!typeof(BaseCommand).IsAssignableFrom(commandType))
                {
                    Debugger.LogError("CommandBinder", obj.name + " command type error.");
                    return;
                }
                commandReflection = ReflectionCache.Singleton[commandType];

                // source event type reflection
                // TODO: 性能优化 GetValue
                // UGUI 组件的 event 有的是 property，有的不是，所以 field 和 property 都判断
                FieldInfo sourceEventFieldInfo = componentReflection.GetField(this.Event);
                PropertyInfo sourceEventPropertyInfo = componentReflection.GetProperty(this.Event);
                if (sourceEventFieldInfo != null)
                {
                    sourceEventType = sourceEventFieldInfo.FieldType.BaseType;
                    sourceEventObj = sourceEventFieldInfo.GetValue(component);
                }
                if (sourceEventPropertyInfo != null)
                {
                    sourceEventType = sourceEventPropertyInfo.PropertyType.BaseType;
                    sourceEventObj = sourceEventPropertyInfo.GetValue(component);
                }
                if (sourceEventType == null || sourceEventObj == null)
                {
                    Debugger.LogError("CommandBinder", obj.name + " event null.");
                    return;
                }
                if (!typeof(UnityEngine.Events.UnityEventBase).IsAssignableFrom(sourceEventType))
                {
                    Debugger.LogError("CommandBinder", obj.name + " event type error.");
                    return;
                }
                sourceEventReflection = ReflectionCache.Singleton[sourceEventType];

                // 判断 event 和 command 参数类型是否匹配
                bool genericTypeExplicit = true;
                while (true)
                {
                    if (sourceEventType.IsGenericType != commandType.IsGenericType)
                    {
                        genericTypeExplicit = false;
                        break;
                    }

                    Type[] sourceEventGenericTypes = sourceEventReflection.GetGenericArguments();
                    Type[] commandGenericTypes = commandReflection.GetGenericArguments();
                    if (sourceEventGenericTypes.Length != commandGenericTypes.Length)
                    {
                        genericTypeExplicit = false;
                        break;
                    }
                    for (int j = 0; j < sourceEventGenericTypes.Length; j++)
                    {
                        if (sourceEventGenericTypes[j] != commandGenericTypes[j])
                        {
                            genericTypeExplicit = false;
                            break;
                        }
                    }
                    break;
                }
                if (!genericTypeExplicit)
                {
                    Debugger.LogError("CommandBinder", obj.name + " event type and command type not explicit.");
                    return;
                }

                command.BindVM(vm);

                sourceEventAction = command.AddListenerToEvent(sourceEventObj, parameter);

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
                if (command != null && sourceEventType != null && sourceEventObj != null && sourceEventAction != null)
                {
                    command.RemoveListenerFromEvent(sourceEventObj, sourceEventAction);
                    if (canExecuteHandler != null)
                    {
                        command.RemoveCanExecuteChangedListener(canExecuteHandler);
                    }
                }
            }
        }

        public List<CommandBinderItem> BindItems = new List<CommandBinderItem>();

        [SerializeField]
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
            base.Bind(vm);

            for (int i = 0; i < BindItems.Count; i++)
            {
                CommandBinderItem item = BindItems[i];
                item.DoBind(vm, item.Parameter, this.gameObject, GetBindComponent());
            }
        }

        public override void BindListItem(VMBehaviour vm, int index)
        {
            base.BindListItem(vm, index);

            for (int i = 0; i < BindItems.Count; i++)
            {
                CommandBinderItem item = BindItems[i];
                item.DoBind(vm, index, this.gameObject, GetBindComponent());
            }
        }

        public override void UnBind()
        {
            base.UnBind();

            for (int i = 0; i < BindItems.Count; i++)
            {
                CommandBinderItem item = BindItems[i];
                item.DoUnBind();
            }
        }
    }
}