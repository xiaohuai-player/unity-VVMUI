using UnityEngine;

namespace VVMUI.Core.Data
{
    [System.Serializable]
    public sealed class ColorData : BaseData<Color>
    {
        public ColorData()
        {
        }

        public ColorData(Color v)
        {
            this.Set(v);
        }

        public static implicit operator Color(ColorData d)
        {
            return d.Get();
        }
    }
}