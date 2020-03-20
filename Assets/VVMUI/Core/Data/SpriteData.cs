using UnityEngine;

namespace VVMUI.Core.Data {
    [System.Serializable]
    public class SpriteData : BaseData<Sprite> {
        [SerializeField]
        private Sprite editorValue;

        public SpriteData () {
            if (editorValue != null) {
                this.Set (editorValue);
            }
        }

        public SpriteData (Sprite v) {
            this.Set (v);
        }

        public static implicit operator SpriteData (Sprite v) {
            return new SpriteData (v);
        }
    }
}