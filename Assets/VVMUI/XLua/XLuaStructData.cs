using System;
using UnityEngine;
using XLua;
using VVMUI.Core.Data;

namespace VVMUI.Script.XLua
{
    public static class XLuaStructData
    {
        public static StructData GenerateVMData(LuaTable luaData)
        {
            LuaTable strct = luaData.Get<LuaTable>("__vm_value");
            StructData strctData = new StructData();
            strct.ForEach<string, LuaTable>(delegate (string key, LuaTable el)
            {
                strctData.AddField(key, XLuaData.GenerateDataWithLuaTable(el));
            });
            return strctData;
        }
    }
}