using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using VVMUI.Core.Converter;
using VVMUI.Core.Data;

namespace VVMUI.Core.Binder {
    public class ListTemplateBinder : MonoBehaviour {
        [SerializeField]
        private int index;
        private IListData source;
        private VMBehaviour vm;

        private List<AbstractDataBinder> dataBinders = new List<AbstractDataBinder> ();
        private List<AbstractCommandBinder> commandBinders = new List<AbstractCommandBinder> ();

        private void Awake () {
            this.gameObject.GetComponentsInChildren<AbstractDataBinder> (true, dataBinders);
            this.gameObject.GetComponentsInChildren<AbstractCommandBinder> (true, commandBinders);
        }

        private void OnDestroy () {
            this.UnBind ();
        }

        public void Bind (VMBehaviour vm, int index, IListData data) {
            this.index = index;
            this.source = data;
            this.vm = vm;

            for (int i = 0; i < dataBinders.Count; i++) {
                if (dataBinders[i].CanBind (this.vm, this.source.GetAt (this.index))) {
                    dataBinders[i].Bind (this.vm, this.source.GetAt (this.index));
                }
            }

            for (int i = 0; i < commandBinders.Count; i++) {
                if (commandBinders[i].CanBindListItem (this.vm, this.index)) {
                    commandBinders[i].BindListItem (this.vm, this.index);
                }
            }

            // 列表项由于是和索引对应的数据引用进行绑定，当数据引用发生改变时（比如列表顺序改变，某项数据引用改变），需要重新进行绑定
            this.source.AddItemValueChangedListener (index, ReBind);
        }

        public void UnBind () {
            if (this.source != null) {
                this.source.RemoveItemValueChangedListener (index, ReBind);
            }
            for (int i = 0; i < dataBinders.Count; i++) {
                dataBinders[i].UnBind ();
            }
            for (int i = 0; i < commandBinders.Count; i++) {
                commandBinders[i].UnBind ();
            }
        }

        private void ReBind () {
            for (int i = 0; i < dataBinders.Count; i++) {
                dataBinders[i].UnBind ();
                if (dataBinders[i].CanBind (this.vm, this.source.GetAt (this.index))) {
                    dataBinders[i].Bind (this.vm, this.source.GetAt (this.index));
                }
            }
        }
    }
}