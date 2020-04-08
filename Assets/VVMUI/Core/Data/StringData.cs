namespace VVMUI.Core.Data {
    [System.Serializable]
    public sealed class StringData : BaseData<string> {
        public StringData () {
        }

        public StringData (string v) {
            this.Set (v);
        }

        public static implicit operator string (StringData d) {
			return d.Get ();
		}
    }
}