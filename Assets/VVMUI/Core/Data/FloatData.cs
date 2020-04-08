namespace VVMUI.Core.Data {
    [System.Serializable]
    public sealed class FloatData : BaseData<float> {
        public FloatData () {
        }

        public FloatData (float v) {
            this.Set (v);
        }

        public static implicit operator float (FloatData d) {
			return d.Get ();
		}
    }
}