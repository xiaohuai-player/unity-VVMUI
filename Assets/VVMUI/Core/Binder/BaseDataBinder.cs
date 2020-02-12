using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using VVMUI.Core.Converter;
using VVMUI.Core.Data;

namespace VVMUI.Core.Binder {
    public class BaseDataBinder : AbstractDataBinder {
        [Serializable]
        public class DataBinderItem {
            public enum BindType {
                Property,
                Active,
                Animation,
                Animator
            }

            public BindType Type;
            public DataDefiner Definer = new DataDefiner ();

            // property bind vars
            public Component Component;
            public string Property;

            // active bind vars
            public GameObject GameObject;

            // animation bind vars
            public Animation Animation;

            // animator bind vars
            public Animator Animator;
            public int AnimatorLayer;

            [HideInInspector]
            public IData Source;

            [HideInInspector]
            public IConverter Converter;

            [HideInInspector]
            public Action SetValueHandler;

            [HideInInspector]
            public object ValueChangedHandler;

            private bool CheckDataTypeValid (Type t, GameObject obj) {
                if (this.Source == null) {
                    Debugger.LogError ("DataBinder", obj.name + " data null.");
                    return false;
                }

                Type propertyType = t;
                Type dataType = this.Source.GetType ();
                Type dataBaseType = dataType.BaseType;

                bool bindData = false;
                if (this.Converter != null) {
                    bindData = true;
                }

                if (this.Source.GetDataType () == propertyType) {
                    bindData = true;
                }

                if (!bindData) {
                    Debugger.LogError ("DataBinder", obj.name + " data type not explicit or converter is null.");
                    return false;
                }

                return true;
            }

            public void DoBind (VMBehaviour vm, GameObject obj) {
                switch (this.Type) {
                    case BindType.Property:
                        DoPropertyBind (vm, obj);
                        break;
                    case BindType.Active:
                        DoActiveBind (vm, obj);
                        break;
                    case BindType.Animation:
                        DoAnimationBind (vm, obj);
                        break;
                    case BindType.Animator:
                        DoAnimatorBind (vm, obj);
                        break;
                }
            }

            private void DoPropertyBind (VMBehaviour vm, GameObject obj) {
                if (this.Component == null) {
                    Debugger.LogError ("DataBinder", obj.name + " component null.");
                    return;
                }
                if (string.IsNullOrEmpty (this.Property)) {
                    Debugger.LogError ("DataBinder", obj.name + " property empty.");
                    return;
                }

                Type componentType = this.Component.GetType ();
                // TODO 性能优化：Type.GetProperty
                PropertyInfo propertyInfo = componentType.GetProperty (this.Property);
                if (propertyInfo == null || propertyInfo.GetSetMethod () == null) {
                    Debugger.LogError ("DataBinder", obj.name + " property null or not support.");
                    return;
                }

                Type propertyType = propertyInfo.PropertyType;
                if (!CheckDataTypeValid (propertyType, obj)) {
                    return;
                }

                ISetValue propertySetter = SetterWrapper.CreatePropertySetterWrapper (propertyInfo);

                // TODO 性能优化：Type.GetMethod
                Type dataType = this.Source.GetType ();
                MethodInfo setMethod = dataType.GetMethod ("Set");
                MethodInfo getMethod = dataType.GetMethod ("Get");
                ISetValue sourceSetter = SetterWrapper.CreateMethodSetterWrapper (setMethod);
                IGetValue sourceGetter = GetterWrapper.CreateMethodGetterWrapper (getMethod);
                
                this.SetValueHandler = delegate () {
                    object value = sourceGetter.Get (this.Source);
                    if (this.Converter != null) {
                        value = this.Converter.Convert (value, propertyType, this.Definer.ConverterParameter, vm);
                    }
                    propertySetter.Set (this.Component, value);
                };
                this.Source.ValueChanged += this.SetValueHandler;
                this.SetValueHandler.Invoke ();

                // 可交互组件的双向绑定
                if (componentType == typeof (Toggle) && this.Property.Equals ("isOn")) {
                    this.ValueChangedHandler = new UnityAction<bool> (delegate (bool arg) {
                        if (this.Converter != null) {
                            object value = this.Converter.ConvertBack (arg, this.Source.GetDataType (), this.Definer.ConverterParameter, vm);
                            sourceSetter.Set (this.Source, value);
                        } else {
                            (this.Source as BoolData).Set (arg);
                        }
                    });
                    (this.Component as Toggle).onValueChanged.AddListener ((UnityAction<bool>) this.ValueChangedHandler);
                }
                if (componentType == typeof (InputField) && this.Property.Equals ("text")) {
                    this.ValueChangedHandler = new UnityAction<string> (delegate (string arg) {
                        if (this.Converter != null) {
                            object value = this.Converter.ConvertBack (arg, this.Source.GetDataType (), this.Definer.ConverterParameter, vm);
                            sourceSetter.Set (this.Source, value);
                        } else {
                            (this.Source as StringData).Set (arg);
                        }
                    });
                    (this.Component as InputField).onValueChanged.AddListener ((UnityAction<string>) this.ValueChangedHandler);
                }
                if (componentType == typeof (Dropdown) && this.Property.Equals ("value")) {
                    this.ValueChangedHandler = new UnityAction<int> (delegate (int arg) {
                        if (this.Converter != null) {
                            object value = this.Converter.ConvertBack (arg, this.Source.GetDataType (), this.Definer.ConverterParameter, vm);
                            sourceSetter.Set (this.Source, value);
                        } else {
                            (this.Source as IntData).Set (arg);
                        }
                    });
                    (this.Component as Dropdown).onValueChanged.AddListener ((UnityAction<int>) this.ValueChangedHandler);
                }
                if (componentType == typeof (Slider) && this.Property.Equals ("value")) {
                    this.ValueChangedHandler = new UnityAction<float> (delegate (float arg) {
                        if (this.Converter != null) {
                            object value = this.Converter.ConvertBack (arg, this.Source.GetDataType (), this.Definer.ConverterParameter, vm);
                            sourceSetter.Set (this.Source, value);
                        } else {
                            (this.Source as FloatData).Set (arg);
                        }
                    });
                    (this.Component as Slider).onValueChanged.AddListener ((UnityAction<float>) this.ValueChangedHandler);
                }
            }

            private void DoActiveBind (VMBehaviour vm, GameObject obj) {
                if (this.GameObject == null) {
                    Debugger.LogError ("DataBinder", obj.name + " gameobject null.");
                    return;
                }

                Type propertyType = typeof (bool);
                if (!CheckDataTypeValid (propertyType, obj)) {
                    return;
                }

                // TODO 性能优化：Type.GetMethod
                Type dataType = this.Source.GetType ();
                MethodInfo getMethod = dataType.GetMethod ("Get");
                IGetValue sourceGetter = GetterWrapper.CreateMethodGetterWrapper (getMethod);
                this.SetValueHandler = delegate () {
                    object value = sourceGetter.Get(this.Source);
                    if (this.Converter != null) {
                        value = this.Converter.Convert (value, propertyType, this.Definer.ConverterParameter, vm);
                    }
                    this.GameObject.SetActive ((bool) value);
                };
                this.Source.ValueChanged += this.SetValueHandler;
                this.SetValueHandler.Invoke ();
            }

            private void DoAnimationBind (VMBehaviour vm, GameObject obj) {
                if (this.Animation == null) {
                    Debugger.LogError ("DataBinder", obj.name + " Animation null.");
                    return;
                }

                Type propertyType = typeof (string);
                if (!CheckDataTypeValid (propertyType, obj)) {
                    return;
                }

                // TODO 性能优化：Type.GetMethod
                Type dataType = this.Source.GetType ();
                MethodInfo getMethod = dataType.GetMethod ("Get");
                IGetValue sourceGetter = GetterWrapper.CreateMethodGetterWrapper (getMethod);
                this.SetValueHandler = delegate () {
                    object value = sourceGetter.Get(this.Source);
                    if (this.Converter != null) {
                        value = this.Converter.Convert (value, propertyType, this.Definer.ConverterParameter, vm);
                    }
                    this.Animation.Stop ();
                    this.Animation.Play ((string) value, PlayMode.StopAll);
                };
                this.Source.ValueChanged += this.SetValueHandler;
                this.SetValueHandler.Invoke ();
            }

            private void DoAnimatorBind (VMBehaviour vm, GameObject obj) {
                if (this.Animator == null) {
                    Debugger.LogError ("DataBinder", obj.name + " Animator null.");
                    return;
                }

                Type propertyType = typeof (string);
                if (!CheckDataTypeValid (propertyType, obj)) {
                    return;
                }

                // TODO 性能优化：Type.GetMethod
                Type dataType = this.Source.GetType ();
                MethodInfo getMethod = dataType.GetMethod ("Get");
                IGetValue sourceGetter = GetterWrapper.CreateMethodGetterWrapper (getMethod);
                this.SetValueHandler = delegate () {
                    object value = sourceGetter.Get(this.Source);
                    if (this.Converter != null) {
                        value = this.Converter.Convert (value, propertyType, this.Definer.ConverterParameter, vm);
                    }
                    this.Animator.Play ((string) value, this.AnimatorLayer);
                };
                this.Source.ValueChanged += this.SetValueHandler;
                this.SetValueHandler.Invoke ();
            }

            public void DoUnBind () {
                switch (this.Type) {
                    case BindType.Property:
                        DoPropertyUnBind ();
                        break;
                    case BindType.Active:
                        DoActiveUnBind ();
                        break;
                    case BindType.Animation:
                        DoAnimationUnBind ();
                        break;
                    case BindType.Animator:
                        DoAnimatorUnBind ();
                        break;
                }
            }

            private void DoPropertyUnBind () {
                if (this.Source != null && this.SetValueHandler != null) {
                    this.Source.ValueChanged -= this.SetValueHandler;
                }
                if (this.ValueChangedHandler != null) {
                    Type componentType = this.Component.GetType ();
                    if (componentType == typeof (Toggle) && this.Property.Equals ("isOn")) {
                        (this.Component as Toggle).onValueChanged.RemoveListener ((UnityAction<bool>) this.ValueChangedHandler);
                    }
                    if (componentType == typeof (InputField) && this.Property.Equals ("text")) {
                        (this.Component as InputField).onValueChanged.RemoveListener ((UnityAction<string>) this.ValueChangedHandler);
                    }
                    if (componentType == typeof (Dropdown) && this.Property.Equals ("value")) {
                        (this.Component as Dropdown).onValueChanged.RemoveListener ((UnityAction<int>) this.ValueChangedHandler);
                    }
                    if (componentType == typeof (Slider) && this.Property.Equals ("value")) {
                        (this.Component as Slider).onValueChanged.RemoveListener ((UnityAction<float>) this.ValueChangedHandler);
                    }
                }
            }

            private void DoActiveUnBind () {
                if (this.Source != null && this.SetValueHandler != null) {
                    this.Source.ValueChanged -= this.SetValueHandler;
                }
            }

            private void DoAnimationUnBind () {
                if (this.Source != null && this.SetValueHandler != null) {
                    this.Source.ValueChanged -= this.SetValueHandler;
                }
            }

            private void DoAnimatorUnBind () {
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