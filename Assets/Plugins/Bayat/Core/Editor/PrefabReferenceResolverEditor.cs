using Bayat.Core.EditorWindows;

using UnityEditor;

using UnityEngine;

namespace Bayat.Core
{

    [CustomEditor(typeof(PrefabReferenceResolver), true)]
    public class PrefabReferenceResolverEditor : Editor
    {

        protected PrefabReferenceResolver prefabReferenceResolver;
        protected SerializedProperty mode;
        protected SerializedProperty refreshDependenciesTimeoutInSeconds;
        protected SerializedProperty ignoredTags;
        protected SerializedProperty ignoreStatic;
        protected bool foldout = false;

        private void OnEnable()
        {
            this.prefabReferenceResolver = (PrefabReferenceResolver)target;
            this.mode = serializedObject.FindProperty("mode");
            this.refreshDependenciesTimeoutInSeconds = serializedObject.FindProperty("refreshDependenciesTimeoutInSeconds");
            this.ignoredTags = serializedObject.FindProperty("ignoredTags");
            this.ignoreStatic = serializedObject.FindProperty("ignoreStatic");
        }

        public override void OnInspectorGUI()
        {
            //base.OnInspectorGUI();
            if (this.prefabReferenceResolver == null)
            {
                return;
            }

            serializedObject.Update();

            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(this.mode);
            EditorGUILayout.PropertyField(this.ignoredTags);
            EditorGUILayout.PropertyField(this.ignoreStatic);

            EditorGUI.BeginDisabledGroup(this.prefabReferenceResolver.Mode == ReferenceResolverMode.Manual);
            EditorGUI.EndDisabledGroup();
            EditorGUILayout.PropertyField(this.refreshDependenciesTimeoutInSeconds);

            GUILayout.Label(string.Format("{0} references", this.prefabReferenceResolver.GuidToReference.Count), EditorStyles.boldLabel);

            if (GUILayout.Button("Open Reference Manager"))
            {
                new PrefabReferenceManagerWindow().Show();
            }

            if (GUILayout.Button("Refresh References"))
            {
                this.prefabReferenceResolver.RefreshDependencies();
            }

            if (GUILayout.Button("Remove Invalid References"))
            {
                this.prefabReferenceResolver.RemoveInvalidReferences();
            }

            if (GUILayout.Button("Reset"))
            {
                if (EditorUtility.DisplayDialog("Reset Prefab Reference Database?", "This action will reset whole prefab reference database and used GUIDs which makes the saved GUIDs obsolete, so there will be problems when loading previously saved data using this database.\n\nProceed at your own risk.", "Reset", "Cancel"))
                {
                    this.prefabReferenceResolver.GuidToReference.Clear();
                    this.prefabReferenceResolver.ReferenceToGuid.Clear();
                    this.prefabReferenceResolver.Reset();
                }
            }

            serializedObject.ApplyModifiedProperties();
        }

    }

}