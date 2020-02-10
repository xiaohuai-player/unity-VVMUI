using UnityEngine;

namespace VVMUI.Core.Data {
    public class Texture2DData : BaseData<Texture2D> {
        public Texture2DData () {
        }

        public Texture2DData (Texture2D v) {
            this.Set (v);
        }

        public static implicit operator Texture2DData (Texture2D v) {
            return new Texture2DData (v);
        }
    }
}