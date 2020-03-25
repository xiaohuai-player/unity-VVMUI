namespace VVMUI.Core.Data {
    [System.Serializable]
    public class BoolData : BaseData<bool> {
        public BoolData () {
        }

        public BoolData (bool v) {
            this.Set (v);
        }

        public static implicit operator BoolData (bool v) {
            return new BoolData (v);
        }

        public static implicit operator bool (BoolData d) {
			return d.Get ();
		}
    }
}