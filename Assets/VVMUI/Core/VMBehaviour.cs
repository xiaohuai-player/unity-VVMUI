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
	[ExecuteInEditMode]
	public class VMBehaviour : MonoBehaviour {
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
				if (fields[i].FieldType.GetInterface ("IData") != null) {
					IData data = fields[i].GetValue (this) as IData;
					if (data != null) {
						_allData[fields[i].Name] = data;
					}
				}
				if (fields[i].FieldType.GetInterface ("ICommand") != null) {
					ICommand cmd = fields[i].GetValue (this) as ICommand;
					if (cmd != null) {
						_allCommands[fields[i].Name] = cmd;
					}
				}
				if (fields[i].FieldType.GetInterface ("IConverter") != null) {
					IConverter cvt = fields[i].GetValue (this) as IConverter;
					if (cvt != null) {
						_allConverters[fields[i].Name] = cvt;
					}
				}
			}
		}

		protected void Bind () {
			BaseDataBinder[] databinders = this.gameObject.GetComponentsInChildren<BaseDataBinder> (true);
			for (int i = 0; i < databinders.Length; i++) {
				databinders[i].Bind (this);
			}

			BaseCommandBinder[] cmdbinders = this.gameObject.GetComponentsInChildren<BaseCommandBinder> (true);
			for (int i = 0; i < cmdbinders.Length; i++) {
				cmdbinders[i].Bind (this);
			}
		}

		protected void RefreshCommandsCanExecute () {
			foreach (KeyValuePair<string, ICommand> kv in _allCommands) {
				kv.Value.RefreshCanExecute ();
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

			RefreshCommandsCanExecute ();

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
			RefreshCommandsCanExecute ();
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