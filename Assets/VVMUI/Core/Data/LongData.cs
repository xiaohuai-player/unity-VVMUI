namespace VVMUI.Core.Data {
    [System.Serializable]
    public class LongData : BaseData<long> {
        [UnityEngine.SerializeField]
        private long editorValue;

        public LongData () {
            this.Set (editorValue);
        }

        public LongData (long v) {
            this.Set (v);
        }

        public static implicit operator LongData (long v) {
            return new LongData (v);
        }
    }
}