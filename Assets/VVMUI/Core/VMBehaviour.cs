using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using VVMUI.Core.Binder;
using VVMUI.Core.Command;
using VVMUI.Core.Converter;
using VVMUI.Core.Data;

namespace VVMUI.Core {
	public class VMBehaviour : MonoBehaviour {
		public GameObject BindRoot;

		private Dictionary<string, IData> _allData = new Dictionary<string, IData> ();
		private Dictionary<string, ICommand> _allCommands = new Dictionary<string, ICommand> ();
		private Dictionary<string, IConverter> _allConverters = new Dictionary<string, IConverter> ();

		public IData GetData (string key) {
			IData d = null;
			_allData.TryGetValue (key, out d);
			return d;
		}

		public void AddData (string key, IData data) {
			_allData[key] = data;
		}

		public ICommand GetCommand (string key) {
			ICommand cmd = null;
			_allCommands.TryGetValue (key, out cmd);
			return cmd;
		}

		public void AddCommand (string key, ICommand cmd) {
			_allCommands[key] = cmd;
		}

		public IConverter GetConverter (string key) {
			IConverter converter = null;
			_allConverters.TryGetValue (key, out converter);
			return converter;
		}

		public void AddConverter (string key, IConverter converter) {
			_allConverters[key] = converter;
		}

		public void AddConverter (string key, Type converterType) {
			_allConverters[key] = (IConverter) Activator.CreateInstance (converterType);
		}

		public void AddConverter<T> (string key) where T : IConverter, new () {
			_allConverters[key] = new T ();
		}

		protected void Collect () {
			Type type = this.GetType ();
			FieldInfo[] fields = type.GetFields ();
			for (int i = 0; i < fields.Length; i++) {
				FieldInfo fi = fields[i];
				Type t = fi.FieldType;
				if (t.GetInterface ("IData") != null || t.BaseType == typeof (StructData)) {
					IData data = fi.GetValue (this) as IData;
					if (data != null) {
						_allData[fi.Name] = data;
					}
				}
				if (t.GetInterface ("ICommand") != null) {
					ICommand cmd = fi.GetValue (this) as ICommand;
					if (cmd != null) {
						_allCommands[fi.Name] = cmd;
					}
				}
				if (t.GetInterface ("IConverter") != null) {
					IConverter cvt = fi.GetValue (this) as IConverter;
					if (cvt != null) {
						_allConverters[fi.Name] = cvt;
					}
				}
			}
		}

		protected void Bind () {
			if (this.BindRoot == null) {
				this.BindRoot = this.gameObject;
			}

			AbstractDataBinder[] databinders = this.BindRoot.GetComponentsInChildren<AbstractDataBinder> (true);
			for (int i = 0; i < databinders.Length; i++) {
				if (databinders[i].CanBind (this)) {
					databinders[i].Bind (this);
				}
			}

			AbstractCommandBinder[] cmdbinders = this.BindRoot.GetComponentsInChildren<AbstractCommandBinder> (true);
			for (int i = 0; i < cmdbinders.Length; i++) {
				if (cmdbinders[i].CanBind (this)) {
					cmdbinders[i].Bind (this);
				}
			}
		}

		public void NotifyCommandsCanExecute () {
			foreach (KeyValuePair<string, ICommand> kv in _allCommands) {
				kv.Value.NotifyCanExecute ();
			}
		}

		protected virtual void BeforeAwake () { }
		protected virtual void AfterAwake () { }
		private void Awake () {
			BeforeAwake ();

			Collect ();
			Bind ();

			AfterAwake ();
		}

		protected virtual void BeforeActive () { }
		protected virtual void AfterActive () { }
		private void OnEnable () {
			BeforeActive ();

			NotifyCommandsCanExecute ();

			AfterActive ();
		}

		protected virtual void BeforeDeactive () { }
		protected virtual void AfterDeactive () { }
		private void OnDisable () {
			BeforeDeactive ();

			//TODO real deactive

			AfterDeactive ();
		}

		protected virtual void OnUpdate () { }
		private void Update () {
			OnUpdate ();
		}

		public virtual void Refresh () {
			NotifyCommandsCanExecute ();
		}

		protected virtual void BeforeDestroy () { }
		protected virtual void AfterDestroy () { }
		private void OnDestroy () {
			BeforeDestroy ();

			//TODO real destroy

			AfterDestroy ();
		}
	}
}