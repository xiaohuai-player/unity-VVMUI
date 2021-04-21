#if USE_UNI_LUA
using LuaAPI = UniLua.Lua;
using RealStatePtr = UniLua.ILuaState;
using LuaCSFunction = UniLua.CSharpFunctionDelegate;
#else
using LuaAPI = XLua.LuaDLL.Lua;
using RealStatePtr = System.IntPtr;
using LuaCSFunction = XLua.LuaDLL.lua_CSFunction;
#endif

using XLua;
using System.Collections.Generic;


namespace XLua.CSObjectWrap
{
    using Utils = XLua.Utils;
    
    public class VVMUIScriptXLuaXLuaDataTypeWrap
    {
		public static void __Register(RealStatePtr L)
        {
		    ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
		    Utils.BeginObjectRegister(typeof(VVMUI.Script.XLua.XLuaDataType), L, translator, 0, 0, 0, 0);
			Utils.EndObjectRegister(typeof(VVMUI.Script.XLua.XLuaDataType), L, translator, null, null, null, null, null);
			
			Utils.BeginClassRegister(typeof(VVMUI.Script.XLua.XLuaDataType), L, null, 8, 0, 0);

            
            Utils.RegisterObject(L, translator, Utils.CLS_IDX, "Boolean", VVMUI.Script.XLua.XLuaDataType.Boolean);
            
            Utils.RegisterObject(L, translator, Utils.CLS_IDX, "Float", VVMUI.Script.XLua.XLuaDataType.Float);
            
            Utils.RegisterObject(L, translator, Utils.CLS_IDX, "Int", VVMUI.Script.XLua.XLuaDataType.Int);
            
            Utils.RegisterObject(L, translator, Utils.CLS_IDX, "String", VVMUI.Script.XLua.XLuaDataType.String);
            
            Utils.RegisterObject(L, translator, Utils.CLS_IDX, "UserData", VVMUI.Script.XLua.XLuaDataType.UserData);
            
            Utils.RegisterObject(L, translator, Utils.CLS_IDX, "Struct", VVMUI.Script.XLua.XLuaDataType.Struct);
            
            Utils.RegisterObject(L, translator, Utils.CLS_IDX, "List", VVMUI.Script.XLua.XLuaDataType.List);
            

			Utils.RegisterFunc(L, Utils.CLS_IDX, "__CastFrom", __CastFrom);
            
            Utils.EndClassRegister(typeof(VVMUI.Script.XLua.XLuaDataType), L, translator);
        }
		
		[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int __CastFrom(RealStatePtr L)
		{
			ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			LuaTypes lua_type = LuaAPI.lua_type(L, 1);
            if (lua_type == LuaTypes.LUA_TNUMBER)
            {
                translator.PushVVMUIScriptXLuaXLuaDataType(L, (VVMUI.Script.XLua.XLuaDataType)LuaAPI.xlua_tointeger(L, 1));
            }
			
            else if(lua_type == LuaTypes.LUA_TSTRING)
            {

			    if (LuaAPI.xlua_is_eq_str(L, 1, "Boolean"))
                {
                    translator.PushVVMUIScriptXLuaXLuaDataType(L, VVMUI.Script.XLua.XLuaDataType.Boolean);
                }
				else if (LuaAPI.xlua_is_eq_str(L, 1, "Float"))
                {
                    translator.PushVVMUIScriptXLuaXLuaDataType(L, VVMUI.Script.XLua.XLuaDataType.Float);
                }
				else if (LuaAPI.xlua_is_eq_str(L, 1, "Int"))
                {
                    translator.PushVVMUIScriptXLuaXLuaDataType(L, VVMUI.Script.XLua.XLuaDataType.Int);
                }
				else if (LuaAPI.xlua_is_eq_str(L, 1, "String"))
                {
                    translator.PushVVMUIScriptXLuaXLuaDataType(L, VVMUI.Script.XLua.XLuaDataType.String);
                }
				else if (LuaAPI.xlua_is_eq_str(L, 1, "UserData"))
                {
                    translator.PushVVMUIScriptXLuaXLuaDataType(L, VVMUI.Script.XLua.XLuaDataType.UserData);
                }
				else if (LuaAPI.xlua_is_eq_str(L, 1, "Struct"))
                {
                    translator.PushVVMUIScriptXLuaXLuaDataType(L, VVMUI.Script.XLua.XLuaDataType.Struct);
                }
				else if (LuaAPI.xlua_is_eq_str(L, 1, "List"))
                {
                    translator.PushVVMUIScriptXLuaXLuaDataType(L, VVMUI.Script.XLua.XLuaDataType.List);
                }
				else
                {
                    return LuaAPI.luaL_error(L, "invalid string for VVMUI.Script.XLua.XLuaDataType!");
                }

            }
			
            else
            {
                return LuaAPI.luaL_error(L, "invalid lua type for VVMUI.Script.XLua.XLuaDataType! Expect number or string, got + " + lua_type);
            }

            return 1;
		}
	}
    
    public class VVMUIScriptXLuaXLuaCommandTypeWrap
    {
		public static void __Register(RealStatePtr L)
        {
		    ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
		    Utils.BeginObjectRegister(typeof(VVMUI.Script.XLua.XLuaCommandType), L, translator, 0, 0, 0, 0);
			Utils.EndObjectRegister(typeof(VVMUI.Script.XLua.XLuaCommandType), L, translator, null, null, null, null, null);
			
			Utils.BeginClassRegister(typeof(VVMUI.Script.XLua.XLuaCommandType), L, null, 7, 0, 0);

            
            Utils.RegisterObject(L, translator, Utils.CLS_IDX, "Void", VVMUI.Script.XLua.XLuaCommandType.Void);
            
            Utils.RegisterObject(L, translator, Utils.CLS_IDX, "Bool", VVMUI.Script.XLua.XLuaCommandType.Bool);
            
            Utils.RegisterObject(L, translator, Utils.CLS_IDX, "Float", VVMUI.Script.XLua.XLuaCommandType.Float);
            
            Utils.RegisterObject(L, translator, Utils.CLS_IDX, "Int", VVMUI.Script.XLua.XLuaCommandType.Int);
            
            Utils.RegisterObject(L, translator, Utils.CLS_IDX, "String", VVMUI.Script.XLua.XLuaCommandType.String);
            
            Utils.RegisterObject(L, translator, Utils.CLS_IDX, "Vector2", VVMUI.Script.XLua.XLuaCommandType.Vector2);
            

			Utils.RegisterFunc(L, Utils.CLS_IDX, "__CastFrom", __CastFrom);
            
            Utils.EndClassRegister(typeof(VVMUI.Script.XLua.XLuaCommandType), L, translator);
        }
		
		[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int __CastFrom(RealStatePtr L)
		{
			ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			LuaTypes lua_type = LuaAPI.lua_type(L, 1);
            if (lua_type == LuaTypes.LUA_TNUMBER)
            {
                translator.PushVVMUIScriptXLuaXLuaCommandType(L, (VVMUI.Script.XLua.XLuaCommandType)LuaAPI.xlua_tointeger(L, 1));
            }
			
            else if(lua_type == LuaTypes.LUA_TSTRING)
            {

			    if (LuaAPI.xlua_is_eq_str(L, 1, "Void"))
                {
                    translator.PushVVMUIScriptXLuaXLuaCommandType(L, VVMUI.Script.XLua.XLuaCommandType.Void);
                }
				else if (LuaAPI.xlua_is_eq_str(L, 1, "Bool"))
                {
                    translator.PushVVMUIScriptXLuaXLuaCommandType(L, VVMUI.Script.XLua.XLuaCommandType.Bool);
                }
				else if (LuaAPI.xlua_is_eq_str(L, 1, "Float"))
                {
                    translator.PushVVMUIScriptXLuaXLuaCommandType(L, VVMUI.Script.XLua.XLuaCommandType.Float);
                }
				else if (LuaAPI.xlua_is_eq_str(L, 1, "Int"))
                {
                    translator.PushVVMUIScriptXLuaXLuaCommandType(L, VVMUI.Script.XLua.XLuaCommandType.Int);
                }
				else if (LuaAPI.xlua_is_eq_str(L, 1, "String"))
                {
                    translator.PushVVMUIScriptXLuaXLuaCommandType(L, VVMUI.Script.XLua.XLuaCommandType.String);
                }
				else if (LuaAPI.xlua_is_eq_str(L, 1, "Vector2"))
                {
                    translator.PushVVMUIScriptXLuaXLuaCommandType(L, VVMUI.Script.XLua.XLuaCommandType.Vector2);
                }
				else
                {
                    return LuaAPI.luaL_error(L, "invalid string for VVMUI.Script.XLua.XLuaCommandType!");
                }

            }
			
            else
            {
                return LuaAPI.luaL_error(L, "invalid lua type for VVMUI.Script.XLua.XLuaCommandType! Expect number or string, got + " + lua_type);
            }

            return 1;
		}
	}
    
}