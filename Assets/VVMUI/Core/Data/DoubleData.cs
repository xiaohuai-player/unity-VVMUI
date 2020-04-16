namespace VVMUI.Core.Data {
	[System.Serializable]
	public sealed class DoubleData : BaseData<float> {
		public DoubleData () { 
			
		}

		public DoubleData (float v) {
			this.Set (v);
		}

		public static implicit operator double (DoubleData d) {
			return d.Get ();
		}
	}
}