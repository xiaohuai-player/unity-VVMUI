using System;
using System.Reflection;

namespace VVMUI.Core.Data {
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