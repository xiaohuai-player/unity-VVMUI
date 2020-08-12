using UnityEngine;

namespace VVMUI.Core.Data {
    [System.Serializable]
    public sealed class TextureData : BaseData<Texture> {
        public TextureData () {
        }

        public TextureData (Texture v) {
            this.Set (v);
        }

        public static implicit operator Texture (TextureData d) {
			return d.Get ();
		}
    }
}