/**
 * @Author: #AUTHOR#
 * @Date: #CREATIONDATE#
 * @Description: this is description of ReflectionCache
 */

using System;
using System.Collections.Generic;
using System.Reflection;

namespace VVMUI
{

    public class ReflectionCacheData
    {
        private Type type;
        private Type[] genericArguments;
        private Dictionary<string, FieldInfo> _fields = new Dictionary<string, FieldInfo>();
        private Dictionary<string, PropertyInfo> _properties = new Dictionary<string, PropertyInfo>();
        private Dictionary<string, MethodInfo> _methods = new Dictionary<string, MethodInfo>();

        public ReflectionCacheData(Type type)
        {
            this.type = type;
        }

        public Type[] GetGenericArguments()
        {
            if (genericArguments == null)
            {
                genericArguments = this.type.GetGenericArguments();
            }
            return genericArguments;
        }

        public FieldInfo GetField(string name)
        {
            if (!_fields.TryGetValue(name, out FieldInfo field))
            {
                _fields[name] = this.type.GetField(name);
            }
            return _fields[name];
        }

        public PropertyInfo GetProperty(string name)
        {
            if (!_properties.TryGetValue(name, out PropertyInfo property))
            {
                _properties[name] = this.type.GetProperty(name);
            }
            return _properties[name];
        }

        public MethodInfo GetMethod(string name)
        {
            if (!_methods.TryGetValue(name, out MethodInfo method))
            {
                _methods[name] = this.type.GetMethod(name);
            }
            return _methods[name];
        }
    }

    public class ReflectionCache
    {
        private static ReflectionCache _singleton;
        public static ReflectionCache Singleton
        {
            get
            {
                if (_singleton == null)
                {
                    _singleton = new ReflectionCache();
                }
                return _singleton;
            }
        }

        private readonly Dictionary<Type, ReflectionCacheData> _cache = new Dictionary<Type, ReflectionCacheData>();

        public ReflectionCacheData this[Type index]
        {
            get
            {
                if (!_cache.TryGetValue(index, out ReflectionCacheData obj))
                {
                    obj = new ReflectionCacheData(index);
                    _cache[index] = obj;
                }
                return obj;
            }
        }
    }
}