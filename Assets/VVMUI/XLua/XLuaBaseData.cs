using System;
using UnityEngine;
using XLua;
using VVMUI.Core.Data;

namespace VVMUI.Script.XLua
{
    public static class XLuaBaseData
    {
        public static IBaseData GenerateVMData(LuaTable luaData)
        {
            XLuaDataType luaDataType = luaData.Get<XLuaDataType>("__vm_type");
            switch (luaDataType)
            {
                case XLuaDataType.Boolean:
                    return new XLuaBaseData<bool>(luaData).VMData;
                case XLuaDataType.Float:
                    return new XLuaBaseData<float>(luaData).VMData;
                case XLuaDataType.Int:
                    return new XLuaBaseData<int>(luaData).VMData;
                case XLuaDataType.String:
                    return new XLuaBaseData<string>(luaData).VMData;
                case XLuaDataType.UserData:
                    object obj = luaData.Get<object>("__vm_value");
                    Type objType = obj.GetType();
                    if (typeof(Enum).IsAssignableFrom(objType))
                    {
                        return new XLuaBaseData<Enum>(luaData).VMData;
                    }
                    else if (typeof(Color).IsAssignableFrom(objType))
                    {
                        return new XLuaBaseData<Color>(luaData).VMData;
                    }
                    else if (typeof(Vector2).IsAssignableFrom(objType))
                    {
                        return new XLuaBaseData<Vector2>(luaData).VMData;
                    }
                    else if (typeof(Vector3).IsAssignableFrom(objType))
                    {
                        return new XLuaBaseData<Vector3>(luaData).VMData;
                    }
                    else if (typeof(Rect).IsAssignableFrom(objType))
                    {
                        return new XLuaBaseData<Rect>(luaData).VMData;
                    }
                    else if (typeof(Sprite).IsAssignableFrom(objType))
                    {
                        return new XLuaBaseData<Sprite>(luaData).VMData;
                    }
                    else if (typeof(Texture).IsAssignableFrom(objType))
                    {
                        return new XLuaBaseData<Texture>(luaData).VMData;
                    }
                    return null;
                default:
                    return null;
            }
        }
    }

    public class XLuaBaseData<T>
    {
        private LuaTable luaData;

        public readonly BaseData<T> VMData;

        public XLuaBaseData(LuaTable table)
        {
            this.VMData = new BaseData<T>(table.Get<string, T>("__vm_value"));
            this.luaData = table;
            this.luaData.Set<string, Action<T>>("__vm_set", delegate (T value) { this.VMData.Set(value); });
            this.luaData.Set<string, Func<T>>("__vm_get", delegate () { return this.VMData.Get(); });
        }
    }
}