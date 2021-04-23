using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using VVMUI.Core;
using VVMUI.Core.Data;
using VVMUI.Core.Binder;

namespace VVMUI.Inspector
{
    [CustomEditor(typeof(ListDataBinder))]
    [CanEditMultipleObjects]
    public class ListDataBinderEditor : Editor
    {
        private DataDefinerDrawer definerDrawer;
        public override void OnInspectorGUI()
        {
            ListDataBinder binder = target as ListDataBinder;
            binder.EditorBind();

            if (definerDrawer == null)
            {
                definerDrawer = new DataDefinerDrawer(binder.Source);
            }
            definerDrawer.Draw(binder.BindVM, binder.BindData, typeof(IList));

            binder.Template = (GameObject)EditorGUILayout.ObjectField("Template:", binder.Template, typeof(GameObject), true);
            if (binder.Template == null || binder.Template.GetComponent<ListTemplateBinder>() == null)
            {
                GUIStyle style = new GUIStyle(EditorStyles.boldLabel);
                style.normal.textColor = Color.red;
                if (binder.Template == null)
                {
                    EditorGUILayout.LabelField("template can not be null.", style);
                }
                else
                {
                    EditorGUILayout.LabelField("template game object must have ListTemplateBinder.", style);
                }
            }
            else
            {
                IData data = binder.Source.GetData(binder.BindVM);
                if (data != null && data is IListData)
                {
                    IListData list = (IListData)data;
                    if (list.Count <= 0)
                    {
                        GUIStyle style = new GUIStyle(EditorStyles.boldLabel);
                        style.normal.textColor = Color.red;
                        EditorGUILayout.LabelField("source list data must instantiate with one element at least.", style);
                    }
                    else
                    {
                        binder.Template.GetComponent<ListTemplateBinder>().ItemSource.Key = binder.Source.Key;
                    }
                }
            }

            binder.Optimize = EditorGUILayout.Toggle("Optimize:", binder.Optimize);
            if (binder.Optimize)
            {
                EditorGUI.BeginDisabledGroup(true);
                EditorGUILayout.ObjectField("Canvas:", binder.Canvas, typeof(Canvas), true);
                EditorGUILayout.ObjectField("ViewPort:", binder.ViewPort, typeof(RectTransform), true);
                EditorGUILayout.ObjectField("ScrollRect:", binder.ScrollRect, typeof(ScrollRect), true);
                EditorGUILayout.ObjectField("LayoutGroup:", binder.LayoutGroup, typeof(LayoutGroup), true);
                EditorGUILayout.IntField("PageRowsCount:", binder.PageRowsCount);
                EditorGUILayout.IntField("RowItemsCount:", binder.RowItemsCount);
                EditorGUILayout.IntField("StepRowsCount:", binder.StepRowsCount);
                EditorGUI.EndDisabledGroup();
            }
        }
    }
}