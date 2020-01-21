using UnityEngine;
using VVMUI.Core.Data;

namespace VVMUI.Core.Binder {
    public abstract class BaseDataBinder : MonoBehaviour {
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

    public abstract class BaseCommandBinder : MonoBehaviour {
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