namespace VVMUI.Core.Data {
    [System.Serializable]
    public class FloatData : BaseData<float> {
        [UnityEngine.SerializeField]
        private float editorValue;

        public FloatData () {
            this.Set (editorValue);
        }

        public FloatData (float v) {
            this.Set (v);
        }

        public static implicit operator FloatData (float v) {
            return new FloatData (v);
        }
    }
}