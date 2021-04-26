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

        // 目前仅支持深度为 2 的 list
        private IListData CreateNestListDataInstanceWithItemValueLuaTable(LuaTable valueData)
        {
            XLuaDataType firstDataType = valueData.Get<XLuaDataType>("__vm_type");
            switch (firstDataType)
            {
                case XLuaDataType.Boolean:
                    return new ListData<ListData<BaseData<bool>>>();
                case XLuaDataType.Float:
                    return new ListData<ListData<BaseData<float>>>();
                case XLuaDataType.Int:
                    return new ListData<ListData<BaseData<int>>>();
                case XLuaDataType.String:
                    return new ListData<ListData<BaseData<string>>>();
                case XLuaDataType.UserData:
                    object obj = valueData.Get<object>("__vm_value");
                    Type objType = obj.GetType();
                    if (typeof(Enum).IsAssignableFrom(objType))
                    {
                        return new ListData<ListData<BaseData<Enum>>>();
                    }
                    else if (typeof(Color).IsAssignableFrom(objType))
                    {
                        return new ListData<ListData<BaseData<Color>>>();
                    }
                    else if (typeof(Vector2).IsAssignableFrom(objType))
                    {
                        return new ListData<ListData<BaseData<Vector2>>>();
                    }
                    else if (typeof(Vector3).IsAssignableFrom(objType))
                    {
                        return new ListData<ListData<BaseData<Vector3>>>();
                    }
                    else if (typeof(Rect).IsAssignableFrom(objType))
                    {
                        return new ListData<ListData<BaseData<Rect>>>();
                    }
                    else if (typeof(Sprite).IsAssignableFrom(objType))
                    {
                        return new ListData<ListData<BaseData<Sprite>>>();
                    }
                    else if (typeof(Texture).IsAssignableFrom(objType))
                    {
                        return new ListData<ListData<BaseData<Texture>>>();
                    }
                    break;
                case XLuaDataType.List:
                    Debugger.LogError("XLuaVMBehaviour", "can not generate nested list data of depth greater than 2.");
                    return null;
                case XLuaDataType.Struct:
                    return new ListData<ListData<StructData>>();
                default:
                    return null;
            }
            return null;
        }

        private IListData CreateListDataInstanceWithItemLuaTable(LuaTable itemData)
        {
            XLuaDataType firstDataType = itemData.Get<XLuaDataType>("__vm_type");
            switch (firstDataType)
            {
                case XLuaDataType.Boolean:
                    return new ListData<BaseData<bool>>();
                case XLuaDataType.Float:
                    return new ListData<BaseData<float>>();
                case XLuaDataType.Int:
                    return new ListData<BaseData<int>>();
                case XLuaDataType.String:
                    return new ListData<BaseData<string>>();
                case XLuaDataType.UserData:
                    object obj = itemData.Get<object>("__vm_value");
                    Type objType = obj.GetType();
                    if (typeof(Enum).IsAssignableFrom(objType))
                    {
                        return new ListData<BaseData<Enum>>();
                    }
                    else if (typeof(Color).IsAssignableFrom(objType))
                    {
                        return new ListData<BaseData<Color>>();
                    }
                    else if (typeof(Vector2).IsAssignableFrom(objType))
                    {
                        return new ListData<BaseData<Vector2>>();
                    }
                    else if (typeof(Vector3).IsAssignableFrom(objType))
                    {
                        return new ListData<BaseData<Vector3>>();
                    }
                    else if (typeof(Rect).IsAssignableFrom(objType))
                    {
                        return new ListData<BaseData<Rect>>();
                    }
                    else if (typeof(Sprite).IsAssignableFrom(objType))
                    {
                        return new ListData<BaseData<Sprite>>();
                    }
                    else if (typeof(Texture).IsAssignableFrom(objType))
                    {
                        return new ListData<BaseData<Texture>>();
                    }
                    break;
                case XLuaDataType.List:
                    LuaTable list = itemData.Get<LuaTable>("__vm_value");
                    LuaTable first = list.Get<int, LuaTable>(1);
                    return CreateNestListDataInstanceWithItemValueLuaTable(first);
                case XLuaDataType.Struct:
                    return new ListData<StructData>();
                default:
                    return null;
            }
            return null;
        }

        private IData GenerateDataWithLuaTable(string k, LuaTable luaData)
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
                case XLuaDataType.List:
                    LuaTable list = luaData.Get<LuaTable>("__vm_value");
                    LuaTable first = list.Get<int, LuaTable>(1);
                    IListData listData = CreateListDataInstanceWithItemLuaTable(first);
                    list.ForEach<int, LuaTable>(delegate (int index, LuaTable item)
                    {
                        listData.AddItem(GenerateDataWithLuaTable(index.ToString(), item));
                    });
                    return listData;
                case XLuaDataType.Struct:
                    LuaTable strct = luaData.Get<LuaTable>("__vm_value");
                    StructData strctData = new StructData();
                    strct.ForEach<string, LuaTable>(delegate (string key, LuaTable el)
                    {
                        strctData.AddField(key, GenerateDataWithLuaTable(key, el));
                    });
                    return strctData;
                default:
                    return null;
            }
        }

        private ICommand GenerateCommandWithLuaTable(LuaTable cmdLua)
        {
            XLuaCommandCanExecuteHandler commandCanExecute = cmdLua.Get<XLuaCommandCanExecuteHandler>("can_execute");
            Func<object, bool> canExecuteDelegate = delegate (object parameter)
            {
                if (commandCanExecute == null)
                {
                    return true;
                }
                else
                {
                    return commandCanExecute.Invoke(vmTable, parameter);
                }
            };

            ICommand command = null;
            XLuaCommandType commandType = cmdLua.Get<XLuaCommandType>("type");
            switch (commandType)
            {
                case XLuaCommandType.Void:
                    XLuaCommandExecuteHandler commandExecute = cmdLua.Get<XLuaCommandExecuteHandler>("execute");
                    command = new VoidCommand(
                        canExecuteDelegate,
                        delegate (object parameter)
                        {
                            if (commandExecute != null)
                            {
                                commandExecute.Invoke(vmTable, parameter);
                            }
                        }
                    );
                    break;
                case XLuaCommandType.Bool:
                    XLuaCommandExecuteHandler<bool> boolCommandExecute = cmdLua.Get<XLuaCommandExecuteHandler<bool>>("execute");
                    command = new BoolCommand(
                        canExecuteDelegate,
                        delegate (bool v, object parameter)
                        {
                            if (boolCommandExecute != null)
                            {
                                boolCommandExecute.Invoke(vmTable, v, parameter);
                            }
                        }
                    );
                    break;
                case XLuaCommandType.Float:
                    XLuaCommandExecuteHandler<float> floatCommandExecute = cmdLua.Get<XLuaCommandExecuteHandler<float>>("execute");
                    command = new FloatCommand(
                        canExecuteDelegate,
                        delegate (float v, object parameter)
                        {
                            if (floatCommandExecute != null)
                            {
                                floatCommandExecute.Invoke(vmTable, v, parameter);
                            }
                        }
                    );
                    break;
                case XLuaCommandType.Int:
                    XLuaCommandExecuteHandler<int> intCommandExecute = cmdLua.Get<XLuaCommandExecuteHandler<int>>("execute");
                    command = new IntCommand(
                        canExecuteDelegate,
                        delegate (int v, object parameter)
                        {
                            if (intCommandExecute != null)
                            {
                                intCommandExecute.Invoke(vmTable, v, parameter);
                            }
                        }
                    );
                    break;
                case XLuaCommandType.String:
                    XLuaCommandExecuteHandler<string> stringCommandExecute = cmdLua.Get<XLuaCommandExecuteHandler<string>>("execute");
                    command = new StringCommand(
                        canExecuteDelegate,
                        delegate (string v, object parameter)
                        {
                            if (stringCommandExecute != null)
                            {
                                stringCommandExecute.Invoke(vmTable, v, parameter);
                            }
                        }
                    );
                    break;
                case XLuaCommandType.Vector2:
                    XLuaCommandExecuteHandler<Vector2> vector2CommandExecute = cmdLua.Get<XLuaCommandExecuteHandler<Vector2>>("execute");
                    command = new Vector2Command(
                        canExecuteDelegate,
                        delegate (Vector2 v, object parameter)
                        {
                            if (vector2CommandExecute != null)
                            {
                                vector2CommandExecute.Invoke(vmTable, v, parameter);
                            }
                        }
                    );
                    break;
            }

            return command;
        }

        protected override void Collect()
        {
            if (vmData != null)
            {
                IEnumerable<string> keys = vmData.GetKeys<string>();
                foreach (string k in keys)
                {
                    LuaTable data = vmData.Get<LuaTable>(k);
                    this.AddData(k, GenerateDataWithLuaTable(k, data));
                }
            }

            if (vmCommand != null)
            {
                IEnumerable<string> keys = vmCommand.GetKeys<string>();
                foreach (string k in keys)
                {
                    LuaTable command = vmCommand.Get<LuaTable>(k);
                    this.AddCommand(k, GenerateCommandWithLuaTable(command));
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