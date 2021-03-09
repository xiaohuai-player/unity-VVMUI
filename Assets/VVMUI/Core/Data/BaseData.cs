using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace VVMUI.Core.Data
{
    public static class BaseData
    {
        public static readonly List<Type> SupportDataType = new List<Type>() {
            typeof(bool),
            typeof(double),
            typeof(float),
            typeof(int),
            typeof(long),
            typeof(string),
            typeof(Color),
            typeof(Vector2),
            typeof(Vector3),
            typeof(Rect),
            typeof(Sprite),
            typeof(Texture)
        };
    }

    public abstract class BaseData<T> : IData<T>, IData
    {
        protected BaseData()
        {

        }

        protected BaseData(T value)
        {
            _value = value;
        }

        [SerializeField]
        private T _value;

        private List<DataChangedHandler> _valueChangedHandlers = new List<DataChangedHandler>();

        public void InvokeValueChanged()
        {
            for (int i = 0; i < _valueChangedHandlers.Count; i++)
            {
                _valueChangedHandlers[i].Invoke(this);
            }
        }

        public void AddValueChangedListener(DataChangedHandler handler)
        {
            _valueChangedHandlers.Add(handler);
        }

        public void RemoveValueChangedListener(DataChangedHandler handler)
        {
            _valueChangedHandlers.Remove(handler);
        }

        public Type GetBindDataType()
        {
            return typeof(T);
        }

        public DataType GetDataType()
        {
            return DataType.Base;
        }

        public T Get()
        {
            return _value;
        }

        public void Set(T arg)
        {
            _value = arg;
            InvokeValueChanged();
        }

        public object FastGetValue()
        {
            return Getter.Get(this);
        }

        public void FastSetValue(object value)
        {
            Setter.Set(this, value);
        }

        public void CopyFrom(IData data)
        {
            if (!this.GetType().IsAssignableFrom(data.GetType()))
            {
                Debug.Log("can not copy data with not the same type");
                return;
            }

            BaseData<T> d = (BaseData<T>)data;
            if (d == null)
            {
                Debug.Log("can not copy data with not the same type");
                return;
            }

            this.Set(d.Get());
        }

        private ISetValue _setter;
        private ISetValue Setter
        {
            get
            {
                if (_setter == null)
                {
                    Type dataType = this.GetType();
                    MethodInfo setMethod = dataType.GetMethod("Set");
                    if (setMethod != null)
                    {
                        _setter = SetterWrapper.CreateMethodSetterWrapper<BaseData<T>, T>(setMethod);
                    }
                }
                return _setter;
            }
        }

        private IGetValue _getter;
        private IGetValue Getter
        {
            get
            {
                if (_getter == null)
                {
                    Type dataType = this.GetType();
                    MethodInfo getMethod = dataType.GetMethod("Get");
                    if (getMethod != null)
                    {
                        _getter = GetterWrapper.CreateMethodGetterWrapper<BaseData<T>, T>(getMethod);
                    }
                }
                return _getter;
            }
        }
    }

    [System.Serializable]
    public sealed class BoolData : BaseData<bool>
    {
        public BoolData() : base()
        {
        }

        public BoolData(bool v) : base(v)
        {
        }

        public static implicit operator bool(BoolData d)
        {
            return d.Get();
        }
    }

    [System.Serializable]
    public sealed class ColorData : BaseData<Color>
    {
        public ColorData() : base()
        {
        }

        public ColorData(Color v) : base(v)
        {
        }

        public static implicit operator Color(ColorData d)
        {
            return d.Get();
        }
    }

    [System.Serializable]
    public sealed class DoubleData : BaseData<double>
    {
        public DoubleData() : base()
        {

        }

        public DoubleData(double v) : base(v)
        {
        }

        public static implicit operator double(DoubleData d)
        {
            return d.Get();
        }
    }

    [System.Serializable]
    public sealed class FloatData : BaseData<float>
    {
        public FloatData() : base()
        {
        }

        public FloatData(float v) : base(v)
        {
        }

        public static implicit operator float(FloatData d)
        {
            return d.Get();
        }
    }

    [System.Serializable]
    public sealed class IntData : BaseData<int>
    {
        public IntData() : base()
        {
        }

        public IntData(int v) : base(v)
        {
        }

        public static implicit operator int(IntData d)
        {
            return d.Get();
        }
    }

    [System.Serializable]
    public sealed class LongData : BaseData<long>
    {
        public LongData() : base()
        {
        }

        public LongData(long v) : base(v)
        {
        }

        public static implicit operator long(LongData d)
        {
            return d.Get();
        }
    }

    [System.Serializable]
    public sealed class RectData : BaseData<Rect>
    {
        public RectData() : base()
        {
        }

        public RectData(Rect v) : base(v)
        {
        }

        public static implicit operator Rect(RectData d)
        {
            return d.Get();
        }
    }

    [System.Serializable]
    public sealed class SpriteData : BaseData<Sprite>
    {
        public SpriteData() : base()
        {
        }

        public SpriteData(Sprite v) : base(v)
        {
        }

        public static implicit operator Sprite(SpriteData d)
        {
            return d.Get();
        }
    }

    [System.Serializable]
    public sealed class StringData : BaseData<string>
    {
        public StringData() : base()
        {
        }

        public StringData(string v) : base(v)
        {
        }

        public static implicit operator string(StringData d)
        {
            return d.Get();
        }
    }

    [System.Serializable]
    [Obsolete("using TextureData instead.")]
    public sealed class Texture2DData : BaseData<Texture2D>
    {
    }

    [System.Serializable]
    public sealed class TextureData : BaseData<Texture>
    {
        public TextureData() : base()
        {
        }

        public TextureData(Texture v) : base(v)
        {
        }

        public static implicit operator Texture(TextureData d)
        {
            return d.Get();
        }
    }

    [System.Serializable]
    public sealed class Vector2Data : BaseData<Vector2>
    {
        public Vector2Data() : base()
        {
        }

        public Vector2Data(Vector2 v) : base(v)
        {
        }

        public static implicit operator Vector2(Vector2Data d)
        {
            return d.Get();
        }
    }

    [System.Serializable]
    public sealed class Vector3Data : BaseData<Vector3>
    {
        public Vector3Data() : base()
        {
        }

        public Vector3Data(Vector3 v) : base(v)
        {
        }

        public static implicit operator Vector3(Vector3Data d)
        {
            return d.Get();
        }
    }
}