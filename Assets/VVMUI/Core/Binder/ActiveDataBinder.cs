using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using VVMUI.Core.Converter;
using VVMUI.Core.Data;

namespace VVMUI.Core.Binder {
    public class ActiveDataBinder : BaseDataBinder {
        public DataDefiner DataDefiner = new DataDefiner ();

        private IData source;
        private IConverter converter;
        private Action setValueHandler;

        private void DoBind (VMBehaviour behaviour) {
            if (source == null) {
                Debugger.LogError ("ActiveDataBinder", this.name + " data null.");
                return;
            }

            Type propertyType = typeof (bool);
            Type dataType = source.GetType ();
            Type dataBaseType = dataType.BaseType;

            bool bindData = false;
            if (converter != null) {
                bindData = true;
            }

            if (dataBaseType.IsGenericType && dataBaseType.GetGenericArguments () [0] == propertyType) {
                bindData = true;
            }

            if (bindData) {
                MethodInfo getMethod = dataType.GetMethod ("Get");
                setValueHandler = delegate () {
                    object value = getMethod.Invoke (source, null);
                    if (converter != null) {
                        value = converter.Convert (value, propertyType, DataDefiner.ConverterParameter, behaviour);
                    }
                    this.gameObject.SetActive ((bool) value);
                };
                source.ValueChanged += setValueHandler;
                setValueHandler.Invoke ();
            } else {
                Debugger.LogError ("ActiveDataBinder", this.name + " data type not explicit or converter is null.");
            }
        }

        public override void Bind (VMBehaviour behaviour) {
            source = DataDefiner.GetData (behaviour);
            converter = DataDefiner.GetConverter (behaviour);
            DoBind (behaviour);
        }

        public override void Bind (VMBehaviour behaviour, IData data) {
            source = DataDefiner.GetData (data);
            converter = DataDefiner.GetConverter (behaviour);
            DoBind (behaviour);
        }

        public override void UnBind () {
            if (source != null && setValueHandler != null) {
                source.ValueChanged -= setValueHandler;
            }
        }
    }
}