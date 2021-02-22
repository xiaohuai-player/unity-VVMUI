using UnityEditor;
using UnityEngine;
using VVMUI.Core;
using VVMUI.Core.Binder;

namespace VVMUI.Inspector
{
    [CustomEditor(typeof(VMDataBinder))]
    [CanEditMultipleObjects]
    public class VMDataBinderEditor : Editor
    {
        private DataDefinerDrawer definerDrawer;
        public override void OnInspectorGUI()
        {
            VMDataBinder binder = target as VMDataBinder;
            binder.EditorBind();

            if (definerDrawer == null)
            {
                definerDrawer = new DataDefinerDrawer(binder.Source);
            }
            definerDrawer.Draw(binder.BindVM, binder.BindData, typeof(object));

            binder.Template = (GameObject)EditorGUILayout.ObjectField("Template:", binder.Template, typeof(GameObject), true);
            if (binder.Template == null || binder.Template.GetComponent<VMBehaviour>() == null)
            {
                GUIStyle style = new GUIStyle(EditorStyles.boldLabel);
                style.normal.textColor = Color.red;
                if (binder.Template == null)
                {
                    EditorGUILayout.LabelField("template can not be null.", style);
                }
                else
                {
                    EditorGUILayout.LabelField("template game object must have VMBehaviour.", style);
                }
            }
        }
    }
}