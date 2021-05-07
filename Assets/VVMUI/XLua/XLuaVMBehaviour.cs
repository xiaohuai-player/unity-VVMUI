using System;
using System.Collections.Generic;
using UnityEngine;
using VVMUI.Core;
using VVMUI.Core.Binder;
using VVMUI.Core.Command;
using VVMUI.Core.Data;
using XLua;

namespace VVMUI.Script.XLua
{
    public class XLuaVMBehaviour : VMBehaviour
    {
        private static LuaEnv Env;
        private static bool Initialized;
        public static void Init(LuaEnv env)
        {
            if (Initialized)
            {
                return;
            }

            if (env == null)
            {
                Debugger.LogError("VVMUI.XLua", "Init with a null LuaEnv.");
                return;
            }

            Env = env;
            Env.DoString("require 'vm'");

            Initialized = true;
        }

        public string LuaPath;

        private LuaTable vmTable;
        private LuaTable vmData;
        private LuaTable vmCommand;
        private LuaTable vmHook;

        private XLuaHookHandler beforeAwake;
        private XLuaHookHandler afterAwake;
        private XLuaHookHandler beforeActive;
        private XLuaHookHandler afterActive;
        private XLuaHookHandler beforeDeactive;
        private XLuaHookHandler afterDeactive;
        private XLuaHookHandler beforeDestroy;
        private XLuaHookHandler afterDestroy;

        private void ExecuteLuaScript()
        {
            object[] results = Env.DoString("return require '" + LuaPath + "'");
            if (results == null || results.Length <= 0)
            {
                return;
            }

            vmTable = (LuaTable)results[0];
            vmTable.Set<string, XLuaVMBehaviour>("behaviour", this);
            vmData = vmTable.Get<LuaTable>("__vm_data");
            vmCommand = vmTable.Get<LuaTable>("__vm_command");
            vmHook = vmTable.Get<LuaTable>("__vm_hook");

            if (vmHook != null)
            {
                beforeAwake = vmHook.Get<XLuaHookHandler>("before_awake");
                afterAwake = vmHook.Get<XLuaHookHandler>("after_awake");
                beforeActive = vmHook.Get<XLuaHookHandler>("before_active");
                afterActive = vmHook.Get<XLuaHookHandler>("after_active");
                beforeDeactive = vmHook.Get<XLuaHookHandler>("before_deactive");
                afterDeactive = vmHook.Get<XLuaHookHandler>("after_deactive");
                beforeDestroy = vmHook.Get<XLuaHookHandler>("before_destroy");
                afterDestroy = vmHook.Get<XLuaHookHandler>("after_destroy");
            }
        }

        protected override void BeforeAwake()
        {
            base.BeforeAwake();

            if (!Initialized)
            {
                Init(new LuaEnv());
            }

            ExecuteLuaScript();

            if (beforeAwake != null)
            {
                beforeAwake.Invoke(vmTable);
            }
        }

        protected override void AfterAwake()
        {
            base.AfterAwake();

            if (afterAwake != null)
            {
                afterAwake.Invoke(this.vmTable);
            }
        }

        protected override void BeforeActive()
        {
            base.BeforeActive();

            if (beforeActive != null)
            {
                beforeActive.Invoke(this.vmTable);
            }
        }

        protected override void AfterActive()
        {
            base.AfterActive();

            if (afterActive != null)
            {
                afterActive.Invoke(this.vmTable);
            }
        }

        protected override void BeforeDeactive()
        {
            base.BeforeDeactive();

            if (beforeDeactive != null)
            {
                beforeDeactive.Invoke(this.vmTable);
            }
        }

        protected override void AfterDeactive()
        {
            base.AfterDeactive();

            if (afterDeactive != null)
            {
                afterDeactive.Invoke(this.vmTable);
            }
        }

        protected override void BeforeDestroy()
        {
            base.BeforeDestroy();

            if (beforeDestroy != null)
            {
                beforeDestroy.Invoke(this.vmTable);
            }
        }

        protected override void AfterDestroy()
        {
            base.AfterDestroy();

            if (afterDestroy != null)
            {
                afterDestroy.Invoke(this.vmTable);
            }
        }

        protected override void Collect()
        {
            if (vmData != null)
            {
                IEnumerable<string> keys = vmData.GetKeys<string>();
                foreach (string k in keys)
                {
                    LuaTable data = vmData.Get<LuaTable>(k);
                    this.AddData(k, XLuaData.GenerateDataWithLuaTable(data));
                }
            }

            if (vmCommand != null)
            {
                IEnumerable<string> keys = vmCommand.GetKeys<string>();
                foreach (string k in keys)
                {
                    LuaTable command = vmCommand.Get<LuaTable>(k);
                    this.AddCommand(k, XLuaCommand.GenerateCommandWithLuaTable(vmTable, command));
                }
            }

            if (this.BindRoot == null)
            {
                this.BindRoot = this.gameObject;
            }
            this.BindRoot.GetComponentsInChildren<AbstractDataBinder>(true, allDataBinders);
            this.BindRoot.GetComponentsInChildren<AbstractCommandBinder>(true, allCommandBinders);
        }

        public override void EditorCollect()
        {
            Env = null;
            Initialized = false;
            Init(new LuaEnv());
            ExecuteLuaScript();
            Collect();
        }
    }
}