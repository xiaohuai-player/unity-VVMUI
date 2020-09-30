using System;

namespace VVMUI.Core.Data
{
    public delegate void DataChangedHandler(IData source);

    public interface IData
    {
        void InvokeValueChanged();
        void AddValueChangedListener(DataChangedHandler handler);
        void RemoveValueChangedListener(DataChangedHandler handler);
        Type GetDataType();
        ISetValue Setter { get; }
        IGetValue Getter { get; }
        void CopyFrom(IData data);
    }

    public interface IData<T>
    {
        T Get();
        void Set(T arg);
    }
}