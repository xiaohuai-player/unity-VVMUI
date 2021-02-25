using UnityEngine;

namespace VVMUI.Core.Data
{
    [System.Serializable]
    public sealed class RectData : BaseData<Rect>
    {
        public RectData()
        {
        }

        public RectData(Rect v)
        {
            this.Set(v);
        }

        public static implicit operator Rect(RectData d)
        {
            return d.Get();
        }
    }
}
