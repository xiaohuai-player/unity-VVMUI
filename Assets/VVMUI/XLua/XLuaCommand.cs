using System;
using UnityEngine;
using XLua;
using VVMUI.Core.Command;

namespace VVMUI.Script.XLua
{
    public static class XLuaCommand
    {
        public static ICommand GenerateCommandWithLuaTable(LuaTable vmTable, LuaTable cmdLua)
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
    }
}