namespace VVMUI.Core.Data {
    public class FloatData : BaseData<float> {
        public FloatData () {
        }

        public FloatData (float v) {
            this.Set (v);
        }

        public static implicit operator FloatData (float v) {
            return new FloatData (v);
        }
    }
}