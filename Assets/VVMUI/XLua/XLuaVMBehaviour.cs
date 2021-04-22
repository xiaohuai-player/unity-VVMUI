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

        protected override void BeforeAwake()
        {
            base.BeforeAwake();

            if (Env == null)
            {
                Init(new LuaEnv());
            }

            object[] results = Env.DoString("return require '" + LuaPath + "'");
            if (results == null || results.Length <= 0)
            {
                return;
            }

            vmTable = (LuaTable)results[0];
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

        public override void Collect()
        {
            if (vmData != null)
            {
                IEnumerable<string> keys = vmData.GetKeys<string>();
                foreach (string k in keys)
                {
                    LuaTable data = vmData.Get<LuaTable>(k);
                    XLuaDataType dataType = data.Get<XLuaDataType>("type");
                    switch (dataType)
                    {
                        case XLuaDataType.Boolean:
                            this.AddData(k, new BoolData(data.Get<bool>("value")));
                            break;
                        case XLuaDataType.Float:
                            this.AddData(k, new FloatData(data.Get<float>("value")));
                            break;
                        case XLuaDataType.Int:
                            this.AddData(k, new IntData(data.Get<int>("value")));
                            break;
                        case XLuaDataType.String:
                            this.AddData(k, new StringData(data.Get<string>("value")));
                            break;
                    }
                }
            }

            if (vmCommand != null)
            {
                IEnumerable<string> keys = vmCommand.GetKeys<string>();
                foreach (string k in keys)
                {
                    LuaTable command = vmCommand.Get<LuaTable>(k);
                    XLuaCommandType commandType = command.Get<XLuaCommandType>("type");
                    XLuaCommandCanExecuteHandler commandCanExecute = command.Get<XLuaCommandCanExecuteHandler>("can_execute");

                    switch (commandType)
                    {
                        case XLuaCommandType.Void:
                            XLuaCommandExecuteHandler commandExecute = command.Get<XLuaCommandExecuteHandler>("execute");
                            this.AddCommand(k, new VoidCommand(
                                delegate(object parameter)
                                {
                                    if (commandCanExecute == null)
                                    {
                                        return true;
                                    }
                                    else
                                    {
                                        return commandCanExecute.Invoke(vmTable, parameter);
                                    }
                                },
                                delegate(object parameter)
                                {
                                    if (commandExecute != null)
                                    {
                                        commandExecute.Invoke(vmTable, parameter);
                                    }
                                }
                            ));
                            break;
                        case XLuaCommandType.Bool:
                            XLuaCommandExecuteHandler<bool> boolCommandExecute = command.Get<XLuaCommandExecuteHandler<bool>>("execute");
                            this.AddCommand(k, new BoolCommand(
                                delegate(object parameter)
                                {
                                    if (commandCanExecute == null)
                                    {
                                        return true;
                                    }
                                    else
                                    {
                                        return commandCanExecute.Invoke(vmTable, parameter);
                                    }
                                },
                                delegate(bool v, object parameter)
                                {
                                    if (boolCommandExecute != null)
                                    {
                                        boolCommandExecute.Invoke(vmTable, v, parameter);
                                    }
                                }
                            ));
                            break;
                        case XLuaCommandType.Float:
                            XLuaCommandExecuteHandler<float> floatCommandExecute = command.Get<XLuaCommandExecuteHandler<float>>("execute");
                            this.AddCommand(k, new FloatCommand(
                                delegate (object parameter)
                                {
                                    if (commandCanExecute == null)
                                    {
                                        return true;
                                    }
                                    else
                                    {
                                        return commandCanExecute.Invoke(vmTable, parameter);
                                    }
                                },
                                delegate (float v, object parameter)
                                {
                                    if (floatCommandExecute != null)
                                    {
                                        floatCommandExecute.Invoke(vmTable, v, parameter);
                                    }
                                }
                            ));
                            break;
                        case XLuaCommandType.Int:
                            XLuaCommandExecuteHandler<int> intCommandExecute = command.Get<XLuaCommandExecuteHandler<int>>("execute");
                            this.AddCommand(k, new IntCommand(
                                delegate (object parameter)
                                {
                                    if (commandCanExecute == null)
                                    {
                                        return true;
                                    }
                                    else
                                    {
                                        return commandCanExecute.Invoke(vmTable, parameter);
                                    }
                                },
                                delegate (int v, object parameter)
                                {
                                    if (intCommandExecute != null)
                                    {
                                        intCommandExecute.Invoke(vmTable, v, parameter);
                                    }
                                }
                            ));
                            break;
                        case XLuaCommandType.String:
                            XLuaCommandExecuteHandler<string> stringCommandExecute = command.Get<XLuaCommandExecuteHandler<string>>("execute");
                            this.AddCommand(k, new StringCommand(
                                delegate (object parameter)
                                {
                                    if (commandCanExecute == null)
                                    {
                                        return true;
                                    }
                                    else
                                    {
                                        return commandCanExecute.Invoke(vmTable, parameter);
                                    }
                                },
                                delegate (string v, object parameter)
                                {
                                    if (stringCommandExecute != null)
                                    {
                                        stringCommandExecute.Invoke(vmTable, v, parameter);
                                    }
                                }
                            ));
                            break;
                        case XLuaCommandType.Vector2:
                            XLuaCommandExecuteHandler<Vector2> vector2CommandExecute = command.Get<XLuaCommandExecuteHandler<Vector2>>("execute");
                            this.AddCommand(k, new Vector2Command(
                                delegate (object parameter)
                                {
                                    if (commandCanExecute == null)
                                    {
                                        return true;
                                    }
                                    else
                                    {
                                        return commandCanExecute.Invoke(vmTable, parameter);
                                    }
                                },
                                delegate (Vector2 v, object parameter)
                                {
                                    if (vector2CommandExecute != null)
                                    {
                                        vector2CommandExecute.Invoke(vmTable, v, parameter);
                                    }
                                }
                            ));
                            break;
                    }
                }
            }

            if (this.BindRoot == null)
            {
                this.BindRoot = this.gameObject;
            }
            this.BindRoot.GetComponentsInChildren<AbstractDataBinder>(true, allDataBinders);
            this.BindRoot.GetComponentsInChildren<AbstractCommandBinder>(true, allCommandBinders);
        }
    }
}