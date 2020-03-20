namespace VVMUI.Core.Data {
    [System.Serializable]
    public class StringData : BaseData<string> {
        [UnityEngine.SerializeField]
        private string editorValue;

        public StringData () {
            this.Set (editorValue);
        }

        public StringData (string v) {
            this.Set (v);
        }

        public static implicit operator StringData (string v) {
            return new StringData (v);
        }
    }
}