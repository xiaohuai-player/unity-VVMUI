using System;
using VVMUI.Core.Data;
using XLua;

namespace VVMUI.Script.XLua
{
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