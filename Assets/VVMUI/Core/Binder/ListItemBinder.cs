using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using VVMUI.Core.Converter;
using VVMUI.Core.Data;

namespace VVMUI.Core.Binder {
    [ExecuteInEditMode]
    public class ListItemBinder : MonoBehaviour {
        private int index;
        private IListData source;

        private void OnDestroy () {
            if (this.source != null) {
                this.source.RemoveItemValueChangedListener (index, Refresh);
            }
        }

        public void Bind (int index, IListData data) {
            this.index = index;
            this.source = data;
            this.source.AddItemValueChangedListener (index, Refresh);
            Refresh ();
        }

        private void Refresh () {

        }
    }
}