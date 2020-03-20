using UnityEngine;

namespace VVMUI.Core.Data {
    [System.Serializable]
    public class Texture2DData : BaseData<Texture2D> {
        [UnityEngine.SerializeField]
        private Texture2D editorValue;

        public Texture2DData () {
            this.Set (editorValue);
        }

        public Texture2DData (Texture2D v) {
            this.Set (v);
        }

        public static implicit operator Texture2DData (Texture2D v) {
            return new Texture2DData (v);
        }
    }
}