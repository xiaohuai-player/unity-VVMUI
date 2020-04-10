using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using VVMUI.Core.Converter;
using VVMUI.Core.Data;

namespace VVMUI.Core.Binder {
    public class VMDataBinder : AbstractDataBinder {
        public DataDefiner Source;
        public GameObject Template;

        private StructData sourceData;
        private VMBehaviour vm;
        private GameObject obj;
        private VMBehaviour objVM;
        private Dictionary<string, IGetValue> getters = new Dictionary<string, IGetValue> ();
        private Dictionary<string, ISetValue> setters = new Dictionary<string, ISetValue> ();
        private Dictionary<string, Action> handlers = new Dictionary<string, Action> ();

        public override void Bind (VMBehaviour vm) {
            if (Source == null) {
                return;
            }

            if (Template == null) {
                return;
            }

            this.sourceData = Source.GetData (vm) as StructData;
            if (this.sourceData == null) {
                return;
            }

            this.vm = vm;

            DoBind ();
        }

        public override void Bind (VMBehaviour vm, IData data) {
            if (Source == null) {
                return;
            }

            if (Template == null) {
                return;
            }

            if (data == null) {
                return;
            }

            this.sourceData = Source.GetData (data) as StructData;
            if (this.sourceData == null) {
                return;
            }

            this.vm = vm;

            DoBind ();
        }

        public override void UnBind () {
            DoUnBind ();
        }

        private void DoBind () {
            this.obj = GameObject.Instantiate (this.Template, this.transform);
            this.obj.SetActive (true);
            this.objVM = this.obj.GetComponent<VMBehaviour> ();

            getters.Clear ();
            setters.Clear ();
            handlers.Clear ();
            foreach (KeyValuePair<string, IData> kv in this.sourceData.Fields) {
                IData dstData = this.objVM.GetData (kv.Key);
                IData srcData = this.sourceData[kv.Key];
                if (dstData != null) {
                    string key = kv.Key;
                    setters[key] = dstData.Setter;
                    getters[key] = srcData.Getter;

                    Action handler = delegate () {
                        setters[key].Set (dstData, getters[key].Get (srcData));
                    };
                    handlers[key] = handler;
                    srcData.ValueChanged += handler;
                    handler.Invoke ();
                }
            }
        }

        private void DoUnBind () {
            foreach (KeyValuePair<string, Action> kv in handlers) {
                IData srcData = this.sourceData[kv.Key];
                srcData.ValueChanged -= kv.Value;
            }
            if (this.obj != null) {
                Destroy (this.obj);
            }
        }
    }
}