namespace VVMUI.Core.Data {
    [System.Serializable]
    public class BoolData : BaseData<bool> {
        [UnityEngine.SerializeField]
        private bool editorValue;

        public BoolData () {
            this.Set (editorValue);
        }

        public BoolData (bool v) {
            this.Set (v);
        }

        public static implicit operator BoolData (bool v) {
            return new BoolData (v);
        }
    }
}