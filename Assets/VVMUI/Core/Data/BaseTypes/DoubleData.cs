namespace VVMUI.Core.Data
{
    [System.Serializable]
    public sealed class DoubleData : BaseData<double>
    {
        public DoubleData()
        {

        }

        public DoubleData(double v)
        {
            this.Set(v);
        }

        public static implicit operator double(DoubleData d)
        {
            return d.Get();
        }
    }
}