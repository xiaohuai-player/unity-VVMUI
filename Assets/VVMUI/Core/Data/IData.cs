using System;

namespace VVMUI.Core.Data {
	public interface IData {
		event Action ValueChanged;
		void InvokeValueChanged ();
	}

	public interface IData<T> {
		T Get ();
		void Set (T arg);
	}
}