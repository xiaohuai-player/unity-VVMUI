using System.Collections;
using UnityEditor;
using UnityEngine;
using VVMUI.Core;
using VVMUI.Core.Data;
using VVMUI.Core.Binder;

namespace VVMUI.Inspector
{
    [CustomEditor(typeof(ListTemplateBinder))]
    [CanEditMultipleObjects]
    public class ListTemplateBinderEditor : Editor
    {
        private DataDefinerDrawer definerDrawer;
        public override void OnInspectorGUI()
        {
            ListTemplateBinder binder = target as ListTemplateBinder;
            VMBehaviour vm = binder.GetComponentInParent<VMBehaviour>();

            if (definerDrawer == null)
            {
                definerDrawer = new DataDefinerDrawer(binder.ItemSource);
            }
            definerDrawer.Draw(vm, null, typeof(IEnumerable));

            IData data = binder.ItemSource.GetData(vm);
            IListData list = data as IListData;
            if (list == null)
            {
                GUIStyle style = new GUIStyle(EditorStyles.boldLabel);
                style.normal.textColor = Color.red;
                EditorGUILayout.LabelField("no source list data.", style);
            }

            EditorGUILayout.LabelField("index: " + binder.Index);
        }
    }
}