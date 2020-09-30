using System.Collections.Generic;
using UnityEngine;
using VVMUI.Core.Data;

namespace VVMUI.Core.Binder
{
    public class VMDataBinder : AbstractDataBinder
    {
        public DataDefiner Source;
        public GameObject Template;

        private StructData sourceData;

        private GameObject obj;
        private VMBehaviour objVM;

        private Dictionary<IData, DataChangedHandler> handlers = new Dictionary<IData, DataChangedHandler>();

        protected override void OnEnable()
        {
            base.OnEnable();
            DoBind();
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            DoUnBind();
        }

        public override void Bind(VMBehaviour vm)
        {
            if (Source == null)
            {
                return;
            }

            if (Template == null)
            {
                return;
            }

            this.sourceData = Source.GetData(vm) as StructData;
            if (this.sourceData == null)
            {
                return;
            }

            DoBind();
        }

        public override void Bind(VMBehaviour vm, IData data)
        {
            if (Source == null)
            {
                return;
            }

            if (Template == null)
            {
                return;
            }

            if (data == null)
            {
                return;
            }

            this.sourceData = Source.GetData(data) as StructData;
            if (this.sourceData == null)
            {
                return;
            }

            DoBind();
        }

        public override void UnBind()
        {
            DoUnBind();
        }

        private void DoBind()
        {
            if (!this.isActiveAndEnabled)
            {
                return;
            }

            if (this.Template == null || this.sourceData == null)
            {
                return;
            }

            if (this.obj != null)
            {
                Destroy(this.obj);
            }

            this.obj = GameObject.Instantiate(this.Template, this.transform);
            this.obj.SetActive(true);
            this.objVM = this.obj.GetComponent<VMBehaviour>();

            this.handlers.Clear();
            foreach (KeyValuePair<string, IData> kv in this.sourceData.Fields)
            {
                IData dstData = this.objVM.GetData(kv.Key);
                IData srcData = this.sourceData[kv.Key];
                if (dstData != null && srcData.GetType().IsAssignableFrom(dstData.GetType()))
                {
                    DataChangedHandler handler = delegate (IData src)
                    {
                        dstData.CopyFrom(src);
                    };
                    handler.Invoke(srcData);
                    srcData.AddValueChangedListener(handler);
                    this.handlers[srcData] = handler;
                }
            }
        }

        private void DoUnBind()
        {
            foreach (KeyValuePair<IData, DataChangedHandler> kv in handlers)
            {
                if (kv.Key != null)
                {
                    kv.Key.RemoveValueChangedListener(kv.Value);
                }
            }
            this.handlers.Clear();

            if (this.obj != null)
            {
                Destroy(this.obj);
            }
        }
    }
}