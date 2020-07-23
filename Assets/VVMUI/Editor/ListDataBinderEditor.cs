using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using VVMUI.Core;
using VVMUI.Core.Binder;
using VVMUI.Core.Command;
using VVMUI.Core.Data;

[CustomEditor (typeof (ListDataBinder))]
[CanEditMultipleObjects]
public class ListDataBinderEditor : Editor {
	private List<bool> itemsExpand = new List<bool> ();
	public override void OnInspectorGUI () {
		ListDataBinder binder = target as ListDataBinder;
		VMBehaviour vm = binder.GetComponentInParent<VMBehaviour> ();
		List<string> datas = new List<string> ();
		if (vm != null) {
			Type type = vm.GetType ();
			FieldInfo[] fields = type.GetFields ();
			for (int i = 0; i < fields.Length; i++) {
				FieldInfo fi = fields[i];
				Type t = fi.FieldType;
				if (t.GetInterface ("IListData") != null) {
					datas.Add (fi.Name);
				}
			}
		}

		EditorGUILayout.BeginHorizontal ();
		EditorGUILayout.LabelField ("Source:");
		int dataIndex = datas.IndexOf (binder.Source.Key);
		dataIndex = EditorGUILayout.Popup (dataIndex, datas.ToArray (), GUILayout.MinWidth (20), GUILayout.MaxWidth (150));
		if (datas.Count > 0 && dataIndex >= 0 && dataIndex < datas.Count) {
			binder.Source.Key = datas[dataIndex];
		}
		binder.Source.Key = EditorGUILayout.DelayedTextField (binder.Source.Key, GUILayout.MaxWidth (150));
		EditorGUILayout.EndHorizontal ();

		binder.Template = (GameObject) EditorGUILayout.ObjectField ("Template:", binder.Template, typeof (GameObject), true);
		binder.Optimize = EditorGUILayout.Toggle ("Optimize:", binder.Optimize);
		if (binder.Optimize) {
			EditorGUI.BeginDisabledGroup (true);
			EditorGUILayout.ObjectField ("ViewPort:", binder.ViewPort, typeof (RectTransform), true);
			EditorGUILayout.ObjectField ("ScrollRect:", binder.ScrollRect, typeof (ScrollRect), true);
			EditorGUILayout.ObjectField ("LayoutGroup:", binder.LayoutGroup, typeof (LayoutGroup), true);
			EditorGUILayout.IntField ("PageItemsCount:", binder.PageItemsCount);
			EditorGUI.EndDisabledGroup ();
		}
	}
}