using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using VVMUI.Core.Converter;
using VVMUI.Core.Data;

namespace VVMUI.Core.Binder {
    [ExecuteInEditMode]
    public class ListDataBinder : BaseDataBinder {
        public string SourceKey;
        public GameObject Template;

        private IListData sourceData;
        private VMBehaviour vm;

        public override void Bind (VMBehaviour vm) {
            IData data = vm.GetData (SourceKey);
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
                    GameObject obj = GameObject.Instantiate (Template.gameObject, this.transform);
                    ListItemBinder binder = obj.GetComponent<ListItemBinder> ();
                    if (binder != null) {
                        binder.Bind (this.vm, i, this.sourceData);
                    }
                }
            } else if (childCount > dataCount) {
                for (int i = dataCount; i < childCount; i++) {
                    GameObject.Destroy (this.transform.GetChild (i).gameObject);
                }
            }
        }
    }
}