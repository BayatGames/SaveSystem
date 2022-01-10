using Bayat.Core.EditorWindows;

using UnityEditor;

using UnityEngine;

namespace Bayat.Core
{

    [CustomEditor(typeof(AssetReferenceResolver), true)]
    public class AssetReferenceResolverEditor : Editor
    {

        protected AssetReferenceResolver assetReferenceResolver;
        protected SerializedProperty mode;
        protected SerializedProperty refreshDependenciesTimeoutInSeconds;
        protected SerializedProperty ignoredTags;
        protected SerializedProperty ignoreStatic;
        protected bool foldout = false;

        private void OnEnable()
        {
            this.assetReferenceResolver = (AssetReferenceResolver)target;
            this.mode = this.serializedObject.FindProperty("mode");
            this.refreshDependenciesTimeoutInSeconds = this.serializedObject.FindProperty("refreshDependenciesTimeoutInSeconds");
            this.ignoredTags = this.serializedObject.FindProperty("ignoredTags");
            this.ignoreStatic = this.serializedObject.FindProperty("ignoreStatic");
        }

        public override void OnInspectorGUI()
        {
            //base.OnInspectorGUI();
            if (assetReferenceResolver == null)
            {
                return;
            }

            this.serializedObject.Update();

            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(this.mode);
            EditorGUILayout.PropertyField(this.ignoredTags);
            EditorGUILayout.PropertyField(this.ignoreStatic);

            EditorGUI.BeginDisabledGroup(this.assetReferenceResolver.Mode == ReferenceResolverMode.Manual);
            EditorGUI.EndDisabledGroup();
            EditorGUILayout.PropertyField(this.refreshDependenciesTimeoutInSeconds);

            GUILayout.Label(string.Format("{0} references", this.assetReferenceResolver.GuidToReference.Count), EditorStyles.boldLabel);

            if (GUILayout.Button("Open Reference Manager"))
            {
                new AssetReferenceManagerWindow().Show();
            }

            if (GUILayout.Button("Refresh References"))
            {
                this.assetReferenceResolver.RefreshDependencies();
            }

            if (GUILayout.Button("Remove Invalid References"))
            {
                this.assetReferenceResolver.RemoveInvalidReferences();
            }

            if (GUILayout.Button("Reset"))
            {
                if (EditorUtility.DisplayDialog("Reset Asset Reference Database?", "This action will reset whole asset reference database and used GUIDs which makes the saved GUIDs obsolote, so there will be problems when loading previously saved data using this database.\n\nProceed at your own risk.", "Reset", "Cancel"))
                {
                    this.assetReferenceResolver.GuidToReference.Clear();
                    this.assetReferenceResolver.ReferenceToGuid.Clear();
                    this.assetReferenceResolver.Reset();
                }
            }

            this.serializedObject.ApplyModifiedProperties();
        }

    }

}