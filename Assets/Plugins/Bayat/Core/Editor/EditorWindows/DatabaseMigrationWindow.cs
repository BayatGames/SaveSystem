using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Bayat.Core.EditorWindows
{

    public class DatabaseMigrationWindow : EditorWindowWrapper
    {

        protected Vector2 scrollPosition;
        protected List<string> availableScenes = new List<string>();
        protected List<string> selectedScenes = new List<string>();
        protected List<SceneMigrationStatus> migratedScenes = new List<SceneMigrationStatus>();

        protected int selectedTab = 0;
        protected string[] toolbarItems = new string[] { "Scenes", "Result" };

        [MenuItem("Window/Bayat/Core/Database Migration")]
        private static void Initialize()
        {
            var window = new DatabaseMigrationWindow();
            window.Show();
        }

        protected override void ConfigureWindow()
        {
            window.titleContent = new GUIContent("Database Migration");
            window.minSize = window.maxSize = new Vector2(630, 400);
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
            GUILayout.Label("Database <b>Migration</b>", Styles.CenteredLargeLabel);
            GUILayout.Label("Use this tool to <b>migrate</b> old database versions to the new one by migrating the old data without loss", Styles.CenteredMiniLabel);

            FindScenes();

            // Check for invalid selected scenes
            for (int i = 0; i < this.selectedScenes.Count; i++)
            {
                if (!this.availableScenes.Contains(this.selectedScenes[i]))
                {
                    this.selectedScenes.Remove(this.availableScenes[i]);
                }
            }

            EditorGUILayout.BeginHorizontal();

            this.selectedTab = GUILayout.Toolbar(this.selectedTab, this.toolbarItems, EditorStyles.toolbarButton);

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginVertical();

            switch (this.selectedTab)
            {
                case 0:
                default:
                    if (this.availableScenes.Count == 0)
                    {
                        GUILayout.Label("There are no scenes available", EditorStyles.centeredGreyMiniLabel);
                    }

                    for (int i = 0; i < this.availableScenes.Count; i++)
                    {
                        DrawSceneToggle(this.availableScenes[i]);
                    }
                    break;
                case 1:
                    if (this.migratedScenes.Count == 0)
                    {
                        GUILayout.Label("There are no migration data available", EditorStyles.centeredGreyMiniLabel);
                    }
                    for (int i = 0; i < this.migratedScenes.Count; i++)
                    {
                        SceneMigrationStatus status = this.migratedScenes[i];
                        EditorGUILayout.HelpBox(string.Format("{0} successfully migrated", status.ScenePath), status.Migrated ? MessageType.Info : MessageType.Warning);
                    }
                    break;
            }

            EditorGUILayout.EndVertical();

            GUILayout.FlexibleSpace();

            EditorGUILayout.BeginHorizontal();

            EditorGUI.BeginDisabledGroup(this.selectedScenes.Count == 0);
            if (GUILayout.Button("Migrate Selected Scenes", EditorStyles.toolbarButton))
            {
                MigrateScenes(this.selectedScenes.ToArray());
            }
            EditorGUI.EndDisabledGroup();

            if (GUILayout.Button("Migrate Build Scenes", EditorStyles.toolbarButton))
            {
                MigrateBuildScenes();
            }
            if (GUILayout.Button("Migrate All Scenes", EditorStyles.toolbarButton))
            {
                MigrateAllScenes();
            }
            if (GUILayout.Button("Migrate Asset Database", EditorStyles.toolbarButton))
            {
                AssetReferenceResolver.Current.MigrateReferenceDatabase();
                EditorUtility.DisplayDialog("Save System", "The Asset Reference Resolver database migrated successfully.", "Got it");
            }

            EditorGUILayout.EndHorizontal();

            GUILayout.Label("Made with ❤️ by Bayat", EditorStyles.centeredGreyMiniLabel);
        }

        protected virtual void DrawSceneToggle(string scenePath)
        {
            bool selected = EditorGUILayout.ToggleLeft(scenePath, this.selectedScenes.Contains(scenePath));
            if (selected && !this.selectedScenes.Contains(scenePath))
            {
                this.selectedScenes.Add(scenePath);
            }
            else if (!selected && this.selectedScenes.Contains(scenePath))
            {
                this.selectedScenes.Remove(scenePath);
            }
        }

        public virtual void MigrateAllScenes()
        {
            string[] allScenes = AssetDatabase.FindAssets("t: Scene");
            string[] scenePaths = new string[allScenes.Length];
            for (int i = 0; i < allScenes.Length; i++)
            {
                scenePaths[i] = AssetDatabase.GUIDToAssetPath(allScenes[i]);
            }
            MigrateScenes(scenePaths);
        }

        public virtual void MigrateBuildScenes()
        {
            string[] scenePaths = new string[EditorBuildSettings.scenes.Length];
            for (int i = 0; i < EditorBuildSettings.scenes.Length; i++)
            {
                EditorBuildSettingsScene editorScene = EditorBuildSettings.scenes[i];
                scenePaths[i] = editorScene.path;
            }
            MigrateScenes(scenePaths);
        }

        public virtual void MigrateScenes(string[] scenesPath)
        {
            if (scenesPath.Length == 0)
            {
                return;
            }
            if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            {
                return;
            }

            float progress = 0f;
            float rate = 1f / scenesPath.Length;
            this.migratedScenes.Clear();

            try
            {
                Scene activeScene = EditorSceneManager.GetActiveScene();
                for (int i = 0; i < scenesPath.Length; i++)
                {
                    string scenePath = scenesPath[i];
                    if (EditorUtility.DisplayCancelableProgressBar("Migrating databases", scenePath, progress))
                    {
                        Debug.LogFormat("Ignoring scene at {0}", scenePath);
                        Debug.LogFormat("Cancelled by user", scenePath);
                        this.migratedScenes.Add(new SceneMigrationStatus(scenePath, false, true));
                    }
                    else
                    {
                        Scene scene;
                        if (activeScene.path == scenePath)
                        {
                            scene = activeScene;
                        }
                        else
                        {
                            scene = EditorSceneManager.OpenScene(scenePath);
                        }

                        EditorSceneManager.SetActiveScene(scene);
                        SceneReferenceResolver referenceResolver = UnityEngine.Object.FindObjectOfType<SceneReferenceResolver>();
                        if (referenceResolver != null)
                        {
                            this.migratedScenes.Add(new SceneMigrationStatus(scenePath, true, false));
                            referenceResolver.MigrateReferenceDatabase();
                        }

                        EditorSceneManager.SaveScene(scene);
                        if (scene != activeScene)
                        {
                            //EditorSceneManager.CloseScene(scene, true);
                        }
                        Debug.LogFormat("Migrated scene at {0}", scenePath);
                    }
                    progress += rate;
                }
                AssetDatabase.SaveAssets();
            }
            catch (System.Exception ex)
            {
                Debug.LogException(ex);
            }
            EditorUtility.ClearProgressBar();

            // Switch to result tab
            this.selectedTab = 1;
        }

        public virtual void FindScenes()
        {
            string[] allScenes = AssetDatabase.FindAssets("t: Scene");
            this.availableScenes.Clear();
            for (int i = 0; i < allScenes.Length; i++)
            {
                this.availableScenes.Add(AssetDatabase.GUIDToAssetPath(allScenes[i]));
            }
        }

        public struct SceneMigrationStatus
        {

            public readonly string ScenePath;
            public readonly bool Migrated;
            public readonly bool Ignored;

            public SceneMigrationStatus(string scenePath, bool migrated, bool ignored)
            {
                this.ScenePath = scenePath;
                this.Migrated = migrated;
                this.Ignored = ignored;
            }

        }

        public static class Styles
        {
            static Styles()
            {
                CenteredLargeLabel = new GUIStyle(EditorStyles.largeLabel);
                CenteredLargeLabel.alignment = TextAnchor.MiddleCenter;
                CenteredLargeLabel.richText = true;
                CenteredMiniLabel = new GUIStyle(EditorStyles.miniLabel);
                CenteredMiniLabel.alignment = TextAnchor.MiddleCenter;
                CenteredMiniLabel.richText = true;
            }

            public static readonly GUIStyle CenteredLargeLabel;
            public static readonly GUIStyle CenteredMiniLabel;

        }

    }

}