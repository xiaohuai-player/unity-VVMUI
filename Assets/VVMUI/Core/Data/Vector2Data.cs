using UnityEngine;

namespace VVMUI.Core.Data
{
    [System.Serializable]
    public sealed class Vector2Data : BaseData<Vector2>
    {
        public Vector2Data()
        {
        }

        public Vector2Data(Vector2 v)
        {
            this.Set(v);
        }

        public static implicit operator Vector2(Vector2Data d)
        {
            return d.Get();
        }
    }
}