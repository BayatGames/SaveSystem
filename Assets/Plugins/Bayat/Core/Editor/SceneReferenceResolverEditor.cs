using Bayat.Core.EditorWindows;

using UnityEditor;

using UnityEngine;

namespace Bayat.Core
{

    [CustomEditor(typeof(SceneReferenceResolver), true)]
    public class SceneReferenceResolverEditor : Editor
    {

        protected SceneReferenceResolver sceneReferenceResolver;
        protected SerializedProperty mode;
        protected SerializedProperty updateOnEnteringPlayMode;
        protected SerializedProperty updateOnSceneSaving;
        protected SerializedProperty refreshDependenciesTimeoutInSeconds;
        protected SerializedProperty ignoredTags;
        protected SerializedProperty ignoreStatic;
        protected bool foldout = false;

        private void OnEnable()
        {
            this.sceneReferenceResolver = (SceneReferenceResolver)target;
            this.mode = this.serializedObject.FindProperty("mode");
            this.updateOnEnteringPlayMode = this.serializedObject.FindProperty("updateOnEnteringPlayMode");
            this.updateOnSceneSaving = this.serializedObject.FindProperty("updateOnSceneSaving");
            this.refreshDependenciesTimeoutInSeconds = this.serializedObject.FindProperty("refreshDependenciesTimeoutInSeconds");
            this.ignoredTags = this.serializedObject.FindProperty("ignoredTags");
            this.ignoreStatic = this.serializedObject.FindProperty("ignoreStatic");
        }

        public override void OnInspectorGUI()
        {
            //base.OnInspectorGUI();
            if (sceneReferenceResolver == null)
            {
                return;
            }

            this.serializedObject.Update();

            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(this.mode);
            EditorGUILayout.PropertyField(this.ignoredTags);
            EditorGUILayout.PropertyField(this.ignoreStatic);

            EditorGUI.BeginDisabledGroup(this.sceneReferenceResolver.Mode == ReferenceResolverMode.Manual);
            EditorGUILayout.PropertyField(this.updateOnEnteringPlayMode);
            EditorGUILayout.PropertyField(this.updateOnSceneSaving);
            EditorGUI.EndDisabledGroup();
            EditorGUILayout.PropertyField(this.refreshDependenciesTimeoutInSeconds);

            GUILayout.Label(string.Format("{0} references", this.sceneReferenceResolver.GuidToReference.Count), EditorStyles.boldLabel);

            if (GUILayout.Button("Open Reference Manager"))
            {
                new SceneReferenceManagerWindow().Show();
            }

            if (GUILayout.Button("Refresh References"))
            {
                this.sceneReferenceResolver.RefreshDependencies();
            }

            if (GUILayout.Button("Remove Invalid References"))
            {
                this.sceneReferenceResolver.RemoveInvalidReferences();
            }

            //var availableDependencies = this.sceneReferenceResolver.AvailableDependencies;
            //var currentDependencies = this.sceneReferenceResolver.SceneDependencies;
            //GUILayout.Label(string.Format("Available: {0}", availableDependencies.Count));
            //GUILayout.Label(string.Format("Current: {0}", currentDependencies.Count));
            //if (availableDependencies.Count > 0)
            //{
            //    EditorGUILayout.HelpBox(string.Format("There are {0} more dependencies available to reference.", availableDependencies.Count), MessageType.Warning);
            //}

            //if (GUILayout.Button("Refresh Available Dependencies"))
            //{
            //    this.sceneReferenceResolver.GetAvailableDependencies();
            //}
            //if (this.sceneReferenceResolver.HasNullReferences())
            //{
            //    EditorGUILayout.HelpBox("There are null references in the scene dependencies, remove them to stop causing further issues and errors.", MessageType.Warning);
            //    if (GUILayout.Button("Remove Null References"))
            //    {
            //        this.sceneReferenceResolver.RemoveNullReferences();
            //    }
            //}

            this.serializedObject.ApplyModifiedProperties();
        }

        public virtual void MigrateDatabase()
        {

        }

        //public List<UnityObject> GetAvailableDependencies()
        //{
        //    var sceneDependencies = new List<UnityObject>();
        //    var sceneObjects = SceneManager.GetActiveScene().GetRootGameObjects();
        //    var dependencies = EditorUtility.CollectDependencies(sceneObjects);
        //    foreach (var dependency in dependencies)
        //    {
        //        if (EditorUtility.IsPersistent(dependency))
        //        {
        //            continue;
        //        }
        //        sceneDependencies.Add(dependency);
        //    }
        //    return sceneDependencies;
        //}

        //public UnityObject[] GetAvailableHierarchyDependencies()
        //{
        //    var sceneObjects = SceneManager.GetActiveScene().GetRootGameObjects();
        //    return EditorUtility.CollectDeepHierarchy(sceneObjects);
        //}

    }

}