using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using VVMUI.Core.Converter;
using VVMUI.Core.Data;

namespace VVMUI.Core.Binder
{
    public class BaseDataBinder : AbstractDataBinder
    {
        [Serializable]
        public class DataBinderItem
        {
            public enum BindType
            {
                Property,
                Active,
                Animation,
                Animator
            }

            public BindType Type;
            public DataDefiner Definer = new DataDefiner();

            // property bind vars
            public Component Component;
            public string Property;
            private Type componentType;
            private ReflectionCacheData componentReflection;

            // animator bind vars
            public int AnimatorLayer;

            [HideInInspector]
            public IBaseData Source;

            [HideInInspector]
            public IConverter Converter;

            [HideInInspector]
            public DataChangedHandler SetValueHandler;

            [HideInInspector]
            public object ValueChangedHandler;

            private static Type Toggle_Type = typeof(Toggle);
            private static Type Input_Type = typeof(InputField);
            private static Type Dropdown_Type = typeof(Dropdown);
            private static Type Slider_Type = typeof(Slider);

            private bool CheckDataTypeValid(Type t, GameObject obj)
            {
                if (this.Source == null)
                {
                    Debugger.LogError("DataBinder", obj.name + " data null.");
                    return false;
                }

                if (!this.Source.GetBindDataType().IsAssignableFrom(t) && this.Converter == null)
                {
                    Debugger.LogError("DataBinder", obj.name + " data type not explicit and converter is null.");
                    return false;
                }

                return true;
            }

            public void DoBind(VMBehaviour vm, GameObject obj)
            {
                switch (this.Type)
                {
                    case BindType.Property:
                        DoPropertyBind(vm, obj);
                        break;
                    case BindType.Active:
                        DoActiveBind(vm, obj);
                        break;
                    case BindType.Animation:
                        DoAnimationBind(vm, obj);
                        break;
                    case BindType.Animator:
                        DoAnimatorBind(vm, obj);
                        break;
                }
            }

            private void DoPropertyBind(VMBehaviour vm, GameObject obj)
            {
                if (this.Component == null)
                {
                    Debugger.LogError("DataBinder", obj.name + " component null.");
                    return;
                }

                if (string.IsNullOrEmpty(this.Property))
                {
                    Debugger.LogError("DataBinder", obj.name + " property empty.");
                    return;
                }

                if (componentType == null)
                {
                    componentType = this.Component.GetType();
                    componentReflection = ReflectionCache.Singleton[componentType];
                }

                PropertyInfo propertyInfo = componentReflection.GetProperty(this.Property);
                if (propertyInfo == null || propertyInfo.GetSetMethod() == null)
                {
                    Debugger.LogError("DataBinder", obj.name + " property null or not support.");
                    return;
                }

                Type propertyType = propertyInfo.PropertyType;
                if (!CheckDataTypeValid(propertyType, obj))
                {
                    return;
                }

                ISetValue propertySetter = SetterWrapper.CreatePropertySetterWrapper(propertyInfo);
                if (propertySetter == null)
                {
                    return;
                }

                // 数据单向绑定
                this.SetValueHandler = delegate (IData source)
                {
                    object value = this.Source.FastGetValue();
                    if (this.Converter != null)
                    {
                        value = this.Converter.Convert(value, propertyType, this.Definer.ConverterParameter, vm);
                    }
                    propertySetter.Set(this.Component, value);

                    // ToggleGroup 特殊处理
                    if (Toggle_Type.IsAssignableFrom(componentType) && this.Property.Equals("isOn"))
                    {
                        Toggle t = this.Component as Toggle;
                        if (t.group != null && t.isOn)
                        {
                            try
                            {
                                t.group.NotifyToggleOn(t);
                            }
                            catch (System.Exception) { }
                        }
                    }
                };
                this.Source.AddValueChangedListener(this.SetValueHandler);
                this.SetValueHandler.Invoke(this.Source);

                // 可交互组件的双向绑定
                if (Toggle_Type.IsAssignableFrom(componentType) && this.Property.Equals("isOn"))
                {
                    this.ValueChangedHandler = new UnityAction<bool>(delegate (bool arg)
                    {
                        if (this.Converter != null)
                        {
                            object value = this.Converter.ConvertBack(arg, this.Source.GetBindDataType(), this.Definer.ConverterParameter, vm);
                            this.Source.FastSetValue(value);
                        }
                        else
                        {
                            (this.Source as BaseData<bool>).Set(arg);
                        }
                    });
                    (this.Component as Toggle).onValueChanged.AddListener((UnityAction<bool>)this.ValueChangedHandler);
                }
                if (Input_Type.IsAssignableFrom(componentType) && this.Property.Equals("text"))
                {
                    this.ValueChangedHandler = new UnityAction<string>(delegate (string arg)
                    {
                        if (this.Converter != null)
                        {
                            object value = this.Converter.ConvertBack(arg, this.Source.GetBindDataType(), this.Definer.ConverterParameter, vm);
                            this.Source.FastSetValue(value);
                        }
                        else
                        {
                            (this.Source as BaseData<string>).Set(arg);
                        }
                    });
                    (this.Component as InputField).onValueChanged.AddListener((UnityAction<string>)this.ValueChangedHandler);
                }
                if (Dropdown_Type.IsAssignableFrom(componentType) && this.Property.Equals("value"))
                {
                    this.ValueChangedHandler = new UnityAction<int>(delegate (int arg)
                    {
                        if (this.Converter != null)
                        {
                            object value = this.Converter.ConvertBack(arg, this.Source.GetBindDataType(), this.Definer.ConverterParameter, vm);
                            this.Source.FastSetValue(value);
                        }
                        else
                        {
                            (this.Source as BaseData<int>).Set(arg);
                        }
                    });
                    (this.Component as Dropdown).onValueChanged.AddListener((UnityAction<int>)this.ValueChangedHandler);
                }
                if (Slider_Type.IsAssignableFrom(componentType) && this.Property.Equals("value"))
                {
                    this.ValueChangedHandler = new UnityAction<float>(delegate (float arg)
                    {
                        if (this.Converter != null)
                        {
                            object value = this.Converter.ConvertBack(arg, this.Source.GetBindDataType(), this.Definer.ConverterParameter, vm);
                            this.Source.FastSetValue(value);
                        }
                        else
                        {
                            (this.Source as BaseData<float>).Set(arg);
                        }
                    });
                    (this.Component as Slider).onValueChanged.AddListener((UnityAction<float>)this.ValueChangedHandler);
                }
            }

            private void DoActiveBind(VMBehaviour vm, GameObject obj)
            {
                Type propertyType = typeof(bool);
                if (!CheckDataTypeValid(propertyType, obj))
                {
                    return;
                }

                this.SetValueHandler = delegate (IData source)
                {
                    object value = this.Source.FastGetValue();
                    if (this.Converter != null)
                    {
                        value = this.Converter.Convert(value, propertyType, this.Definer.ConverterParameter, vm);
                    }
                    obj.SetActive((bool)value);
                };
                this.Source.AddValueChangedListener(this.SetValueHandler);
                this.SetValueHandler.Invoke(this.Source);
            }

            private void DoAnimationBind(VMBehaviour vm, GameObject obj)
            {
                Animation cptAnim = obj.GetComponent<Animation>();
                if (cptAnim == null)
                {
                    Debugger.LogError("DataBinder", obj.name + " Animation null.");
                    return;
                }

                Type propertyType = typeof(string);
                if (!CheckDataTypeValid(propertyType, obj))
                {
                    return;
                }

                this.SetValueHandler = delegate (IData source)
                {
                    object value = this.Source.FastGetValue();
                    if (this.Converter != null)
                    {
                        value = this.Converter.Convert(value, propertyType, this.Definer.ConverterParameter, vm);
                    }
                    cptAnim.Stop();
                    string animName = (string)value;
                    if (!string.IsNullOrEmpty(animName))
                    {
                        cptAnim.Play(animName, PlayMode.StopAll);
                    }
                };
                this.Source.AddValueChangedListener(this.SetValueHandler);
                this.SetValueHandler.Invoke(this.Source);
            }

            private void DoAnimatorBind(VMBehaviour vm, GameObject obj)
            {
                Animator cptAnim = obj.GetComponent<Animator>();
                if (cptAnim == null)
                {
                    Debugger.LogError("DataBinder", obj.name + " Animator null.");
                    return;
                }

                Type propertyType = typeof(string);
                if (!CheckDataTypeValid(propertyType, obj))
                {
                    return;
                }

                this.SetValueHandler = delegate (IData source)
                {
                    object value = this.Source.FastGetValue();
                    if (this.Converter != null)
                    {
                        value = this.Converter.Convert(value, propertyType, this.Definer.ConverterParameter, vm);
                    }
                    cptAnim.StopPlayback();
                    string animName = (string)value;
                    if (!string.IsNullOrEmpty(animName))
                    {
                        cptAnim.Play(animName, this.AnimatorLayer);
                    }
                };
                this.Source.AddValueChangedListener(this.SetValueHandler);
                this.SetValueHandler.Invoke(this.Source);
            }

            public void DoUnBind()
            {
                switch (this.Type)
                {
                    case BindType.Property:
                        DoPropertyUnBind();
                        break;
                    case BindType.Active:
                        DoActiveUnBind();
                        break;
                    case BindType.Animation:
                        DoAnimationUnBind();
                        break;
                    case BindType.Animator:
                        DoAnimatorUnBind();
                        break;
                }
            }

            private void DoPropertyUnBind()
            {
                if (this.Source != null && this.SetValueHandler != null)
                {
                    this.Source.RemoveValueChangedListener(this.SetValueHandler);
                    this.SetValueHandler = null;
                }
                if (this.ValueChangedHandler != null)
                {
                    Type cptType = this.Component.GetType();
                    if (Toggle_Type.IsAssignableFrom(cptType) && this.Property.Equals("isOn"))
                    {
                        (this.Component as Toggle).onValueChanged.RemoveListener((UnityAction<bool>)this.ValueChangedHandler);
                    }
                    if (Input_Type.IsAssignableFrom(cptType) && this.Property.Equals("text"))
                    {
                        (this.Component as InputField).onValueChanged.RemoveListener((UnityAction<string>)this.ValueChangedHandler);
                    }
                    if (Dropdown_Type.IsAssignableFrom(cptType) && this.Property.Equals("value"))
                    {
                        (this.Component as Dropdown).onValueChanged.RemoveListener((UnityAction<int>)this.ValueChangedHandler);
                    }
                    if (Slider_Type.IsAssignableFrom(cptType) && this.Property.Equals("value"))
                    {
                        (this.Component as Slider).onValueChanged.RemoveListener((UnityAction<float>)this.ValueChangedHandler);
                    }
                    this.ValueChangedHandler = null;
                }
            }

            private void DoActiveUnBind()
            {
                if (this.Source != null && this.SetValueHandler != null)
                {
                    this.Source.RemoveValueChangedListener(this.SetValueHandler);
                }
            }

            private void DoAnimationUnBind()
            {
                if (this.Source != null && this.SetValueHandler != null)
                {
                    this.Source.RemoveValueChangedListener(this.SetValueHandler);
                }
            }

            private void DoAnimatorUnBind()
            {
                if (this.Source != null && this.SetValueHandler != null)
                {
                    this.Source.RemoveValueChangedListener(this.SetValueHandler);
                }
            }
        }

        public List<DataBinderItem> BindItems = new List<DataBinderItem>();

        public override void Bind(VMBehaviour vm)
        {
            base.Bind(vm);

            for (int i = 0; i < BindItems.Count; i++)
            {
                DataBinderItem item = BindItems[i];

                IData source = item.Definer.GetData(vm);
                if (source == null || source.GetDataType() != DataType.Base)
                {
                    Debugger.LogError("DataBinder", this.gameObject.name + " data is not base data.");
                    continue;
                }

                item.Source = (IBaseData)source;
                item.Converter = item.Definer.GetConverter(vm);
                item.DoBind(vm, this.gameObject);
            }
        }

        public override void Bind(VMBehaviour vm, IData data)
        {
            base.Bind(vm, data);

            for (int i = 0; i < BindItems.Count; i++)
            {
                DataBinderItem item = BindItems[i];

                IData source = item.Definer.GetData(data);
                if (source == null || source.GetDataType() != DataType.Base)
                {
                    Debugger.LogError("DataBinder", this.gameObject.name + " data is not base data.");
                    continue;
                }

                item.Source = (IBaseData)source;
                item.Converter = item.Definer.GetConverter(vm);
                item.DoBind(vm, this.gameObject);
            }
        }

        public override void UnBind()
        {
            base.UnBind();

            for (int i = 0; i < BindItems.Count; i++)
            {
                DataBinderItem item = BindItems[i];
                item.DoUnBind();
            }
        }
    }
}