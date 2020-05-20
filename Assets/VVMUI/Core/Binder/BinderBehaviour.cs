using UnityEngine;
using UnityEngine.EventSystems;
using VVMUI.Core.Data;

namespace VVMUI.Core.Binder {
    public abstract class AbstractDataBinder : UIBehaviour {
        public virtual bool CanBind (VMBehaviour vm) {
            return true;
        }

        public virtual void Bind (VMBehaviour vm) { }

        public virtual bool CanBind (VMBehaviour vm, IData data) {
            return true;
        }

        public virtual void Bind (VMBehaviour vm, IData data) { }

        public virtual void UnBind () { }
    }

    public abstract class AbstractCommandBinder : UIBehaviour {
        public virtual bool CanBind (VMBehaviour vm) {
            return true;
        }

        public virtual void Bind (VMBehaviour vm) { }

        public virtual bool CanBindListItem (VMBehaviour vm, int index) {
            return true;
        }

        public virtual void BindListItem (VMBehaviour vm, int index) {

        }

        public virtual void UnBind () { }
    }
}