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

        public override void Bind (VMBehaviour behaviour) {
            IData data = behaviour.GetData (SourceKey);
            if (data == null) {
                return;
            }

            sourceData = data as IListData;
            if (sourceData == null) {
                return;
            }

            for (int i = 0; i < this.transform.childCount; i++) {
                Destroy (this.transform.GetChild (i).gameObject);
            }

            (sourceData as IData).ValueChanged += Arrange;

            Arrange ();
        }

        public override void Bind (VMBehaviour vm, IData data) {
            if (data == null) {
                return;
            }

            sourceData = data as IListData;
            if (sourceData == null) {
                return;
            }

            for (int i = 0; i < this.transform.childCount; i++) {
                Destroy (this.transform.GetChild (i).gameObject);
            }

            (sourceData as IData).ValueChanged += Arrange;

            Arrange ();
        }

        public override void UnBind () {
            (sourceData as IData).ValueChanged -= Arrange;
        }

        private void Arrange () {
            if (Template == null) {
                return;
            }

            int childCount = this.transform.childCount;
            int dataCount = (sourceData as IList).Count;
            if (childCount < dataCount) {
                for (int i = childCount; i < dataCount; i++) {
                    GameObject.Instantiate (Template.gameObject, this.transform);
                }
            } else if (childCount > dataCount) {
                for (int i = dataCount; i < childCount; i++) {
                    GameObject.Destroy (this.transform.GetChild (i).gameObject);
                }
            }
        }
    }
}