using UnityEngine;
using VVMUI.Core.Data;

namespace VVMUI.Core.Binder {
    public abstract class BaseDataBinder : MonoBehaviour {
        public virtual void Bind (VMBehaviour vm) { }
        public virtual void Bind (VMBehaviour vm, IData data) { }
        public virtual void UnBind () { }
    }

    public abstract class BaseCommandBinder : MonoBehaviour {
        public virtual void Bind (VMBehaviour vm) { }
        public virtual void UnBind () { }
    }
}