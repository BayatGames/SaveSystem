using Bayat.Core.EditorWindows;

using UnityEditor;

using UnityEngine;

namespace Bayat.SaveSystem
{

    public class AboutWindow : EditorWindowWrapper
    {

        protected const string ChangelogUrl = "https://docs.bayat.io/save-system/manual/changelog";

        [MenuItem("Window/Bayat/Save System/About")]
        private static void Initialize()
        {
            EditorApplication.update -= Initialize;
            var window = new AboutWindow();
            window.Show();
        }

        protected override void ConfigureWindow()
        {
            window.titleContent = new GUIContent("About Save System", SaveSystemEditorResources.IsometricIcon);
            window.minSize = window.maxSize = new Vector2(200, 200);
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
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label(SaveSystemEditorResources.LogoIcon, GUILayout.Width(128), GUILayout.Height(128));
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();

            GUILayout.Label(string.Format("Version: <b>{0}</b>", PluginVersion.Version), Styles.CenteredLargeLabel);

            if (GUILayout.Button("Changelog"))
            {
                OpenChangelog();
            }

            GUILayout.FlexibleSpace();
            GUILayout.Label("Made with ❤️ by Bayat", EditorStyles.centeredGreyMiniLabel);
        }

        protected virtual void OpenChangelog()
        {
            Application.OpenURL(string.Format("{0}#{1}", ChangelogUrl, PluginVersion.Version.ToString().Replace(".", string.Empty)));
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