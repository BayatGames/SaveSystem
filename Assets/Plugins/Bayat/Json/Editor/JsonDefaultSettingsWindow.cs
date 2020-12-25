using Bayat.Core.EditorWindows;
using UnityEditor;
using UnityEngine;

namespace Bayat.Json
{

    public class JsonDefaultSettingsWindow : EditorWindowWrapper
    {

        protected SerializedObject settings;
        protected Vector2 scrollPosition;

        [MenuItem("Window/Bayat/Json/Default Settings")]
        private static void Initialize()
        {
            var window = new JsonDefaultSettingsWindow();
            window.Show();
        }

        protected override void ConfigureWindow()
        {
            window.titleContent = new GUIContent("Json - Settings");
            window.minSize = window.maxSize = new Vector2(630, 400);

            this.settings = new SerializedObject(JsonSerializerSettingsPreset.DefaultPreset);
        }

        public new void Show()
        {
            if (window == null)
            {
                ShowUtility();
                window.Center();
            }
            else
            {
                window.Focus();
            }
        }

        public override void OnGUI()
        {
            GUILayout.Label("Default <b>Json Settings</b>", Styles.CenteredLargeLabel);

            this.settings.Update();

            this.scrollPosition = EditorGUILayout.BeginScrollView(this.scrollPosition);
            SerializedProperty iterator = this.settings.GetIterator();
            iterator.NextVisible(true);
            while (iterator.NextVisible(false))
            {
                EditorGUILayout.PropertyField(iterator, true, new GUILayoutOption[0]);
            }
            EditorGUILayout.EndScrollView();

            this.settings.ApplyModifiedProperties();

            GUILayout.FlexibleSpace();
            GUILayout.Label("Made with ❤️ by Bayat", EditorStyles.centeredGreyMiniLabel);
        }

        public static class Styles
        {
            static Styles()
            {
                CenteredLargeLabel = new GUIStyle(EditorStyles.largeLabel);
                CenteredLargeLabel.alignment = TextAnchor.MiddleCenter;
                CenteredLargeLabel.richText = true;
            }

            public static readonly GUIStyle CenteredLargeLabel;

        }

    }

}