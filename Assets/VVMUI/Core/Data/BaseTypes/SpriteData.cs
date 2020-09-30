using UnityEngine;

namespace VVMUI.Core.Data
{
    [System.Serializable]
    public sealed class SpriteData : BaseData<Sprite>
    {
        public SpriteData()
        {
        }

        public SpriteData(Sprite v)
        {
            this.Set(v);
        }

        public static implicit operator Sprite(SpriteData d)
        {
            return d.Get();
        }
    }
}