using Bayat.Core.EditorWindows;

using UnityEditor;
using UnityEditor.Build;

using UnityEngine;

namespace Bayat.SaveSystem
{

    public class WelcomeWindow : EditorWindowWrapper
    {

        private static bool? _showOnStartup;

        public static bool ShowOnStartup
        {
            get
            {
                if (!_showOnStartup.HasValue)
                {
                    _showOnStartup = EditorPrefs.GetBool("Bayat.SaveSystem.WelcomeWindow.ShowOnStartUp", true);
                }
                return _showOnStartup.GetValueOrDefault();
            }
            set
            {
                _showOnStartup = value;
            }
        }

        [InitializeOnLoadMethod]
        private static void InitializeOnLoad()
        {
            if (!ShowOnStartup)
            {
                return;
            }
            EditorApplication.update += Initialize;
        }

        [MenuItem("Window/Bayat/Save System/Welcome")]
        private static void Initialize()
        {
            EditorApplication.update -= Initialize;
            var window = new WelcomeWindow();
            window.Show();
        }

        protected override void ConfigureWindow()
        {
            window.titleContent = new GUIContent("Welcome to Save System", SaveSystemEditorResources.IsometricIcon);
            window.minSize = window.maxSize = new Vector2(530, 460);
        }

        [InitializeOnLoadMethod]
        private static void CheckScriptingRuntime()
        {
            #if UNITY_6000_0_OR_NEWER
            var currentCompatiblityLevel = PlayerSettings.GetApiCompatibilityLevel(NamedBuildTarget.FromBuildTargetGroup(EditorUserBuildSettings.selectedBuildTargetGroup));
            #else
            var currentCompatiblityLevel = PlayerSettings.GetApiCompatibilityLevel(BuildPipeline.GetBuildTargetGroup(EditorUserBuildSettings.activeBuildTarget));
            #endif
#if !UNITY_2019_3_OR_NEWER
            if (PlayerSettings.scriptingRuntimeVersion != ScriptingRuntimeVersion.Latest || currentCompatiblityLevel != ApiCompatibilityLevel.NET_4_6)
            {
                UpdateScriptingRuntime();
                return;
            }
#endif
            if (currentCompatiblityLevel != ApiCompatibilityLevel.NET_4_6)
            {
                //UpdateScriptingRuntime();
            }
        }

        private static void UpdateScriptingRuntime()
        {
            if (EditorUtility.DisplayDialog(".NET 4.x is required", "The save system requires .NET 4.x, would you like to enable it now?", "Enable Now", "Cancel"))
            {
#if !UNITY_2019_3_OR_NEWER
                PlayerSettings.scriptingRuntimeVersion = ScriptingRuntimeVersion.Latest;
#elif UNITY_6000_0_OR_NEWER
                PlayerSettings.SetApiCompatibilityLevel(NamedBuildTarget.FromBuildTargetGroup(EditorUserBuildSettings.selectedBuildTargetGroup), ApiCompatibilityLevel.NET_4_6);
#else
                PlayerSettings.SetApiCompatibilityLevel(BuildPipeline.GetBuildTargetGroup(EditorUserBuildSettings.activeBuildTarget), ApiCompatibilityLevel.NET_4_6);
#endif
            }
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

        public override void OnClose()
        {
            if (ShowOnStartup)
            {
                ShowOnStartup = false;
            }
            EditorPrefs.SetBool("Bayat.SaveSystem.WelcomeWindow.ShowOnStartUp", ShowOnStartup);
        }

        public override void OnGUI()
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label(SaveSystemEditorResources.LogoIcon, GUILayout.Width(128), GUILayout.Height(128));
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();

            GUILayout.Label("Thank you for choosing <b>Bayat - Save System</b>", Styles.CenteredLargeLabel);

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Try out Demos", EditorStyles.boldLabel);
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Download Demos"))
            {
                Application.OpenURL("http://docs.bayat.io/save-system/packages/demos.unitypackage");
            }
            EditorGUILayout.EndHorizontal();
            GUILayout.Label("Try out different aspects of the system and its features by installing the demos.", EditorStyles.wordWrappedLabel);

            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Watch Demos", EditorStyles.boldLabel);
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Watch Now"))
            {
                Application.OpenURL("https://youtu.be/KBCrJMHynG4");
            }
            EditorGUILayout.EndHorizontal();
            GUILayout.Label("Watch the demo reel and see how the save system works in action.", EditorStyles.wordWrappedLabel);

            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Manual", EditorStyles.boldLabel);
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Learn More"))
            {
                Application.OpenURL("http://docs.bayat.io/save-system/manual");
            }
            EditorGUILayout.EndHorizontal();
            GUILayout.Label("Get started with the system using the helpful and in-depth manual.", EditorStyles.wordWrappedLabel);

            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Scripting API", EditorStyles.boldLabel);
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Learn More"))
            {
                Application.OpenURL("http://docs.bayat.io/save-system/api");
            }
            EditorGUILayout.EndHorizontal();
            GUILayout.Label("Check out the scripting API reference of the save system.", EditorStyles.wordWrappedLabel);

            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Integrations", EditorStyles.boldLabel);
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Learn More"))
            {
                Application.OpenURL("http://docs.bayat.io/save-system/manual/integrations");
            }
            EditorGUILayout.EndHorizontal();
            GUILayout.Label("Check out the available integrations for extending the system.", EditorStyles.wordWrappedLabel);

            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Support & More", EditorStyles.boldLabel);
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Email"))
            {
                Application.OpenURL("mailto:support@bayat.io");
            }
            if (GUILayout.Button("Discord"))
            {
                Application.OpenURL("https://discord.gg/HWMqD7T");
            }
            if (GUILayout.Button("Forum"))
            {
                Application.OpenURL("https://forum.unity.com/threads/bayat-save-system-an-ultimate-data-management-solution.817416/");
            }
            if (GUILayout.Button("YouTube"))
            {
                Application.OpenURL("https://www.youtube.com/channel/UCDLJbvqDKJyBKU2E8TMEQpQ/");
            }
            EditorGUILayout.EndHorizontal();
            GUILayout.Label("Have an issue or question? Let us know!", EditorStyles.wordWrappedLabel);

            EditorGUILayout.EndVertical();

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