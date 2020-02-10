using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using VVMUI.Core.Converter;
using VVMUI.Core.Data;

namespace VVMUI.Core.Binder {
    public class ListDataBinder : AbstractDataBinder {
        public DataDefiner Source;
        public GameObject Template;

        private IListData sourceData;
        private VMBehaviour vm;

        public override void Bind (VMBehaviour vm) {
            if (Source == null) {
                return;
            }

            IData data = Source.GetData (vm);
            if (data == null) {
                return;
            }

            this.sourceData = data as IListData;
            if (this.sourceData == null) {
                return;
            }

            this.vm = vm;

            for (int i = 0; i < this.transform.childCount; i++) {
                Destroy (this.transform.GetChild (i).gameObject);
            }

            (this.sourceData as IData).ValueChanged += Arrange;

            Arrange ();
        }

        public override void Bind (VMBehaviour vm, IData data) {
            if (Source == null) {
                return;
            }

            if (data == null) {
                return;
            }

            this.sourceData = Source.GetData (data) as IListData;
            if (this.sourceData == null) {
                return;
            }

            this.vm = vm;

            for (int i = 0; i < this.transform.childCount; i++) {
                Destroy (this.transform.GetChild (i).gameObject);
            }

            (this.sourceData as IData).ValueChanged += Arrange;

            Arrange ();
        }

        public override void UnBind () {
            (this.sourceData as IData).ValueChanged -= Arrange;
        }

        private void Arrange () {
            if (Template == null) {
                return;
            }

            int childCount = this.transform.childCount;
            int dataCount = (sourceData as IList).Count;
            if (childCount < dataCount) {
                for (int i = childCount; i < dataCount; i++) {
                    GameObject obj = GameObject.Instantiate (Template, this.transform);
                    ListTemplateBinder binder = obj.GetComponent<ListTemplateBinder> ();
                    if (binder != null) {
                        binder.Bind (this.vm, i, this.sourceData);
                    }
                }
            } else if (childCount > dataCount) {
                for (int i = dataCount; i < childCount; i++) {
                    GameObject obj = this.transform.GetChild (i).gameObject;
                    ListTemplateBinder binder = obj.GetComponent<ListTemplateBinder> ();
                    if (binder != null) {
                        binder.UnBind ();
                    }
                    GameObject.Destroy (obj);
                }
            }
        }
    }
}