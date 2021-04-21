using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Scripting;
using VVMUI.Core.Binder;
using VVMUI.Core.Command;
using VVMUI.Core.Converter;
using VVMUI.Core.Data;

namespace VVMUI.Core
{
    public class VMBehaviour : UIBehaviour
    {
        public GameObject BindRoot;

        protected bool collected = false;
        protected bool binded = false;

        protected readonly Dictionary<string, IData> allDatas = new Dictionary<string, IData>();
        protected readonly Dictionary<string, ICommand> allCommands = new Dictionary<string, ICommand>();
        protected readonly Dictionary<string, IConverter> allConverters = new Dictionary<string, IConverter>();

        protected readonly List<AbstractDataBinder> allDataBinders = new List<AbstractDataBinder>();
        protected readonly List<AbstractCommandBinder> allCommandBinders = new List<AbstractCommandBinder>();

        public IEnumerable<string> GetDataKeys()
        {
            return allDatas.Keys;
        }

        public IData GetData(string key)
        {
            IData d = null;
            allDatas.TryGetValue(key, out d);
            return d;
        }

        public void AddData(string key, IData data)
        {
            allDatas[key] = data;
        }

        public IEnumerable<string> GetCommandKeys()
        {
            return allCommands.Keys;
        }

        public ICommand GetCommand(string key)
        {
            ICommand cmd = null;
            allCommands.TryGetValue(key, out cmd);
            return cmd;
        }

        public void AddCommand(string key, ICommand cmd)
        {
            allCommands[key] = cmd;
        }

        public IEnumerable<string> GetConverterKeys()
        {
            return allConverters.Keys;
        }

        public IConverter GetConverter(string key)
        {
            IConverter converter = null;
            allConverters.TryGetValue(key, out converter);
            return converter;
        }

        public void AddConverter(string key, IConverter converter)
        {
            allConverters[key] = converter;
        }

        public void AddConverter(string key, Type converterType)
        {
            allConverters[key] = (IConverter)Activator.CreateInstance(converterType);
        }

        public void AddConverter<T>(string key) where T : IConverter, new()
        {
            allConverters[key] = new T();
        }

        public virtual void Collect()
        {
            Type type = this.GetType();
            FieldInfo[] fields = type.GetFields();
            for (int i = 0; i < fields.Length; i++)
            {
                FieldInfo fi = fields[i];
                Type t = fi.FieldType;
                if (typeof(IData).IsAssignableFrom(t))
                {
                    IData data = fi.GetValue(this) as IData;
                    if (data != null)
                    {
                        allDatas[fi.Name] = data;
                    }
                }
                if (typeof(ICommand).IsAssignableFrom(t))
                {
                    ICommand cmd = fi.GetValue(this) as ICommand;
                    if (cmd != null)
                    {
                        allCommands[fi.Name] = cmd;
                    }
                }
                if (typeof(IConverter).IsAssignableFrom(t))
                {
                    IConverter cvt = fi.GetValue(this) as IConverter;
                    if (cvt != null)
                    {
                        allConverters[fi.Name] = cvt;
                    }
                }
            }

            if (this.BindRoot == null)
            {
                this.BindRoot = this.gameObject;
            }
            this.BindRoot.GetComponentsInChildren<AbstractDataBinder>(true, allDataBinders);
            this.BindRoot.GetComponentsInChildren<AbstractCommandBinder>(true, allCommandBinders);
        }

        protected void Bind()
        {
            for (int i = 0; i < this.allDataBinders.Count; i++)
            {
                if (this.allDataBinders[i].CanBind(this))
                {
                    this.allDataBinders[i].Bind(this);
                }
            }

            for (int i = 0; i < this.allCommandBinders.Count; i++)
            {
                if (this.allCommandBinders[i].CanBind(this))
                {
                    this.allCommandBinders[i].Bind(this);
                }
            }
        }

        public void NotifyCommandsCanExecute()
        {
            foreach (KeyValuePair<string, ICommand> kv in allCommands)
            {
                kv.Value.NotifyCanExecute();
            }
        }

        protected virtual void BeforeAwake() { }
        protected virtual void AfterAwake() { }
        protected override void Awake()
        {
            BeforeAwake();

            base.Awake();

            Collect();
            Bind();

            AfterAwake();
        }

        protected virtual void BeforeActive() { }
        protected virtual void AfterActive() { }
        protected override void OnEnable()
        {
            BeforeActive();

            base.OnEnable();

            NotifyCommandsCanExecute();

            AfterActive();
        }

        protected virtual void BeforeDeactive() { }
        protected virtual void AfterDeactive() { }
        protected override void OnDisable()
        {
            BeforeDeactive();

            base.OnDisable();

            AfterDeactive();
        }

        public virtual void Refresh()
        {
            NotifyCommandsCanExecute();
        }

        protected virtual void BeforeDestroy() { }
        protected virtual void AfterDestroy() { }
        protected override void OnDestroy()
        {
            BeforeDestroy();

            base.OnDestroy();

            AfterDestroy();
        }
    }
}