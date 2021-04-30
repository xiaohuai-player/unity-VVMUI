using XLua;
using VVMUI.Core.Data;

namespace VVMUI.Script.XLua
{
    public static class XLuaData
    {
        public static IData GenerateDataWithLuaTable(LuaTable luaData)
        {
            XLuaDataType luaDataType = luaData.Get<XLuaDataType>("__vm_type");
            if (luaDataType == XLuaDataType.List)
            {
                return XLuaListData.GenerateVMData(luaData);
            }
            else if (luaDataType == XLuaDataType.Struct)
            {
                return XLuaStructData.GenerateVMData(luaData);
            }
            else
            {
                return XLuaBaseData.GenerateVMData(luaData);
            }
        }
    }
}