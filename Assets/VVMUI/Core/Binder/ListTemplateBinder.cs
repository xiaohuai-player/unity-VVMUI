using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using VVMUI.Core.Data;

namespace VVMUI.Core.Binder
{
    public class ListTemplateBinder : UIBehaviour
    {
        public DataDefiner ItemSource = new DataDefiner();

        [SerializeField]
        private int index;
        public int Index
        {
            get { return index; }
        }

        private IData item;
        private IListData source;
        private VMBehaviour vm;

        private List<AbstractDataBinder> dataBinders = new List<AbstractDataBinder>();
        private List<AbstractCommandBinder> commandBinders = new List<AbstractCommandBinder>();

        protected override void Awake()
        {
            base.Awake();
            this.gameObject.GetComponentsInChildren<AbstractDataBinder>(true, dataBinders);
            this.gameObject.GetComponentsInChildren<AbstractCommandBinder>(true, commandBinders);
        }

        protected override void OnDestroy()
        {
            this.UnBind();
        }

        public void Bind(VMBehaviour vm, int index, IListData data)
        {
            this.index = index;
            this.source = data;
            this.item = this.source.GetAt(this.index);
            this.vm = vm;

            for (int i = 0; i < dataBinders.Count; i++)
            {
                if (dataBinders[i].CanBind(this.vm, item))
                {
                    dataBinders[i].Bind(this.vm, item);
                }
            }

            for (int i = 0; i < commandBinders.Count; i++)
            {
                if (commandBinders[i].CanBindListItem(this.vm, this.index))
                {
                    commandBinders[i].BindListItem(this.vm, this.index);
                }
            }
        }

        public void UnBind()
        {
            for (int i = 0; i < dataBinders.Count; i++)
            {
                dataBinders[i].UnBind();
            }
            for (int i = 0; i < commandBinders.Count; i++)
            {
                commandBinders[i].UnBind();
            }
        }

        private void ReBind()
        {
            this.UnBind();
            this.Bind(this.vm, this.index, this.source);
        }

        public void ReBind(int index)
        {
            if (index < 0 || index >= this.source.Count)
            {
                this.UnBind();
                return;
            }

            if (index != this.index || this.source.GetAt(index) != this.item)
            {
                this.index = index;
                this.ReBind();
            }
        }
    }
}