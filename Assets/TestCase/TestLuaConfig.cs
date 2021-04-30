using UnityEngine;
using System.Collections.Generic;
using System;
using XLua;
using System.Reflection;
using System.Linq;

public static class TestLuaConfig
{
    [LuaCallCSharp]
    public static List<Type> LuaCallCSharp = new List<Type>() {
        // Unity
        typeof(TextAnchor)
    };
}