using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Scripting;
using UnityEngine.EventSystems;
using VVMUI.Core.Binder;
using VVMUI.Core.Command;
using VVMUI.Core.Converter;
using VVMUI.Core.Data;

[assembly: Preserve]

namespace VVMUI.Core
{
    public class VMBehaviour : UIBehaviour
    {
        public GameObject BindRoot;

        [HideInInspector]
        public object ActiveParameter;

        private readonly Dictionary<string, IData> allDatas = new Dictionary<string, IData>();
        private readonly Dictionary<string, ICommand> allCommands = new Dictionary<string, ICommand>();
        private readonly Dictionary<string, IConverter> allConverters = new Dictionary<string, IConverter>();

        private readonly List<AbstractDataBinder> allDataBinders = new List<AbstractDataBinder>();
        private readonly List<AbstractCommandBinder> allCommandBinders = new List<AbstractCommandBinder>();

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

        public void Collect()
        {
            Type type = this.GetType();
            FieldInfo[] fields = type.GetFields();
            for (int i = 0; i < fields.Length; i++)
            {
                FieldInfo fi = fields[i];
                Type t = fi.FieldType;
                if (t.GetInterface("IData") != null || typeof(StructData).IsAssignableFrom(t))
                {
                    IData data = fi.GetValue(this) as IData;
                    if (data != null)
                    {
                        allDatas[fi.Name] = data;
                    }
                }
                if (t.GetInterface("ICommand") != null)
                {
                    ICommand cmd = fi.GetValue(this) as ICommand;
                    if (cmd != null)
                    {
                        allCommands[fi.Name] = cmd;
                    }
                }
                if (t.GetInterface("IConverter") != null)
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

            //TODO real deactive

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

            //TODO real destroy

            AfterDestroy();
        }
    }
}