using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace VVMUI.Core.Data
{
    public interface IGetValue
    {
        object Get(object target);
    }

    public class GetterWrapper
    {
        private static Dictionary<PropertyInfo, IGetValue> _propertyGettersCache = new Dictionary<PropertyInfo, IGetValue>();
        private static Dictionary<MethodInfo, IGetValue> _methodGettersCache = new Dictionary<MethodInfo, IGetValue>();

        public static IGetValue CreateMethodGetterWrapper<TTarget, TValue>(MethodInfo methodInfo)
        {
            if (methodInfo == null)
                throw new ArgumentNullException("methodInfo is null");

            if (methodInfo.GetParameters().Length != 0)
                throw new ArgumentNullException("不支持含参读取方法");

            IGetValue getter = null;
            if (_methodGettersCache.TryGetValue(methodInfo, out getter))
            {
                return getter;
            }

            getter = new GetterWrapper<TTarget, TValue>(methodInfo);
            return getter;
        }
    }

    public class GetterWrapper<TTarget, TValue> : IGetValue
    {
        private Func<TTarget, TValue> _getter;

        public GetterWrapper(PropertyInfo propertyInfo)
        {
            if (propertyInfo == null)
                throw new ArgumentNullException("propertyInfo is null");

            if (propertyInfo.CanRead == false)
                throw new NotSupportedException("属性不支持读操作。");

            MethodInfo m = propertyInfo.GetGetMethod(true);
            _getter = (Func<TTarget, TValue>)Delegate.CreateDelegate(typeof(Func<TTarget, TValue>), null, m);
        }

        public GetterWrapper(MethodInfo methodInfo)
        {
            if (methodInfo == null)
                throw new ArgumentNullException("methodInfo is null");

            _getter = (Func<TTarget, TValue>)Delegate.CreateDelegate(typeof(Func<TTarget, TValue>), null, methodInfo);
        }

        public TValue GetValue(TTarget target)
        {
            return _getter(target);
        }

        public object Get(object target)
        {
            return _getter((TTarget)target);
        }
    }

    public interface ISetValue
    {
        void Set(object target, object val);
    }

    public class SetterWrapper
    {
        private static Dictionary<PropertyInfo, ISetValue> _propertySettersCache = new Dictionary<PropertyInfo, ISetValue>();
        private static Dictionary<MethodInfo, ISetValue> _methodSettersCache = new Dictionary<MethodInfo, ISetValue>();

        //TODO 需要去掉动态泛型，改为写死所有支持的类型
        public static ISetValue CreatePropertySetterWrapper(PropertyInfo propertyInfo)
        {
            if (propertyInfo == null)
                throw new ArgumentNullException("propertyInfo is null");
            if (propertyInfo.CanWrite == false)
                throw new NotSupportedException("属性不支持写操作。");

            ISetValue setter = null;
            if (_propertySettersCache.TryGetValue(propertyInfo, out setter))
            {
                return setter;
            }

            MethodInfo mi = propertyInfo.GetSetMethod(true);
            if (mi.GetParameters().Length > 1)
                throw new NotSupportedException("不支持构造索引器属性的委托。");

            Type instanceType = typeof(SetterWrapper<,>).MakeGenericType(propertyInfo.DeclaringType, propertyInfo.PropertyType);
            setter = (ISetValue)Activator.CreateInstance(instanceType, propertyInfo);
            _propertySettersCache[propertyInfo] = setter;
            return setter;
        }

        public static ISetValue CreateMethodSetterWrapper<TTarget, TValue>(MethodInfo methodInfo)
        {
            if (methodInfo == null)
                throw new ArgumentNullException("methodInfo is null");

            if (methodInfo.GetParameters().Length != 1)
                throw new NotSupportedException("方法参数数量必须为 1");

            ISetValue setter = null;
            if (_methodSettersCache.TryGetValue(methodInfo, out setter))
            {
                return setter;
            }

            setter = new SetterWrapper<TTarget, TValue>(methodInfo);
            _methodSettersCache[methodInfo] = setter;
            return setter;
        }
    }

    public class SetterWrapper<TTarget, TValue> : ISetValue
    {
        private Action<TTarget, TValue> _setter;

        public SetterWrapper(PropertyInfo propertyInfo)
        {
            if (propertyInfo == null)
                throw new ArgumentNullException("propertyInfo is null");

            if (propertyInfo.CanWrite == false)
                throw new NotSupportedException("属性不支持写操作。");

            MethodInfo m = propertyInfo.GetSetMethod(true);
            _setter = (Action<TTarget, TValue>)Delegate.CreateDelegate(typeof(Action<TTarget, TValue>), null, m);
        }

        public SetterWrapper(MethodInfo methodInfo)
        {
            if (methodInfo == null)
                throw new ArgumentNullException("methodInfo is null");

            _setter = (Action<TTarget, TValue>)Delegate.CreateDelegate(typeof(Action<TTarget, TValue>), null, methodInfo);
        }

        public void SetValue(TTarget target, TValue val)
        {
            _setter(target, val);
        }

        public void Set(object target, object val)
        {
            _setter((TTarget)target, (TValue)val);
        }
    }
}