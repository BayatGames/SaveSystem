using System;
using System.Collections;
using System.Collections.Generic;

using UnityEditor;

using UnityEngine;
using UnityEngine.SceneManagement;

using UnityObject = UnityEngine.Object;

namespace Bayat.Core
{

    /// <summary>
    /// A small utility window for adding references to <see cref="SceneReferenceResolver"/> and <see cref="AssetReferenceResolver"/>
    /// </summary>
    public class AddReferenceWindow : EditorWindow
    {

        protected List<UnityObject> selectedUnityObjects = new List<UnityObject>();
        protected Vector2 scrollPosition;
        protected List<Dependency> dependencies = new List<Dependency>();
        protected List<Dependency> selectedDependencies = new List<Dependency>();
        protected AssetReferenceResolver assetReferenceResolver = AssetReferenceResolver.Current;
        protected List<Scene> missingSceneReferenceManagers = new List<Scene>();

        [MenuItem("Window/Bayat/Core/Add Reference")]
        public static AddReferenceWindow Initialize()
        {
            var window = GetWindow<AddReferenceWindow>();
            window.titleContent = new GUIContent("Add Reference");
            return window;
        }

        public static AddReferenceWindow Initialize(UnityObject[] unityObjects)
        {
            var window = Initialize();
            for (int i = 0; i < Selection.objects.Length; i++)
            {
                if (!window.selectedUnityObjects.Contains(Selection.objects[i]))
                {
                    window.selectedUnityObjects.Add(Selection.objects[i]);
                }
            }
            window.selectedDependencies.Clear();
            window.OnSelectionChanged();
            return window;
        }

        protected virtual void OnEnable()
        {
            //Selection.selectionChanged += OnSelectionChanged;
        }

        protected virtual void OnDisable()
        {
            //Selection.selectionChanged -= OnSelectionChanged;
        }

        protected virtual void OnInspectorUpdate()
        {
            Repaint();
        }

        protected virtual void OnSelectionChanged()
        {
            RefreshDependencies();
        }

        protected virtual void RefreshDependencies()
        {
            this.dependencies.Clear();
            this.missingSceneReferenceManagers.Clear();
            var unityObjects = new List<UnityObject>(EditorUtility.CollectDependencies(this.selectedUnityObjects.ToArray()));
            for (int i = 0; i < unityObjects.Count; i++)
            {
                var unityObject = unityObjects[i];
                if (IsReferenced(unityObject))
                {
                    continue;
                }
                var dependency = new Dependency();
                dependency.unityObject = unityObject;
                dependency.preview = AssetPreview.GetAssetPreview(unityObject);
                if (dependency.preview == null)
                {
                    dependency.preview = AssetPreview.GetMiniThumbnail(unityObject);
                }
                dependency.isPersistent = EditorUtility.IsPersistent(unityObject);
                if (!dependency.isPersistent)
                {
                    SceneReferenceResolver sceneReferenceResolver = null;
                    Scene? scene = SceneReferenceResolver.GetUnityObjectScene(unityObject);
                    if (scene != null)
                    {
                        Scene validScene = (Scene)scene;
                        sceneReferenceResolver = SceneReferenceResolver.GetReferenceResolver(validScene);
                        dependency.missingReferenceResolver = sceneReferenceResolver == null;
                        if (sceneReferenceResolver == null && !this.missingSceneReferenceManagers.Contains(validScene))
                        {
                            this.missingSceneReferenceManagers.Add(validScene);
                        }
                        if (sceneReferenceResolver != null && !sceneReferenceResolver.IsValidUnityObject(unityObject))
                        {
                            dependency = null;
                            continue;
                        }
                    }
                }
                else
                {
                    if (this.assetReferenceResolver != null && !this.assetReferenceResolver.IsValidUnityObject(unityObject))
                    {
                        dependency = null;
                        continue;
                    }
                    dependency.missingReferenceResolver = this.assetReferenceResolver == null;
                }
                this.dependencies.Add(dependency);
            }
            this.dependencies.Sort((x, y) =>
            {
                return x.isPersistent.CompareTo(y.isPersistent);
            });
        }

        protected virtual bool IsReferenced(UnityObject unityObject)
        {
            if (this.assetReferenceResolver == null)
            {
                return false;
            }
            bool isReferenced = this.assetReferenceResolver.Contains(unityObject);
            if (isReferenced)
            {
                return isReferenced;
            }
            if (unityObject is GameObject || unityObject is Component)
            {
                Scene? scene = SceneReferenceResolver.GetUnityObjectScene(unityObject);
                if (scene != null)
                {
                    Scene validScene = (Scene)scene;
                    var sceneReferenceResolver = SceneReferenceResolver.GetReferenceResolver(validScene);
                    if (sceneReferenceResolver == null)
                    {
                        return false;
                    }
                    return sceneReferenceResolver.Contains(unityObject);
                }
            }
            return false;
        }

        protected virtual void OnGUI()
        {
            EditorGUI.BeginChangeCheck();
            if (this.selectedUnityObjects.Count <= 0)
            {
                this.selectedUnityObjects.Add(null);
            }
            var removeIndices = new List<int>();
            for (int i = 0; i < this.selectedUnityObjects.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();
                this.selectedUnityObjects[i] = EditorGUILayout.ObjectField(string.Format("Selected Object {0}", i + 1), this.selectedUnityObjects[i], typeof(UnityObject), true);
                if (GUILayout.Button("X", GUILayout.Width(30)))
                {
                    removeIndices.Add(i);
                }
                EditorGUILayout.EndHorizontal();
            }
            foreach (int index in removeIndices)
            {
                this.selectedUnityObjects.RemoveAt(index);
            }
            removeIndices = null;
            if (this.selectedUnityObjects.Count <= 0)
            {
                this.selectedUnityObjects.Add(null);
            }
            if (EditorGUI.EndChangeCheck())
            {
                this.selectedDependencies.Clear();
                OnSelectionChanged();
            }
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Use Selection Object") || this.selectedUnityObjects == null || this.selectedUnityObjects.Count == 0)
            {
                for (int i = 0; i < Selection.objects.Length; i++)
                {
                    if (!this.selectedUnityObjects.Contains(Selection.objects[i]))
                    {
                        this.selectedUnityObjects.Add(Selection.objects[i]);
                    }
                }
                this.selectedDependencies.Clear();
                OnSelectionChanged();
            }
            EditorGUILayout.EndHorizontal();

            EditorGUI.BeginChangeCheck();
            this.assetReferenceResolver = (AssetReferenceResolver)EditorGUILayout.ObjectField("Asset Reference Manager", this.assetReferenceResolver, typeof(AssetReferenceResolver), false);
            if (EditorGUI.EndChangeCheck())
            {
                RefreshDependencies();
            }

            EditorGUILayout.BeginHorizontal();
            bool deselectAll = false;
            bool selectAll = false;
            if (this.selectedDependencies.Count > 0)
            {
                deselectAll = GUILayout.Button("Deselect All");
            }
            else
            {
                selectAll = GUILayout.Button("Select All");
            }
            EditorGUILayout.EndHorizontal();

            if (this.assetReferenceResolver == null)
            {
                EditorGUILayout.HelpBox("Specify an Asset Reference Manager to resolve Assets references, otherwise Asset objects cannot be referenced.", MessageType.Warning);
            }
            if (this.missingSceneReferenceManagers.Count > 0)
            {
                EditorGUILayout.HelpBox("The below scenes don't have a Scene Reference Manager, their objects cannot be referenced until they have one.", MessageType.Warning);
            }
            for (int i = 0; i < this.missingSceneReferenceManagers.Count; i++)
            {
                EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
                GUILayout.Label(this.missingSceneReferenceManagers[i].name);
                if (GUILayout.Button("Add Manager"))
                {
                    var gameObject = new GameObject("Scene Reference Manager", typeof(SceneReferenceResolver));
                    SceneManager.MoveGameObjectToScene(gameObject, this.missingSceneReferenceManagers[i]);
                    Undo.RegisterCreatedObjectUndo(gameObject, "Automatically Created Scene Referernce Manager");
                    RefreshDependencies();
                }
                EditorGUILayout.EndHorizontal();
            }

            this.scrollPosition = EditorGUILayout.BeginScrollView(this.scrollPosition);
            for (int i = 0; i < this.dependencies.Count; i++)
            {
                var dependency = this.dependencies[i];
                bool isAlreadySelected = this.selectedDependencies.Contains(dependency);

                EditorGUI.BeginDisabledGroup(dependency.missingReferenceResolver);
                EditorGUILayout.BeginHorizontal();
                EditorGUI.BeginChangeCheck();
                bool isSelected = EditorGUILayout.Toggle(new GUIContent(dependency.unityObject.name, dependency.preview), isAlreadySelected);

                if (selectAll)
                {
                    isSelected = true;
                }
                else if (deselectAll)
                {
                    isSelected = false;
                }

                if (EditorGUI.EndChangeCheck() || selectAll || deselectAll)
                {
                    if (isSelected && !isAlreadySelected)
                    {
                        this.selectedDependencies.Add(dependency);
                    }
                    else if (!isSelected && isAlreadySelected)
                    {
                        this.selectedDependencies.Remove(dependency);
                    }
                }
                GUILayout.Label(dependency.unityObject.GetType().Name);
                if (GUILayout.Button(dependency.isPersistent ? "Asset" : "Scene", GUILayout.Width(140)))
                {
                    Selection.activeObject = dependency.unityObject;
                }
                EditorGUILayout.EndHorizontal();
                EditorGUI.EndDisabledGroup();
            }
            EditorGUILayout.EndScrollView();

            if (GUILayout.Button("Add References"))
            {
                AddSelectedReferences();
            }
        }

        public virtual void AddSelectedReferences()
        {
            for (int i = 0; i < this.selectedDependencies.Count; i++)
            {
                var dependency = this.selectedDependencies[i];
                if (dependency.isPersistent)
                {
                    this.assetReferenceResolver.Add(dependency.unityObject);
                    continue;
                }

                Scene? scene = SceneReferenceResolver.GetUnityObjectScene(dependency.unityObject);
                if (scene != null)
                {
                    Scene validScene = (Scene)scene;
                    var sceneReferenceResolver = SceneReferenceResolver.GetReferenceResolver(validScene);
                    if (sceneReferenceResolver != null)
                    {
                        sceneReferenceResolver.Add(dependency.unityObject);
                    }
                }
            }
            RefreshDependencies();
        }

        [MenuItem("GameObject/Bayat/Core/Add Scene Reference(s)")]
        private static void AddSceneReferenceMenuItem()
        {
            Initialize(Selection.objects);
        }

        [MenuItem("GameObject/Bayat/Core/Add Scene Reference(s)", true)]
        private static bool AddSceneReferenceMenuItemValidation()
        {
            if (Selection.objects.Length > 1)
            {
                return true;
            }
            else if (Selection.objects.Length > 0)
            {
                if (!EditorUtility.IsPersistent(Selection.activeObject))
                {
                    return false;
                }
                if (Selection.activeObject == null)
                {
                    return false;
                }
                Scene? scene = SceneReferenceResolver.GetUnityObjectScene(Selection.activeObject);
                if (scene != null)
                {
                    var referenceResolver = SceneReferenceResolver.GetReferenceResolver((Scene)scene);
                    if (!referenceResolver.IsValidUnityObject(Selection.activeObject))
                    {
                        return false;
                    }
                }
                else if (SceneReferenceResolver.Current != null && !SceneReferenceResolver.Current.IsValidUnityObject(Selection.activeObject))
                {
                    return false;
                }
                Type assetType = Selection.activeObject.GetType();
                if (typeof(MonoScript).IsAssignableFrom(assetType) || typeof(UnityEditor.DefaultAsset).IsAssignableFrom(assetType))
                {
                    return false;
                }
                return true;
            }
            return false;
        }

        [MenuItem("GameObject/Bayat/Core/Add Asset Reference(s)")]
        private static void AddAssetReferenceMenuItem()
        {
            Initialize(Selection.objects);
        }

        [MenuItem("GameObject/Bayat/Core/Add Asset Reference(s)", true)]
        private static bool AddAssetReferenceMenuItemValidation()
        {
            if (Selection.objects.Length > 1)
            {
                return true;
            }
            else if (Selection.objects.Length > 0)
            {
                if (!EditorUtility.IsPersistent(Selection.activeObject))
                {
                    return false;
                }
                if (Selection.activeObject == null || !AssetReferenceResolver.CanBeSaved(Selection.activeObject))
                {
                    return false;
                }
                Type assetType = Selection.activeObject.GetType();
                if (typeof(MonoScript).IsAssignableFrom(assetType) || typeof(UnityEditor.DefaultAsset).IsAssignableFrom(assetType))
                {
                    return false;
                }
                return true;
            }
            return false;
        }

        [MenuItem("Assets/Bayat/Core/Add Asset Reference(s)")]
        private static void ProjectAddAssetReferenceMenuItem()
        {
            List<UnityEngine.Object> objectsToAdd = new List<UnityEngine.Object>();
            foreach (var assetGUID in Selection.assetGUIDs)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(assetGUID);
                objectsToAdd.Add(AssetDatabase.LoadAssetAtPath(assetPath, AssetDatabase.GetMainAssetTypeAtPath(assetPath)));
            }
            Initialize(objectsToAdd.ToArray());
        }

        [MenuItem("Assets/Bayat/Core/Add Asset Reference(s)", true)]
        private static bool ProjectAddAssetReferenceMenuItemValidation()
        {
            if (Selection.assetGUIDs.Length > 1)
            {
                return true;
            }
            else if (Selection.assetGUIDs.Length > 0)
            {
                string assetGUID = Selection.assetGUIDs[0];
                string assetPath = AssetDatabase.GUIDToAssetPath(assetGUID);
                UnityEngine.Object obj = AssetDatabase.LoadAssetAtPath(assetPath, AssetDatabase.GetMainAssetTypeAtPath(assetPath));
                if (!EditorUtility.IsPersistent(obj))
                {
                    return false;
                }
                if (obj == null || !AssetReferenceResolver.CanBeSaved(obj))
                {
                    return false;
                }
                Type assetType = obj.GetType();
                if (typeof(MonoScript).IsAssignableFrom(assetType) || typeof(UnityEditor.DefaultAsset).IsAssignableFrom(assetType))
                {
                    return false;
                }
                return !AssetReferenceResolver.Current.Contains(obj);
            }
            return false;
        }

        public class Dependency
        {

            public Texture preview;
            public bool isPersistent;
            public UnityObject unityObject;
            public bool isReferenced;
            public bool missingReferenceResolver;

        }

    }

}