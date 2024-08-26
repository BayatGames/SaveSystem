using Bayat.Core.EditorWindows;
using Bayat.Json;

using UnityEditor;

using UnityEngine;

namespace Bayat.SaveSystem
{

    public class SaveSystemDefaultSettingsWindow : EditorWindowWrapper
    {

        protected SerializedObject settings;
        protected SerializedProperty storageConnectionStringProperty;
        protected SerializedProperty useMetaDataProperty;
        protected SerializedProperty useCatalogProperty;
        protected SerializedProperty useEncryptionoProperty;
        protected SerializedProperty passwordProperty;
        protected SerializedProperty encryptionAlgorithmNameProperty;
        protected SerializedProperty serializerSettingsPresetProperty;
        protected SerializedProperty logExceptionsProperty;

        [MenuItem("Window/Bayat/Save System/Default Settings")]
        private static void Initialize()
        {
            var window = new SaveSystemDefaultSettingsWindow();
            window.Show();
        }

        protected override void ConfigureWindow()
        {
            window.titleContent = new GUIContent("Save System - Settings", SaveSystemEditorResources.IsometricIcon);
            window.minSize = window.maxSize = new Vector2(630, 400);

            this.settings = new SerializedObject(SaveSystemSettingsPreset.DefaultPreset);
            this.storageConnectionStringProperty = this.settings.FindProperty("storageConnectionString");
            this.useMetaDataProperty = this.settings.FindProperty("useMetaData");
            this.useCatalogProperty = this.settings.FindProperty("useCatalog");
            this.useEncryptionoProperty = this.settings.FindProperty("useEncryption");
            this.passwordProperty = this.settings.FindProperty("password");
            this.encryptionAlgorithmNameProperty = this.settings.FindProperty("ecryptionAlgorithmName");
            this.serializerSettingsPresetProperty = this.settings.FindProperty("serializerSettingsPreset");
            this.logExceptionsProperty = this.settings.FindProperty("logExceptions");
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
            GUILayout.Label("Default <b>Settings</b>", Styles.CenteredLargeLabel);

            this.settings.Update();

            EditorGUILayout.PropertyField(this.storageConnectionStringProperty);
            GUILayout.Label(@"{0} = Application.persistentDataPath
{1} = Application.dataPath
{2} = Application.streamingAssetsPath
{3} = Application.temporaryCachePath
{4} = Application.absoluteURL
{5} = Application.buildGUID
{6} = Application.companyName
{7} = Application.productName
{8} = Application.identifier
{9} = Application.version
{10} = Application.unityVersion", EditorStyles.miniLabel);

            EditorGUILayout.PropertyField(this.useMetaDataProperty);
            EditorGUILayout.PropertyField(this.useCatalogProperty);
            EditorGUILayout.PropertyField(this.useEncryptionoProperty);
            EditorGUILayout.PropertyField(this.passwordProperty);
            EditorGUILayout.PropertyField(this.encryptionAlgorithmNameProperty);
            EditorGUILayout.PropertyField(this.serializerSettingsPresetProperty);
            EditorGUILayout.PropertyField(this.logExceptionsProperty);
            if (GUILayout.Button("Open Default Json Settings"))
            {
                new JsonDefaultSettingsWindow().Show();
            }

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