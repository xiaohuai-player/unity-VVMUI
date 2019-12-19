namespace VVMUI.Core.Data {
    public class StringData : BaseData<string> {
        public StringData () { }

        public StringData (string v) {
            this.Set (v);
        }

        public static implicit operator StringData (string v) {
            return new StringData (v);
        }
    }
}