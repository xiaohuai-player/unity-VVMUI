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

        //TODO: 目前仅支持深度为 2 的 list
        private IListData CreateNestListDataInstanceWithItemValueLuaTable(LuaTable valueData)
        {
            XLuaDataType firstDataType = valueData.Get<XLuaDataType>("type");
            switch (firstDataType)
            {
                case XLuaDataType.Boolean:
                    return new ListData<ListData<BoolData>>();
                case XLuaDataType.Float:
                    return new ListData<ListData<FloatData>>();
                case XLuaDataType.Int:
                    return new ListData<ListData<IntData>>();
                case XLuaDataType.String:
                    return new ListData<ListData<StringData>>();
                case XLuaDataType.UserData:
                    object obj = valueData.Get<object>("value");
                    Type objType = obj.GetType();
                    if (typeof(Enum).IsAssignableFrom(objType))
                    {
                        return new ListData<ListData<EnumData>>();
                    }
                    else if (typeof(Color).IsAssignableFrom(objType))
                    {
                        return new ListData<ListData<ColorData>>();
                    }
                    else if (typeof(Vector2).IsAssignableFrom(objType))
                    {
                        return new ListData<ListData<Vector2Data>>();
                    }
                    else if (typeof(Vector3).IsAssignableFrom(objType))
                    {
                        return new ListData<ListData<Vector3Data>>();
                    }
                    else if (typeof(Rect).IsAssignableFrom(objType))
                    {
                        return new ListData<ListData<RectData>>();
                    }
                    else if (typeof(Sprite).IsAssignableFrom(objType))
                    {
                        return new ListData<ListData<SpriteData>>();
                    }
                    else if (typeof(Texture).IsAssignableFrom(objType))
                    {
                        return new ListData<ListData<TextureData>>();
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
            XLuaDataType firstDataType = itemData.Get<XLuaDataType>("type");
            switch (firstDataType)
            {
                case XLuaDataType.Boolean:
                    return new ListData<BoolData>();
                case XLuaDataType.Float:
                    return new ListData<FloatData>();
                case XLuaDataType.Int:
                    return new ListData<IntData>();
                case XLuaDataType.String:
                    return new ListData<StringData>();
                case XLuaDataType.UserData:
                    object obj = itemData.Get<object>("value");
                    Type objType = obj.GetType();
                    if (typeof(Enum).IsAssignableFrom(objType))
                    {
                        return new ListData<EnumData>();
                    }
                    else if (typeof(Color).IsAssignableFrom(objType))
                    {
                        return new ListData<ColorData>();
                    }
                    else if (typeof(Vector2).IsAssignableFrom(objType))
                    {
                        return new ListData<Vector2Data>();
                    }
                    else if (typeof(Vector3).IsAssignableFrom(objType))
                    {
                        return new ListData<Vector3Data>();
                    }
                    else if (typeof(Rect).IsAssignableFrom(objType))
                    {
                        return new ListData<RectData>();
                    }
                    else if (typeof(Sprite).IsAssignableFrom(objType))
                    {
                        return new ListData<SpriteData>();
                    }
                    else if (typeof(Texture).IsAssignableFrom(objType))
                    {
                        return new ListData<TextureData>();
                    }
                    break;
                case XLuaDataType.List:
                    LuaTable list = itemData.Get<LuaTable>("value");
                    LuaTable first = list.Get<int, LuaTable>(1);
                    return CreateNestListDataInstanceWithItemValueLuaTable(first);
                case XLuaDataType.Struct:
                    return new ListData<StructData>();
                default:
                    return null;
            }
            return null;
        }

        private IData GenerateDataWithLuaTable(LuaTable data)
        {
            XLuaDataType dataType = data.Get<XLuaDataType>("type");
            switch (dataType)
            {
                case XLuaDataType.Boolean:
                    return new BoolData(data.Get<bool>("value"));
                case XLuaDataType.Float:
                    return new FloatData(data.Get<float>("value"));
                case XLuaDataType.Int:
                    return new IntData(data.Get<int>("value"));
                case XLuaDataType.String:
                    return new StringData(data.Get<string>("value"));
                case XLuaDataType.UserData:
                    object obj = data.Get<object>("value");
                    Type objType = obj.GetType();
                    if (typeof(Enum).IsAssignableFrom(objType))
                    {
                        return new EnumData((Enum)obj);
                    }
                    else if (typeof(Color).IsAssignableFrom(objType))
                    {
                        return new ColorData((Color)obj);
                    }
                    else if (typeof(Vector2).IsAssignableFrom(objType))
                    {
                        return new Vector2Data((Vector2)obj);
                    }
                    else if (typeof(Vector3).IsAssignableFrom(objType))
                    {
                        return new Vector3Data((Vector3)obj);
                    }
                    else if (typeof(Rect).IsAssignableFrom(objType))
                    {
                        return new RectData((Rect)obj);
                    }
                    else if (typeof(Sprite).IsAssignableFrom(objType))
                    {
                        return new SpriteData((Sprite)obj);
                    }
                    else if (typeof(Texture).IsAssignableFrom(objType))
                    {
                        return new TextureData((Texture)obj);
                    }
                    break;
                case XLuaDataType.List:
                    LuaTable list = data.Get<LuaTable>("value");
                    LuaTable first = list.Get<int, LuaTable>(1);
                    IListData listData = CreateListDataInstanceWithItemLuaTable(first);
                    list.ForEach<int, LuaTable>(delegate (int index, LuaTable item)
                    {
                        listData.AddItem(GenerateDataWithLuaTable(item));
                    });
                    return listData;
                case XLuaDataType.Struct:
                    LuaTable strct = data.Get<LuaTable>("value");
                    StructData strctData = new StructData();
                    strct.ForEach<string, LuaTable>(delegate (string key, LuaTable el)
                    {
                        strctData.AddField(key, GenerateDataWithLuaTable(el));
                    });
                    return strctData;
                default:
                    return null;
            }
            return null;
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
                    this.AddData(k, GenerateDataWithLuaTable(data));
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
            base.EditorCollect();

            Env = null;
            Initialized = false;
            Init(new LuaEnv());
            ExecuteLuaScript();
            Collect();
        }
    }
}