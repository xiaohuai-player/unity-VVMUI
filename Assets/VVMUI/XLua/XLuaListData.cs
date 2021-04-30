using System;
using System.Collections.Generic;
using UnityEngine;
using XLua;
using VVMUI.Core;
using VVMUI.Core.Data;

namespace VVMUI.Script.XLua
{
    public static class XLuaListData
    {
        // 目前仅支持深度为 2 的 list
        private static IListData CreateNestListDataInstanceWithItemValueLuaTable(LuaTable valueData)
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
                    Debugger.LogError("XLuaListData", "can not generate nested list data of depth greater than 2.");
                    return null;
                case XLuaDataType.Struct:
                    return new ListData<ListData<StructData>>();
                default:
                    return null;
            }
            return null;
        }

        private static IListData CreateListDataInstanceWithItemLuaTable(LuaTable itemData)
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

        public static IListData GenerateVMData(LuaTable luaData)
        {
            LuaTable listLua = luaData.Get<LuaTable>("__vm_value");
            LuaTable first = listLua.Get<int, LuaTable>(1);
            IListData listData = CreateListDataInstanceWithItemLuaTable(first);
            listData.DisableValueChangeHandler = true;
            listLua.ForEach<int, LuaTable>(delegate (int index, LuaTable item)
            {
                listData.Add(XLuaData.GenerateDataWithLuaTable(item));
            });
            listData.DisableValueChangeHandler = false;
            listData.InvokeValueChanged();

            luaData.Set<string, Action<LuaTable>>("__vm_list_add", delegate (LuaTable item) {
                listData.AddItem(XLuaData.GenerateDataWithLuaTable(item));
            });

            luaData.Set<string, Action<int, LuaTable>>("__vm_list_insert", delegate (int index, LuaTable item) {
                listData.InsertItem(index - 1, XLuaData.GenerateDataWithLuaTable(item));
            });

            luaData.Set<string, Action>("__vm_list_pop", delegate () {
                listData.RemoveAt(listData.Count - 1);
            });

            luaData.Set<string, Action<int>>("__vm_list_remove", delegate (int index) {
                listData.RemoveAt(index - 1);
            });

            luaData.Set<string, Action>("__vm_list_reorder", delegate () {
                listData.DisableValueChangeHandler = true;
                LuaTable newListLua = luaData.Get<LuaTable>("__vm_value");
                listData.Clear();
                newListLua.ForEach<int, LuaTable>(delegate (int index, LuaTable item)
                {
                    listData.Add(XLuaData.GenerateDataWithLuaTable(item));
                });
                listData.DisableValueChangeHandler = false;
                listData.InvokeValueChanged();
            });

            return listData;
        }
    }
}