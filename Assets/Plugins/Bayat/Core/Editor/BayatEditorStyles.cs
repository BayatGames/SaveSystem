using UnityEditor;
using UnityEngine;

namespace Bayat.Core
{

    /// <summary>
    /// Bayat Unity Editor GUI styles.
    /// </summary>
    public static class BayatEditorStyles
    {

        private static GUIStyle toolbarSearchField;

        public static GUIStyle ToolbarSearchField
        {
            get { return toolbarSearchField; }
        }

        static BayatEditorStyles()
        {
            toolbarSearchField = GetStyle("ToolbarSeachTextField");
        }

        public static GUIStyle GetStyle(string styleName)
        {
            GUIStyle s = GUI.skin.FindStyle(styleName) ?? EditorGUIUtility.GetBuiltinSkin(EditorSkin.Inspector).FindStyle(styleName);
            if (s == null)
            {
                Debug.LogError("Missing built-in guistyle " + styleName);
                s = GUIStyle.none;
            }
            return s;
        }

    }

}