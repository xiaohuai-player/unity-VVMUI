using System;
using System.Reflection;

namespace VVMUI.Core.Data {
    public interface IGetValue {
        object Get (object target);
    }

    public class GetterWrapper {
        public static IGetValue CreatePropertyGetterWrapper (PropertyInfo propertyInfo) {
            if (propertyInfo == null)
                throw new ArgumentNullException ("propertyInfo is null");
            if (propertyInfo.CanRead == false)
                throw new NotSupportedException ("属性不支持读操作。");

            MethodInfo mi = propertyInfo.GetGetMethod (true);

            if (mi.GetParameters ().Length > 1)
                throw new NotSupportedException ("不支持构造索引器属性的委托。");

            Type instanceType = typeof (GetterWrapper<,>).MakeGenericType (propertyInfo.DeclaringType, propertyInfo.PropertyType);
            return (IGetValue) Activator.CreateInstance (instanceType, propertyInfo);
        }

        public static IGetValue CreateMethodGetterWrapper (MethodInfo methodInfo) {
            if (methodInfo == null)
                throw new ArgumentNullException ("methodInfo is null");

            if (methodInfo.GetParameters ().Length != 0)
                throw new ArgumentNullException ("不支持含参读取方法");

            // TODO 检查 methodInfo.ReturnParameter

            Type instanceType = typeof (GetterWrapper<,>).MakeGenericType (methodInfo.DeclaringType, methodInfo.ReturnType);
            return (IGetValue) Activator.CreateInstance (instanceType, methodInfo);
        }
    }

    public class GetterWrapper<TTarget, TValue> : IGetValue {
        private Func<TTarget, TValue> _getter;

        public GetterWrapper (PropertyInfo propertyInfo) {
            if (propertyInfo == null)
                throw new ArgumentNullException ("propertyInfo is null");

            if (propertyInfo.CanRead == false)
                throw new NotSupportedException ("属性不支持读操作。");

            MethodInfo m = propertyInfo.GetGetMethod (true);
            _getter = (Func<TTarget, TValue>) Delegate.CreateDelegate (typeof (Func<TTarget, TValue>), null, m);
        }

        public GetterWrapper (MethodInfo methodInfo) {
            if (methodInfo == null)
                throw new ArgumentNullException ("methodInfo is null");

            _getter = (Func<TTarget, TValue>) Delegate.CreateDelegate (typeof (Func<TTarget, TValue>), null, methodInfo);
        }

        public TValue GetValue (TTarget target) {
            return _getter (target);
        }

        public object Get (object target) {
            return _getter ((TTarget) target);
        }
    }


    public interface ISetValue {
        void Set (object target, object val);
    }

    public class SetterWrapper {
        public static ISetValue CreatePropertySetterWrapper (PropertyInfo propertyInfo) {
            if (propertyInfo == null)
                throw new ArgumentNullException ("propertyInfo is null");
            if (propertyInfo.CanWrite == false)
                throw new NotSupportedException ("属性不支持写操作。");

            MethodInfo mi = propertyInfo.GetSetMethod (true);

            if (mi.GetParameters ().Length > 1)
                throw new NotSupportedException ("不支持构造索引器属性的委托。");

            Type instanceType = typeof (SetterWrapper<,>).MakeGenericType (propertyInfo.DeclaringType, propertyInfo.PropertyType);
            return (ISetValue) Activator.CreateInstance (instanceType, propertyInfo);
        }

        public static ISetValue CreateMethodSetterWrapper (MethodInfo methodInfo) {
            if (methodInfo == null)
                throw new ArgumentNullException ("methodInfo is null");

            if (methodInfo.GetParameters ().Length != 1)
                throw new NotSupportedException ("方法参数数量必须为 1");

            Type instanceType = typeof (SetterWrapper<,>).MakeGenericType (methodInfo.DeclaringType, methodInfo.GetParameters () [0].ParameterType);
            return (ISetValue) Activator.CreateInstance (instanceType, methodInfo);
        }
    }

    public class SetterWrapper<TTarget, TValue> : ISetValue {
        private Action<TTarget, TValue> _setter;

        public SetterWrapper (PropertyInfo propertyInfo) {
            if (propertyInfo == null)
                throw new ArgumentNullException ("propertyInfo is null");

            if (propertyInfo.CanWrite == false)
                throw new NotSupportedException ("属性不支持写操作。");

            MethodInfo m = propertyInfo.GetSetMethod (true);
            _setter = (Action<TTarget, TValue>) Delegate.CreateDelegate (typeof (Action<TTarget, TValue>), null, m);
        }

        public SetterWrapper (MethodInfo methodInfo) {
            if (methodInfo == null)
                throw new ArgumentNullException ("methodInfo is null");

            _setter = (Action<TTarget, TValue>) Delegate.CreateDelegate (typeof (Action<TTarget, TValue>), null, methodInfo);
        }

        public void SetValue (TTarget target, TValue val) {
            _setter (target, val);
        }

        public void Set (object target, object val) {
            _setter ((TTarget) target, (TValue) val);
        }
    }
}