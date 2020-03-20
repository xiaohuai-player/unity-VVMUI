namespace VVMUI.Core.Data {
    [System.Serializable]
    public class IntData : BaseData<int> {
        [UnityEngine.SerializeField]
        private int editorValue;

        public IntData () { 
            this.Set (editorValue);
        }

        public IntData (int v) {
            this.Set (v);
        }

        public static implicit operator IntData (int v) {
            return new IntData (v);
        }
    }
}