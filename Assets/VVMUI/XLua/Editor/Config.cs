using UnityEngine;
using System.Collections.Generic;
using System;
using XLua;
using System.Reflection;
using System.Linq;

namespace VVMUI.Script.XLua.Config
{
    public static class VMXLuaConfig
    {
        [LuaCallCSharp]
        public static List<Type> LuaCallCSharp = new List<Type>() {
            // Unity
            typeof(Color),
            typeof(Vector2),
            typeof(Vector3),
            typeof(Rect),
            typeof(Sprite),
            typeof(Texture),

            // VVMUI
            typeof(XLuaDataType),
            typeof(XLuaCommandType)
        };

        [CSharpCallLua]
        public static List<Type> CSharpCallLua = new List<Type>()
        {
            typeof(XLuaHookHandler),
            typeof(XLuaCommandExecuteHandler),
            typeof(XLuaCommandExecuteHandler<bool>),
            typeof(XLuaCommandExecuteHandler<int>),
            typeof(XLuaCommandExecuteHandler<float>),
            typeof(XLuaCommandExecuteHandler<string>),
            typeof(XLuaCommandExecuteHandler<Vector2>),
            typeof(XLuaCommandCanExecuteHandler)
        };

        //黑名单
        [BlackList]
        public static List<List<string>> BlackList = new List<List<string>>()  {
                new List<string>(){"System.Xml.XmlNodeList", "ItemOf"},
                new List<string>(){"UnityEngine.WWW", "movie"},
    #if UNITY_WEBGL
                new List<string>(){"UnityEngine.WWW", "threadPriority"},
    #endif
                new List<string>(){"UnityEngine.Texture2D", "alphaIsTransparency"},
                new List<string>(){"UnityEngine.Security", "GetChainOfTrustValue"},
                new List<string>(){"UnityEngine.CanvasRenderer", "onRequestRebuild"},
                new List<string>(){"UnityEngine.Light", "areaSize"},
                new List<string>(){"UnityEngine.Light", "lightmapBakeType"},
                new List<string>(){"UnityEngine.WWW", "MovieTexture"},
                new List<string>(){"UnityEngine.WWW", "GetMovieTexture"},
                new List<string>(){"UnityEngine.AnimatorOverrideController", "PerformOverrideClipListCleanup"},
    #if !UNITY_WEBPLAYER
                new List<string>(){"UnityEngine.Application", "ExternalEval"},
    #endif
                new List<string>(){"UnityEngine.GameObject", "networkView"}, //4.6.2 not support
                new List<string>(){"UnityEngine.Component", "networkView"},  //4.6.2 not support
                new List<string>(){"System.IO.FileInfo", "GetAccessControl", "System.Security.AccessControl.AccessControlSections"},
                new List<string>(){"System.IO.FileInfo", "SetAccessControl", "System.Security.AccessControl.FileSecurity"},
                new List<string>(){"System.IO.DirectoryInfo", "GetAccessControl", "System.Security.AccessControl.AccessControlSections"},
                new List<string>(){"System.IO.DirectoryInfo", "SetAccessControl", "System.Security.AccessControl.DirectorySecurity"},
                new List<string>(){"System.IO.DirectoryInfo", "CreateSubdirectory", "System.String", "System.Security.AccessControl.DirectorySecurity"},
                new List<string>(){"System.IO.DirectoryInfo", "Create", "System.Security.AccessControl.DirectorySecurity"},
                new List<string>(){"UnityEngine.MonoBehaviour", "runInEditMode"},
            };

#if UNITY_2018_1_OR_NEWER
        [BlackList]
        public static Func<MemberInfo, bool> MethodFilter = (memberInfo) =>
        {
            if (memberInfo.DeclaringType.IsGenericType && memberInfo.DeclaringType.GetGenericTypeDefinition() == typeof(Dictionary<,>))
            {
                if (memberInfo.MemberType == MemberTypes.Constructor)
                {
                    ConstructorInfo constructorInfo = memberInfo as ConstructorInfo;
                    var parameterInfos = constructorInfo.GetParameters();
                    if (parameterInfos.Length > 0)
                    {
                        if (typeof(System.Collections.IEnumerable).IsAssignableFrom(parameterInfos[0].ParameterType))
                        {
                            return true;
                        }
                    }
                }
                else if (memberInfo.MemberType == MemberTypes.Method)
                {
                    var methodInfo = memberInfo as MethodInfo;
                    if (methodInfo.Name == "TryAdd" || methodInfo.Name == "Remove" && methodInfo.GetParameters().Length == 2)
                    {
                        return true;
                    }
                }
            }
            return false;
        };
#endif
    }
}