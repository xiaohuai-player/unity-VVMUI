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

        public override void Collect()
        {
            if (Env == null)
            {
                Init(new LuaEnv());
            }

            object[] results = Env.DoString("return require '" + LuaPath + "'");
            if (results == null || results.Length <= 0)
            {
                return;
            }

            LuaTable vm = (LuaTable)results[0];
            LuaTable vmData = vm.Get<LuaTable>("__vm_data");
            LuaTable vmCommand = vm.Get<LuaTable>("__vm_command");
            LuaTable vmHook = vm.Get<LuaTable>("__vm_hook");

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
                                        return commandCanExecute.Invoke(vm, parameter);
                                    }
                                },
                                delegate(object parameter)
                                {
                                    if (commandExecute != null)
                                    {
                                        commandExecute.Invoke(vm, parameter);
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
                                        return commandCanExecute.Invoke(vm, parameter);
                                    }
                                },
                                delegate(bool v, object parameter)
                                {
                                    if (boolCommandExecute != null)
                                    {
                                        boolCommandExecute.Invoke(vm, v, parameter);
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
                                        return commandCanExecute.Invoke(vm, parameter);
                                    }
                                },
                                delegate (float v, object parameter)
                                {
                                    if (floatCommandExecute != null)
                                    {
                                        floatCommandExecute.Invoke(vm, v, parameter);
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
                                        return commandCanExecute.Invoke(vm, parameter);
                                    }
                                },
                                delegate (int v, object parameter)
                                {
                                    if (intCommandExecute != null)
                                    {
                                        intCommandExecute.Invoke(vm, v, parameter);
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
                                        return commandCanExecute.Invoke(vm, parameter);
                                    }
                                },
                                delegate (string v, object parameter)
                                {
                                    if (stringCommandExecute != null)
                                    {
                                        stringCommandExecute.Invoke(vm, v, parameter);
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
                                        return commandCanExecute.Invoke(vm, parameter);
                                    }
                                },
                                delegate (Vector2 v, object parameter)
                                {
                                    if (vector2CommandExecute != null)
                                    {
                                        vector2CommandExecute.Invoke(vm, v, parameter);
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