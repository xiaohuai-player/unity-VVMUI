using System;
using UnityEngine;

namespace VVMUI.Core.Data
{
    [System.Serializable]
    [Obsolete("using TextureData instead.")]
    public sealed class Texture2DData : BaseData<Texture2D>
    {
        public Texture2DData()
        {
        }

        public Texture2DData(Texture2D v)
        {
            this.Set(v);
        }

        public static implicit operator Texture2D(Texture2DData d)
        {
            return d.Get();
        }
    }
}