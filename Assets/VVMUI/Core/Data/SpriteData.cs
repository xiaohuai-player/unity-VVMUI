using UnityEngine;

namespace VVMUI.Core.Data {
    public class SpriteData : BaseData<Sprite> {
        public SpriteData () {
        }

        public SpriteData (Sprite v) {
            this.Set (v);
        }

        public static implicit operator SpriteData (Sprite v) {
            return new SpriteData (v);
        }
    }
}