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
        [SerializeField]
        private int index;
        private IListData source;
        private VMBehaviour vm;

        private List<BaseDataBinder> dataBinders = new List<BaseDataBinder> ();
        private List<BaseCommandBinder> commandBinders = new List<BaseCommandBinder> ();

        private void Awake () {
            this.gameObject.GetComponentsInChildren<BaseDataBinder> (true, dataBinders);
            this.gameObject.GetComponentsInChildren<BaseCommandBinder> (true, commandBinders);
        }

        private void OnDestroy () {
            this.UnBind ();
        }

        public void Bind (VMBehaviour vm, int index, IListData data) {
            this.index = index;
            this.source = data;
            this.vm = vm;
            this.source.AddItemValueChangedListener (index, Refresh);
            Refresh ();
        }

        public void UnBind () {
            if (this.source != null) {
                this.source.RemoveItemValueChangedListener (index, Refresh);
            }
            for (int i = 0; i < dataBinders.Count; i++) {
                dataBinders[i].UnBind ();
            }
            for (int i = 0; i < commandBinders.Count; i++) {
                commandBinders[i].UnBind ();
            }
        }

        private void Refresh () {
            for (int i = 0; i < dataBinders.Count; i++) {
                dataBinders[i].UnBind ();
                if (dataBinders[i].CanBind (this.vm, this.source.GetAt (this.index))) {
                    dataBinders[i].Bind (this.vm, this.source.GetAt (this.index));
                }
            }
            for (int i = 0; i < commandBinders.Count; i++) {
                commandBinders[i].UnBind ();
                if (commandBinders[i].CanBind (this.vm)) {
                    commandBinders[i].Bind (this.vm);
                }
            }
        }
    }
}