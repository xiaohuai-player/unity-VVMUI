using UnityEngine;

namespace VVMUI.Core.Data
{
    [System.Serializable]
    public sealed class Vector3Data : BaseData<Vector3>
    {
        public Vector3Data()
        {
        }

        public Vector3Data(Vector3 v)
        {
            this.Set(v);
        }

        public static implicit operator Vector3(Vector3Data d)
        {
            return d.Get();
        }
    }
}