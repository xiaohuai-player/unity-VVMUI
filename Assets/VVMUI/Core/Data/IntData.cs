namespace VVMUI.Core.Data {
    public class IntData : BaseData<int> {
        public IntData () { }

        public IntData (int v) {
            this.Set (v);
        }

        public static implicit operator IntData (int v) {
            return new IntData (v);
        }
    }
}