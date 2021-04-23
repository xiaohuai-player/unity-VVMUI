using System;
using System.Reflection;

namespace VVMUI.Core.Data
{
    public enum DataType
    {
        Base,
        List,
        Struct,
        Dictionary
    }

    public delegate void DataChangedHandler(IData source);

    public interface IData
    {
        void InvokeValueChanged();
        void AddValueChangedListener(DataChangedHandler handler);
        void RemoveValueChangedListener(DataChangedHandler handler);
        Type GetBindDataType();
        DataType GetDataType();
        void CopyFrom(IData data);
    }
}