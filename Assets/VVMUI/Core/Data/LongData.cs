namespace VVMUI.Core.Data {
    public class LongData : BaseData<long> {
        public LongData () { }

        public LongData (long v) {
            this.Set (v);
        }

        public static implicit operator LongData (long v) {
            return new LongData (v);
        }
    }
}