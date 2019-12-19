using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using VVMUI.Core.Converter;
using VVMUI.Core.Data;

namespace VVMUI.Core.Binder {
    [ExecuteInEditMode]
    public class DataBinder : BaseDataBinder {
        [Serializable]
        public class DataBinderItem {
            public Component Component;
            public string Property;
            public DataDefiner Definer = new DataDefiner ();

            [HideInInspector]
            public IData Source;

            [HideInInspector]
            public IConverter Converter;

            [HideInInspector]
            public Action SetValueHandler;

            public void DoBind (VMBehaviour vm, GameObject obj) {
                if (this.Component == null) {
                    Debugger.LogError ("DataBinder", obj.name + " component null.");
                    return;
                }
                if (string.IsNullOrEmpty (this.Property)) {
                    Debugger.LogError ("DataBinder", obj.name + " property empty.");
                    return;
                }

                Type componentType = this.Component.GetType ();
                PropertyInfo propertyInfo = componentType.GetProperty (this.Property);
                if (propertyInfo == null || propertyInfo.GetSetMethod () == null) {
                    Debugger.LogError ("DataBinder", obj.name + " property null or not support.");
                    return;
                }

                if (this.Source == null) {
                    Debugger.LogError ("DataBinder", obj.name + " data null.");
                    return;
                }

                Type propertyType = propertyInfo.PropertyType;
                Type dataType = this.Source.GetType ();
                Type dataBaseType = dataType.BaseType;

                bool bindData = false;
                if (this.Converter != null) {
                    bindData = true;
                }

                if (dataBaseType.IsGenericType && dataBaseType.GetGenericArguments () [0] == propertyType) {
                    bindData = true;
                }

                if (!bindData) {
                    Debugger.LogError ("DataBinder", obj.name + " data type not explicit or converter is null.");
                    return;
                }

                MethodInfo getMethod = dataType.GetMethod ("Get");
                this.SetValueHandler = delegate () {
                    object value = getMethod.Invoke (this.Source, null);
                    if (this.Converter != null) {
                        value = this.Converter.Convert (value, propertyType, this.Definer.ConverterParameter, vm);
                    }
                    propertyInfo.SetValue (this.Component, value, null);
                };
                this.Source.ValueChanged += this.SetValueHandler;
                this.SetValueHandler.Invoke ();
            }

            public void DoUnBind () {
                if (this.Source != null && this.SetValueHandler != null) {
                    this.Source.ValueChanged -= this.SetValueHandler;
                }
            }
        }

        public List<DataBinderItem> BindItems = new List<DataBinderItem> ();

        public override void Bind (VMBehaviour behaviour) {
            for (int i = 0; i < BindItems.Count; i++) {
                DataBinderItem item = BindItems[i];
                item.Source = item.Definer.GetData (behaviour);
                item.Converter = item.Definer.GetConverter (behaviour);
                item.DoBind (behaviour, this.gameObject);
            }
        }

        public override void Bind (VMBehaviour behaviour, IData source) {
            for (int i = 0; i < BindItems.Count; i++) {
                DataBinderItem item = BindItems[i];
                item.Source = item.Definer.GetData (source);
                item.Converter = item.Definer.GetConverter (behaviour);
                item.DoBind (behaviour, this.gameObject);
            }
        }

        public override void UnBind () {
            for (int i = 0; i < BindItems.Count; i++) {
                DataBinderItem item = BindItems[i];
                item.DoUnBind ();
            }
        }
    }
}