using UnityEngine;
using UnityEngine.EventSystems;
using VVMUI.Core.Data;

namespace VVMUI.Core.Binder
{
    public abstract class AbstractDataBinder : UIBehaviour
    {
        public VMBehaviour BindVM { get; protected set; }

        public IData BindData { get; protected set; }

        public virtual bool CanBind(VMBehaviour vm)
        {
            return true;
        }

        public virtual bool CanBind(VMBehaviour vm, IData data)
        {
            return true;
        }

        public virtual void Bind(VMBehaviour vm) { this.BindVM = vm; }

        public virtual void Bind(VMBehaviour vm, IData data) { this.BindVM = vm; this.BindData = data; }

#if UNITY_EDITOR
        public void EditorBind()
        {
            if (!Application.isPlaying)
            {
                this.BindData = null;
                this.BindVM = this.GetComponentInParent<VMBehaviour>(true);
                if (this.BindVM != null)
                {
                    this.BindVM.EditorCollect();
                    if (this.GetComponentInParent<ListTemplateBinder>(true) != null)
                    {
                        IData data = this.GetComponentInParent<ListTemplateBinder>(true).ItemSource.GetData(this.BindVM);
                        IListData list = data as IListData;
                        if (list != null && list.Count > 0)
                        {
                            this.BindData = list.GetAt(0);
                        }
                    }
                }
            }
        }
#endif

        public virtual void UnBind() { this.BindVM = null; this.BindData = null; }
    }

    public abstract class AbstractCommandBinder : UIBehaviour
    {
        public VMBehaviour BindVM { get; protected set; }

        public virtual bool CanBind(VMBehaviour vm)
        {
            return true;
        }

        public virtual void Bind(VMBehaviour vm) { this.BindVM = vm; }

        public virtual bool CanBindListItem(VMBehaviour vm, int index)
        {
            return true;
        }

        public virtual void BindListItem(VMBehaviour vm, int index)
        {
            this.BindVM = vm;
        }

#if UNITY_EDITOR
        public void EditorBind()
        {
            if (!Application.isPlaying)
            {
                this.BindVM = this.GetComponentInParent<VMBehaviour>(true);
                if (this.BindVM != null)
                {
                    this.BindVM.EditorCollect();
                }
            }
        }
#endif

        public virtual void UnBind() { this.BindVM = null; }
    }
}