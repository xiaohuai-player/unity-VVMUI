using System;

namespace VVMUI.Core.Data {
	public interface IData {
		void InvokeValueChanged ();
		void AddValueChangedListener (Action handler);
		void RemoveValueChangedListener (Action handler);
		Type GetDataType ();
		ISetValue Setter { get; }
		IGetValue Getter { get; }
	}

	public interface IData<T> {
		T Get ();
		void Set (T arg);
	}
}