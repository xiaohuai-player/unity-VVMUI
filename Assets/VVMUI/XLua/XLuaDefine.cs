using XLua;
using VVMUI.Core.Data;

namespace VVMUI.Script.XLua
{
    public enum XLuaDataType
    {
        // base types
        Boolean,
        Float,
        Int,
        String,
        UserData,

        // complex types
        Struct,
        List
    }

    public enum XLuaCommandType
    {
        Void,
        Bool,
        Float,
        Int,
        String,
        Vector2
    }

    public delegate void XLuaHookHandler(LuaTable vm);
    public delegate bool XLuaCommandCanExecuteHandler(LuaTable vm, object parameter);
    public delegate void XLuaCommandExecuteHandler(LuaTable vm, object parameter);
    public delegate void XLuaCommandExecuteHandler<T>(LuaTable vm, T value, object parameter);
}