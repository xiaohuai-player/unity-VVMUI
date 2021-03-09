using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using VVMUI.Core;
using VVMUI.Core.Data;
using VVMUI.Core.Command;
using VVMUI.Core.Binder;
using VVMUI.Core.Converter;
using XLua;

namespace VVMUI.Script.XLua
{
    public class XLuaVMBehaviour : VMBehaviour
    {
        private static LuaEnv Env;

        public static void Init(LuaEnv env)
        {
            if (env == null)
            {
                Debugger.LogError("VVMUI.XLua", "Init with a null LuaEnv.");
                return;
            }

            Env = env;
            Env.DoString("require 'vm'");
        }

        public string LuaPath;

        protected override void BeforeAwake()
        {
            base.BeforeAwake();

            // for test
            Init(new LuaEnv());

            if (Env != null)
            {
                Env.DoString("require '" + LuaPath + "'");
            }
        }
    }
}