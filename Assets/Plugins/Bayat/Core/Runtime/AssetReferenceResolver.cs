using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using System.IO;

using UnityEditor;
using UnityEditor.SceneManagement;
#endif

using Bayat.Core.Utilities;

using UnityObject = UnityEngine.Object;
using System.Threading;

namespace Bayat.Core
{

#if BAYAT_DEVELOPER
    [CreateAssetMenu(menuName = "Bayat/Core/Asset Reference Resolver")]
#endif
    public class AssetReferenceResolver : ScriptableObject
    {

        private static AssetReferenceResolver current;

        /// <summary>
        /// Gets the current asset reference resolver.
        /// </summary>
        public static AssetReferenceResolver Current
        {
            get
            {
                if (current == null)
                {
                    AssetReferenceResolver instance = null;
#if UNITY_EDITOR
                    string[] instanceGuids = AssetDatabase.FindAssets("t: AssetReferenceResolver");
                    if (instanceGuids.Length > 1)
                    {
                        Debug.LogError("There is more than one AssetReferenceResolver in this project, but there must only be one.");
                        Debug.Log("Deleting other instances of AssetReferenceResolver and keeping only one instance.");
                        Debug.LogFormat("The selected instance is: {0}", AssetDatabase.GUIDToAssetPath(instanceGuids[0]));
                        instance = AssetDatabase.LoadAssetAtPath<AssetReferenceResolver>(AssetDatabase.GUIDToAssetPath(instanceGuids[0]));

                        // Delete other instances
                        for (int i = 1; i < instanceGuids.Length; i++)
                        {
                            string instanceGuid = instanceGuids[i];
                            AssetDatabase.DeleteAsset(AssetDatabase.GUIDToAssetPath(instanceGuid));
                        }
                    }
                    else if (instanceGuids.Length == 0)
                    {
                        Debug.Log("No Asset Reference Resolver instance found, creating a new one at 'Assets/Resources/Bayat/Core'.");
                        instance = ScriptableObject.CreateInstance<AssetReferenceResolver>();
                        System.IO.Directory.CreateDirectory("Assets/Resources/Bayat/Core");
                        AssetDatabase.CreateAsset(instance, "Assets/Resources/Bayat/Core/AssetReferenceResolver.asset");
                        AssetDatabase.SaveAssets();
                        AssetDatabase.Refresh();
                    }
                    else
                    {
                        instance = AssetDatabase.LoadAssetAtPath<AssetReferenceResolver>(AssetDatabase.GUIDToAssetPath(instanceGuids[0]));
                    }
#else
                    instance = Resources.Load<AssetReferenceResolver>("Bayat/Core/AssetReferenceResolver");
#endif
                    current = instance;
                }
                return current;
            }
        }

        [Tooltip("Ignore GameObjects that have any of these tags")]
        [SerializeField]
        protected string[] ignoredTags = new string[] { "EditorOnly" };

        [Tooltip("Include GameObjects that have any of these tags, ignore the others.")]
        [SerializeField]
        protected bool includeIgnoredTags = false;

        [Tooltip("Ignore Assets that have any of these labels (bottom right icon in the asset inspector)")]
        [SerializeField]
        protected string[] ignoredLabels = new string[0];

        [Tooltip("Include Assets that have any of these labels, ignore the others.")]
        [SerializeField]
        protected bool includeIgnoredLabels = false;

        [Tooltip("Whether to ignore static GameObjects")]
        [SerializeField]
        protected bool ignoreStatic = false;
        [SerializeField]
        protected List<string> guids = new List<string>();
        [SerializeField]
        protected List<UnityObject> dependencies = new List<UnityObject>();

        [SerializeField]
        protected GuidToReferenceDictionary guidToReference = new GuidToReferenceDictionary();
        protected ReferenceToGuidDictionary referenceToGuid = new ReferenceToGuidDictionary();

#if UNITY_EDITOR
        [SerializeField]
        protected ReferenceResolverMode mode = ReferenceResolverMode.Auto;
        [SerializeField]
        protected float refreshDependenciesTimeoutInSeconds = 2f;
#endif

        /// <summary>
        /// Gets the GUIDs list.
        /// </summary>
        [Obsolete("This property will be removed in future releases, use GuidToReference or ReferenceToGuid instead.", false)]
        public virtual List<string> Guids
        {
            get
            {
                return this.guids;
            }
        }

        /// <summary>
        /// Gets the project dependencies list.
        /// </summary>
        [Obsolete("This property will be removed in future releases, use GuidToReference or ReferenceToGuid instead.", false)]
        public virtual List<UnityObject> Dependencies
        {
            get
            {
                return this.dependencies;
            }
        }

        public virtual GuidToReferenceDictionary GuidToReference
        {
            get
            {
                if (this.guidToReference == null)
                {
                    this.guidToReference = new GuidToReferenceDictionary();

                    if (this.guids != null)
                    {
                        for (int i = 0; i < this.guids.Count; i++)
                        {
                            this.guidToReference.Add(this.guids[i], this.dependencies[i]);
                        }
                        this.guids.Clear();
                        this.dependencies.Clear();
                    }
                }
                return this.guidToReference;
            }
            set
            {
                this.guidToReference = value;
            }
        }

        public virtual ReferenceToGuidDictionary ReferenceToGuid
        {
            get
            {
                if (this.referenceToGuid == null)
                {
                    this.referenceToGuid = new ReferenceToGuidDictionary();
                }
                if (this.referenceToGuid.Count != this.guidToReference.Count)
                {

                    // Populate the reverse dictionary with the items from the normal dictionary.
                    foreach (var kvp in this.guidToReference)
                    {
                        if (kvp.Value != null)
                        {
                            this.referenceToGuid[kvp.Value] = kvp.Key;
                        }
                    }
                }
                return this.referenceToGuid;
            }
            set
            {
                this.referenceToGuid = value;
            }
        }

#if UNITY_EDITOR
        public virtual ReferenceResolverMode Mode
        {
            get
            {
                return this.mode;
            }
        }
#endif

        public virtual void OnBeforeSerialize()
        {

            // Empty the referenceToGuid so it has to be refreshed.
            this.referenceToGuid = null;
#if UNITY_EDITOR
            // This is called before building or when things are being serialised before pressing play.
            if (BuildPipeline.isBuildingPlayer || (EditorApplication.isPlayingOrWillChangePlaymode && !EditorApplication.isPlaying))
            {
                //RemoveNullReferences();
                RemoveNullValues();
            }
#endif
        }

        public virtual void OnAfterDeserialize()
        {

            // Empty the referenceToGuid so it has to be refreshed.
            this.referenceToGuid = null;
        }

        public virtual void Reset()
        {
#if UNITY_EDITOR
            //if (mode == ReferenceResolverMode.Auto)
            //{
            //    CollectSceneDependencies();
            //}
            Debug.Log("Asset Reference Resolver has been reset");
#endif
        }

#if UNITY_EDITOR
        /// <summary>
        /// Migrates the old reference database to new dictionary based database.
        /// </summary>
        public void MigrateReferenceDatabase()
        {
            SerializedObject serializedObject = new SerializedObject(this);
            for (int i = 0; i < this.guids.Count; i++)
            {
                Add(this.dependencies[i], this.guids[i]);
            }
            this.guids.Clear();
            this.dependencies.Clear();
        }

        /// <summary>
        /// Refreshes the dependencies database and fetches the new ones from the scene.
        /// </summary>
        public virtual void RefreshDependencies()
        {
            RemoveNullValues();

            List<UnityObject> allBuildScenesAssets = new List<UnityObject>();
            Scene activeScene = EditorSceneManager.GetActiveScene();
            for (int i = 0; i < EditorBuildSettings.scenes.Length; i++)
            {
                EditorBuildSettingsScene editorScene = EditorBuildSettings.scenes[i];
                Scene scene;
                if (activeScene.path == editorScene.path)
                {
                    scene = activeScene;
                }
                else
                {
                    scene = EditorSceneManager.OpenScene(editorScene.path, OpenSceneMode.Additive);
                }
                UnityObject[] dependencies = EditorUtility.CollectDependencies(scene.GetRootGameObjects());
                foreach (var sceneDependency in dependencies)
                {
                    if (EditorUtility.IsPersistent(sceneDependency))
                    {
                        allBuildScenesAssets.Add(sceneDependency);
                    }
                }
                if (scene != activeScene)
                {
                    EditorSceneManager.CloseScene(scene, true);
                }
            }

            // Remove Save System Manager from dependency list
            AddDependencies(allBuildScenesAssets.ToArray(), this.refreshDependenciesTimeoutInSeconds);

            MaterialPropertiesResolver.Current.CollectMaterials();

            AssetDatabase.SaveAssets();
        }

        /// <summary>
        /// Adds the specified objects dependencies to the database.
        /// </summary>
        /// <param name="objs"></param>
        /// <param name="timeoutSecs"></param>
        public virtual void AddDependencies(UnityObject[] objs, float timeoutSecs = 2)
        {

            // Empty the referenceToGuid so it has to be refreshed.
            this.referenceToGuid = null;
            float progress = 0;
            float objRate = 1f / objs.Length;
            var startTime = Time.realtimeSinceStartup;
            foreach (var obj in objs)
            {
                if (Time.realtimeSinceStartup - startTime > timeoutSecs)
                {
                    EditorUtility.DisplayDialog("Add Asset References Timeout", string.Format("The process of adding references took more than {0} seconds and has been terminated, if you need to reference the remaining objects, increase the timeout.", timeoutSecs), "Done");
                    break;
                }

                var dependencies = EditorUtility.CollectDependencies(new UnityObject[] { obj });
                float rate = objRate / dependencies.Length;

                foreach (var dependency in dependencies)
                {
                    if (!EditorUtility.IsPersistent(dependency))
                    {
                        continue;
                    }

                    if (dependency == null || !IsValidUnityObject(dependency))
                    {
                        continue;
                    }

                    Type assetType = dependency.GetType();
                    if (typeof(MonoScript).IsAssignableFrom(assetType) || typeof(UnityEditor.DefaultAsset).IsAssignableFrom(assetType))
                    {
                        continue;
                    }
                    progress += rate;

                    Add(dependency);
                }
            }
            Undo.RecordObject(this, "Update Save System Asset Reference List");
        }

        /// <summary>
        /// Removes invalid references.
        /// </summary>
        public virtual void RemoveInvalidReferences()
        {
            var removeGuids = new List<string>();
            foreach (var item in GuidToReference)
            {
                if (!EditorUtility.IsPersistent(item.Value))
                {
                    removeGuids.Add(item.Key);
                    continue;
                }

                if (item.Value == null || !IsValidUnityObject(item.Value))
                {
                    removeGuids.Add(item.Key);
                    continue;
                }
            }
            Undo.RecordObject(this, "Remove Invalid References");
            for (int i = 0; i < removeGuids.Count; i++)
            {
                this.guidToReference.Remove(removeGuids[i]);
            }
            ReferenceToGuid.Clear();

            // Fix for unity serialization
            EditorUtility.SetDirty(this);
        }
#endif

        /// <summary>
        /// Gets the given object reference GUID.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public virtual string Get(UnityObject obj)
        {
            string guid;
            if (!ReferenceToGuid.TryGetValue(obj, out guid))
                return string.Empty;
            return guid;
        }

        /// <summary>
        /// Gets the object associated with the given GUID.
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        public virtual UnityObject Get(string guid)
        {
            if (string.IsNullOrEmpty(guid))
                return null;
            UnityObject obj;
            if (!this.guidToReference.TryGetValue(guid, out obj))
                return null;
            return obj;
        }

        /// <summary>
        /// Adds the object to the database by generating a new GUID if it is not already in the database.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public virtual string Add(UnityObject obj)
        {
            string guid;
            // If it already exists in the list, do nothing.
            if (ReferenceToGuid.TryGetValue(obj, out guid))
                return guid;
            // Add the reference to the Dictionary.
            guid = Guid.NewGuid().ToString("N");
            Add(obj, guid);
#if UNITY_EDITOR
            // Fix for unity serialization
            EditorUtility.SetDirty(this);
#endif
            return guid;
        }

        /// <summary>
        /// Adds the object to the database with the given GUID if it is not empty, otherwise generates a new GUID.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="guid"></param>
        public virtual void Add(UnityObject obj, string guid)
        {

            // If the GUID is null or empty, generate a new GUID.
            if (string.IsNullOrEmpty(guid))
            {
                guid = Guid.NewGuid().ToString("N");
            }

            // Add the reference to the Dictionary or update the existing one.
            guidToReference[guid] = obj;
            ReferenceToGuid[obj] = guid;
#if UNITY_EDITOR
            // Fix for unity serialization
            EditorUtility.SetDirty(this);
#endif
        }

        /// <summary>
        /// Removes the object and its GUID from the database.
        /// </summary>
        /// <param name="obj"></param>
        public virtual void Remove(UnityObject obj)
        {
            string referenceID;

            // Get the reference ID, or do nothing if it doesn't exist.
            if (ReferenceToGuid.TryGetValue(obj, out referenceID))
                return;

            ReferenceToGuid.Remove(obj);
            guidToReference.Remove(referenceID);
#if UNITY_EDITOR
            // Fix for unity serialization
            EditorUtility.SetDirty(this);
#endif
        }

        /// <summary>
        /// Removes the GUID and the associated object from the database.
        /// </summary>
        /// <param name="referenceID"></param>
        public virtual void Remove(string referenceID)
        {
            UnityObject obj;
            // Get the reference ID, or do nothing if it doesn't exist.
            if (!guidToReference.TryGetValue(referenceID, out obj))
                return;

            ReferenceToGuid.Remove(obj);
            guidToReference.Remove(referenceID);
#if UNITY_EDITOR
            // Fix for unity serialization
            EditorUtility.SetDirty(this);
#endif
        }

        /// <summary>
        /// Returns true if the database has null references, otherwise true.
        /// </summary>
        /// <returns></returns>
        public virtual bool HasNullValues()
        {
            var nullKeys = this.guidToReference.Where(pair => pair.Value == null)
                                .Select(pair => pair.Key).ToList();
            return nullKeys.Count > 0;
        }

        /// <summary>
        /// Removes null values and references from the database.
        /// </summary>
        public virtual void RemoveNullValues()
        {
            //var nullKeys = this.guidToReference.Where(pair => pair.Value == null)
            //                    .Select(pair => pair.Key).ToList();
            //foreach (var key in nullKeys)
            //{
            //    this.guidToReference.Remove(key);
            //}
            GuidToReference.RemoveNullValues();
            ReferenceToGuid.RemoveNullValues();
#if UNITY_EDITOR
            // Fix for unity serialization
            EditorUtility.SetDirty(this);
#endif
        }

        /// <summary>
        /// Removes the duplicate values.
        /// </summary>
        public virtual void RemoveDuplicateValues()
        {
            var uniqueDictionary = this.guidToReference.GroupBy(pair => pair.Value)
                         .Select(group => group.First())
                         .ToDictionary(pair => pair.Key, pair => pair.Value);
            this.guidToReference.Clear();
            ReferenceToGuid.Clear();
            foreach (var item in uniqueDictionary)
            {
                this.guidToReference.Add(item.Key, item.Value);
                ReferenceToGuid.Add(item.Value, item.Key);
            }
#if UNITY_EDITOR
            // Fix for unity serialization
            EditorUtility.SetDirty(this);
#endif
        }

        /// <summary>
        /// Clears the database.
        /// </summary>
        public virtual void Clear()
        {
            ReferenceToGuid.Clear();
            this.guidToReference.Clear();
#if UNITY_EDITOR
            // Fix for unity serialization
            EditorUtility.SetDirty(this);
#endif
        }

        /// <summary>
        /// Checks whether the database contains the given object or not.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public virtual bool Contains(UnityObject obj)
        {
            return ReferenceToGuid.ContainsKey(obj);
        }

        /// <summary>
        /// Checks whether the database contains the given GUID or not.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public virtual bool Contains(string referenceID)
        {
            return this.guidToReference.ContainsKey(referenceID);
        }

        /// <summary>
        /// Changes the old GUID to the new GUID in the database.
        /// </summary>
        /// <param name="oldGuid"></param>
        /// <param name="newGuid"></param>
        public virtual void ChangeGuid(string oldGuid, string newGuid)
        {
            this.guidToReference.ChangeKey(oldGuid, newGuid);
            // Empty the referenceToGuid so it has to be refreshed.
            this.referenceToGuid = null;
#if UNITY_EDITOR
            // Fix for unity serialization
            EditorUtility.SetDirty(this);
#endif
        }

#if UNITY_EDITOR
        /// <summary>
        /// Checks whether the object can be saved or not.
        /// </summary>
        /// <param name="obj">The object</param>
        /// <returns>True if can be saved otherwise false</returns>
        public static bool CanBeSaved(UnityObject obj)
        {
            // Check if any of the hide flags determine that it should not be saved.
            if ((((obj.hideFlags & HideFlags.DontSave) == HideFlags.DontSave) ||
                 ((obj.hideFlags & HideFlags.DontSaveInBuild) == HideFlags.DontSaveInBuild) ||
                 ((obj.hideFlags & HideFlags.DontSaveInEditor) == HideFlags.DontSaveInEditor) ||
                 ((obj.hideFlags & HideFlags.HideAndDontSave) == HideFlags.HideAndDontSave)))
            {
                var type = obj.GetType();
                // Meshes are marked with HideAndDontSave, but shouldn't be ignored.
                if (type != typeof(Mesh) && type != typeof(Material))
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// [Editor-only] Checks whether the given <paramref name="unityObject"/> is a valid Unity object to be referenced or not.
        /// </summary>
        /// <remarks>
        /// This method checks the Unity object HideFlags and Tags.
        /// The following HideFlags are considered invalid except for Meshes and Materials:
        /// - <see href="https://docs.unity3d.com/ScriptReference/HideFlags.DontSave.html">HideFlags.DontSave</see>
        /// - <see href="https://docs.unity3d.com/ScriptReference/HideFlags.DontSaveInBuild.html">HideFlags.DontSaveInBuild</see>
        /// - <see href="https://docs.unity3d.com/ScriptReference/HideFlags.DontSaveInEditor.html">HideFlags.DontSaveInEditor</see>
        /// - <see href="https://docs.unity3d.com/ScriptReference/HideFlags.HideAndDontSave.html">HideFlags.HideAndDontSave</see>
        /// And then checks whether the given Unity object is a GameObject, if it is, then uses the <see cref="ignoredTags"/> to determine if the GameObject is valid.
        /// Uses <see cref="HasInvalidTag(GameObject)"/>.
        /// </remarks>
        /// <seealso href="https://docs.unity3d.com/ScriptReference/Object-hideFlags.html"/>
        /// <seealso href="https://docs.unity3d.com/ScriptReference/HideFlags.html"/>
        /// <param name="unityObject">The Unity object to check</param>
        /// <returns>Returns true if the Unity object is valid, otherwise false</returns>
        public virtual bool IsValidUnityObject(UnityObject unityObject)
        {
            if (unityObject == null)
            {
                return false;
            }

            // Check if any of the hide flags determine that it should not be refrenced or serialized.
            if ((((unityObject.hideFlags & HideFlags.DontSave) == HideFlags.DontSave) ||
                 ((unityObject.hideFlags & HideFlags.DontSaveInBuild) == HideFlags.DontSaveInBuild) ||
                 ((unityObject.hideFlags & HideFlags.DontSaveInEditor) == HideFlags.DontSaveInEditor) ||
                 ((unityObject.hideFlags & HideFlags.HideAndDontSave) == HideFlags.HideAndDontSave)))
            {
                var type = unityObject.GetType();

                // Meshes are marked with HideAndDontSave, but shouldn't be ignored.
                if (type != typeof(Mesh) && type != typeof(Material))
                {
                    return false;
                }
            }

            if (HasInvalidLabel(unityObject)) return false;            

            GameObject gameObject = null;
            if (unityObject is GameObject)
            {
                gameObject = unityObject as GameObject;
            }
            if (unityObject is Component)
            {
                gameObject = (unityObject as Component).gameObject;
            }
            if (gameObject != null)
            {
                if (this.ignoreStatic && gameObject.isStatic)
                {
                    return false;
                }
                else
                {
                    return !HasInvalidTag(gameObject);
                }
            }
            return true;
        }

        /// <summary>
        /// Checks whether the given GameObject has any invalid tags determined using <see cref="ignoredTags"/> field.
        /// </summary>
        /// <param name="gameObject">The GameObject to check</param>
        /// <returns>Returns true if the GameObject has any invalid tag, otherwise false.
        /// In case <see cref="includeIgnoredTags"/> is true, returns false when GameObject has "invalid" label and vise versa.</returns>
        public virtual bool HasInvalidTag(GameObject gameObject)
        {
            for (int i = 0; i < this.ignoredTags.Length; i++)
            {
                if (gameObject.CompareTag(this.ignoredTags[i]))
                {
                    return !includeIgnoredTags;
                }
            }
            return includeIgnoredTags;
        }


        /// <summary>
        /// Checks whether the given Asset has any invalid labels (they're located at the bottom of the asset's inspector view) inspector view),
        /// determined using <see cref="ignoredLabels"/> field.
        /// </summary>
        /// <param name="unityObject">The Asset to check</param>
        /// <returns>Returns true if the Asset has any invalid labels, otherwise false. 
        /// In case <see cref="includeIgnoredLabels"/> is true, returns false when Asset has "invalid" label and vise versa.</returns>
        public bool HasInvalidLabel(UnityObject unityObject)
        {
            if (ignoredLabels.Length < 1) return false;

            var objLabels = AssetDatabase.GetLabels(unityObject);
            return objLabels.Any(label => ignoredLabels.Contains(label, StringComparer.InvariantCultureIgnoreCase)) ^ includeIgnoredLabels;
        }


        [InitializeOnLoadMethod]
        public static void UpdateLocation()
        {
            string path = AssetDatabase.GetAssetPath(AssetReferenceResolver.Current);
            if (path.StartsWith("Assets/Plugins/Bayat"))
            {
                if (EditorUtility.DisplayDialog("Updating AssetReferenceResolver Location",
                    "The save system requires the AssetReferenceResolver to reside at Assets/Resources/Bayat/Core folder, " +
                    "press Update to update and move the AssetReferenceResolver to the desired folder.", "Update", "Cancel"))
                {
                    if (!Directory.Exists("Assets/Resources"))
                    {
                        AssetDatabase.CreateFolder("Assets", "Resources");
                    }
                    if (!Directory.Exists("Assets/Resources/Bayat"))
                    {
                        AssetDatabase.CreateFolder("Assets/Resources", "Bayat");
                    }
                    if (!Directory.Exists("Assets/Resources/Bayat/Core"))
                    {
                        AssetDatabase.CreateFolder("Assets/Resources/Bayat", "Core");
                    }
                    string message = AssetDatabase.MoveAsset(path, "Assets/Resources/Bayat/Core/AssetReferenceResolver.asset");
                    if (string.IsNullOrEmpty(message))
                    {
                        Debug.Log("The Asset Reference Resolver has been moved to 'Assets/Resources/Bayat/Core' successfully");
                    }
                    else
                    {
                        Debug.LogError(message);
                    }
                }
            }
        }
#endif

        //#if UNITY_EDITOR
        //        /// <summary>
        //        /// Collects the default dependencies.
        //        /// </summary>
        //        public virtual void CollectDefaultDependencies()
        //        {
        //            bool undoRecorded = false;

        //            // Remove NULL values from Dictionary.
        //            if (RemoveNullReferences() > 0)
        //            {
        //                Undo.RecordObject(this, "Update AssetReferenceResolver List");
        //                undoRecorded = true;
        //            }

        //            UnityObject[] builtInExtraAssets = AssetDatabase.LoadAllAssetsAtPath("Resources/unity_builtin_extra");
        //            UnityObject[] defaultAssets = AssetDatabase.LoadAllAssetsAtPath("Library/unity default resources");
        //            List<UnityObject> allAssets = new List<UnityObject>();
        //            for (int i = 0; i < builtInExtraAssets.Length; i++)
        //            {
        //                UnityObject assetObj = builtInExtraAssets[i];
        //                if (assetObj == null || allAssets.Contains(assetObj))
        //                {
        //                    continue;
        //                }
        //                allAssets.Add(assetObj);
        //            }
        //            for (int i = 0; i < defaultAssets.Length; i++)
        //            {
        //                UnityObject assetObj = defaultAssets[i];
        //                if (assetObj == null || allAssets.Contains(assetObj))
        //                {
        //                    continue;
        //                }
        //                allAssets.Add(assetObj);
        //            }

        //            foreach (var asset in allAssets)
        //            {
        //                if (asset == null || !CanBeSaved(asset))
        //                {
        //                    continue;
        //                }
        //                Type assetType = asset.GetType();
        //                if (typeof(MonoScript).IsAssignableFrom(assetType) || typeof(UnityEditor.DefaultAsset).IsAssignableFrom(assetType))
        //                {
        //                    continue;
        //                }

        //                // If we're adding a new item to the type list, make sure we've recorded an undo for the object.
        //                if (string.IsNullOrEmpty(ResolveGuid(asset)))
        //                {
        //                    if (!undoRecorded)
        //                    {
        //                        Undo.RecordObject(this, "Update AssetReferenceResolver List");
        //                        undoRecorded = true;
        //                    }
        //                    string guid = Guid.NewGuid().ToString("N");
        //                    Add(guid, asset);
        //                }
        //            }

        //            MaterialPropertiesResolver.Current.CollectMaterials();
        //        }

        //        /// <summary>
        //        /// Collects the project dependencies from build scenes.
        //        /// </summary>
        //        public virtual void CollectProjectDependencies()
        //        {
        //            bool undoRecorded = false;

        //            // Remove NULL values from Dictionary.
        //            if (RemoveNullReferences() > 0)
        //            {
        //                Undo.RecordObject(this, "Update AssetReferenceResolver List");
        //                undoRecorded = true;
        //            }

        //            List<UnityObject> allAssets = new List<UnityObject>();
        //            Scene activeScene = EditorSceneManager.GetActiveScene();
        //            for (int i = 0; i < EditorBuildSettings.scenes.Length; i++)
        //            {
        //                EditorBuildSettingsScene editorScene = EditorBuildSettings.scenes[i];
        //                Scene scene;
        //                if (activeScene.path == editorScene.path)
        //                {
        //                    scene = activeScene;
        //                }
        //                else
        //                {
        //                    scene = EditorSceneManager.OpenScene(editorScene.path, OpenSceneMode.Additive);
        //                }
        //                UnityObject[] dependencies = EditorUtility.CollectDependencies(scene.GetRootGameObjects());
        //                foreach (var sceneDependency in dependencies)
        //                {
        //                    if (EditorUtility.IsPersistent(sceneDependency))
        //                    {
        //                        allAssets.Add(sceneDependency);
        //                    }
        //                }
        //                if (scene != activeScene)
        //                {
        //                    EditorSceneManager.CloseScene(scene, true);
        //                }
        //            }

        //            foreach (var asset in allAssets)
        //            {
        //                if (asset == null || !CanBeSaved(asset))
        //                {
        //                    continue;
        //                }

        //                Type assetType = asset.GetType();
        //                if (typeof(MonoScript).IsAssignableFrom(assetType) || typeof(UnityEditor.DefaultAsset).IsAssignableFrom(assetType))
        //                {
        //                    continue;
        //                }

        //                // If we're adding a new item to the type list, make sure we've recorded an undo for the object.
        //                if (string.IsNullOrEmpty(ResolveGuid(asset)))
        //                {
        //                    if (!undoRecorded)
        //                    {
        //                        Undo.RecordObject(this, "Update AssetReferenceResolver List");
        //                        undoRecorded = true;
        //                    }
        //                    string guid = Guid.NewGuid().ToString("N");
        //                    Add(guid, asset);
        //                }
        //            }

        //            MaterialPropertiesResolver.Current.CollectMaterials();
        //        }

        //        /// <summary>
        //        /// Collects the active scene dependencies from build scenes.
        //        /// </summary>
        //        public virtual void CollectActiveSceneDependencies()
        //        {
        //            bool undoRecorded = false;

        //            // Remove NULL values from Dictionary.
        //            if (RemoveNullReferences() > 0)
        //            {
        //                Undo.RecordObject(this, "Update AssetReferenceResolver List");
        //                undoRecorded = true;
        //            }

        //            List<UnityObject> allAssets = new List<UnityObject>();
        //            Scene activeScene = EditorSceneManager.GetActiveScene();
        //            UnityObject[] dependencies = EditorUtility.CollectDependencies(activeScene.GetRootGameObjects());
        //            foreach (var sceneDependency in dependencies)
        //            {
        //                if (EditorUtility.IsPersistent(sceneDependency))
        //                {
        //                    allAssets.Add(sceneDependency);
        //                }
        //            }

        //            foreach (var asset in allAssets)
        //            {
        //                if (asset == null || !CanBeSaved(asset))
        //                {
        //                    continue;
        //                }

        //                Type assetType = asset.GetType();
        //                if (typeof(MonoScript).IsAssignableFrom(assetType) || typeof(UnityEditor.DefaultAsset).IsAssignableFrom(assetType))
        //                {
        //                    continue;
        //                }

        //                // If we're adding a new item to the type list, make sure we've recorded an undo for the object.
        //                if (string.IsNullOrEmpty(ResolveGuid(asset)))
        //                {
        //                    if (!undoRecorded)
        //                    {
        //                        Undo.RecordObject(this, "Update AssetReferenceResolver List");
        //                        undoRecorded = true;
        //                    }
        //                    string guid = Guid.NewGuid().ToString("N");
        //                    Add(guid, asset);
        //                }
        //            }

        //            MaterialPropertiesResolver.Current.CollectMaterials();
        //        }

        //        /// <summary>
        //        /// Collects the whole project dependencies.
        //        /// </summary>
        //        public virtual void CollectAllProjectDependencies()
        //        {
        //            CollectDefaultDependencies();
        //            bool undoRecorded = false;

        //            // Remove NULL values from Dictionary.
        //            if (RemoveNullReferences() > 0)
        //            {
        //                Undo.RecordObject(this, "Update AssetReferenceResolver List");
        //                undoRecorded = true;
        //            }

        //            string[] assetGuids = AssetDatabase.FindAssets("t: Object");
        //            List<UnityObject> allAssets = new List<UnityObject>();

        //            for (int i = 0; i < assetGuids.Length; i++)
        //            {
        //                string assetGuid = assetGuids[i];
        //                string assetPath = AssetDatabase.GUIDToAssetPath(assetGuid);
        //                UnityObject assetObj = AssetDatabase.LoadAssetAtPath<UnityObject>(assetPath);
        //                if (assetObj == null || allAssets.Contains(assetObj))
        //                {
        //                    continue;
        //                }
        //                allAssets.Add(assetObj);
        //            }

        //            foreach (var asset in allAssets)
        //            {
        //                if (asset == null || !CanBeSaved(asset))
        //                {
        //                    continue;
        //                }
        //                Type assetType = asset.GetType();
        //                if (typeof(MonoScript).IsAssignableFrom(assetType) || typeof(UnityEditor.DefaultAsset).IsAssignableFrom(assetType))
        //                {
        //                    continue;
        //                }

        //                // If we're adding a new item to the type list, make sure we've recorded an undo for the object.
        //                if (string.IsNullOrEmpty(ResolveGuid(asset)))
        //                {
        //                    if (!undoRecorded)
        //                    {
        //                        Undo.RecordObject(this, "Update AssetReferenceResolver List");
        //                        undoRecorded = true;
        //                    }
        //                    string guid = Guid.NewGuid().ToString("N");
        //                    Add(guid, asset);
        //                }
        //            }

        //            MaterialPropertiesResolver.Current.CollectMaterials();
        //        }
        //#endif

        //        /// <summary>
        //        /// Checks whether if has any null references or not.
        //        /// </summary>
        //        /// <returns>True if has null references otherwise false</returns>
        //        public virtual bool HasNullReferences()
        //        {
        //            int index = 0;
        //            while (index < this.guids.Count)
        //            {
        //                if (this.dependencies[index] == null)
        //                {
        //                    return true;
        //                }
        //                else
        //                {
        //                    index++;
        //                }
        //            }
        //            return false;
        //        }

        //        /// <summary>
        //        /// Removes the null references.
        //        /// </summary>
        //        /// <returns>The count of removed references</returns>
        //        public virtual int RemoveNullReferences()
        //        {
        //            int index = 0;
        //            int removedCount = 0;
        //            while (index < this.guids.Count)
        //            {
        //                if (this.dependencies[index] == null)
        //                {
        //                    this.guids.RemoveAt(index);
        //                    this.dependencies.RemoveAt(index);
        //                    removedCount++;
        //                }
        //                else
        //                {
        //                    index++;
        //                }
        //            }
        //            return removedCount;
        //        }

        //        /// <summary>
        //        /// Checks whether contains the GUID or not.
        //        /// </summary>
        //        /// <param name="guid">The GUID</param>
        //        /// <returns>True if has GUID otherwise false</returns>
        //        public virtual bool Contains(string guid)
        //        {
        //            return this.guids.Contains(guid);
        //        }

        //        /// <summary>
        //        /// Checks whether contains the object or not.
        //        /// </summary>
        //        /// <param name="obj">The object</param>
        //        /// <returns>True if has object otherwise false</returns>
        //        public virtual bool Contains(UnityObject obj)
        //        {
        //            return this.dependencies.Contains(obj);
        //        }

        //        /// <summary>
        //        /// Resolves the GUID and gets the object associated to it.
        //        /// </summary>
        //        /// <param name="guid">The GUID</param>
        //        /// <returns>The object associated to this GUID</returns>
        //        public virtual UnityObject ResolveReference(string guid)
        //        {
        //            int index = this.guids.IndexOf(guid);
        //            if (index == -1)
        //            {
        //                return null;
        //            }
        //            else
        //            {
        //                return this.dependencies[index];
        //            }
        //        }

        //        /// <summary>
        //        /// Resolves the object and gets the GUID associated to it.
        //        /// </summary>
        //        /// <param name="obj"></param>
        //        /// <returns>The GUID associated to the object</returns>
        //        public virtual string ResolveGuid(UnityObject obj)
        //        {
        //            int index = this.dependencies.IndexOf(obj);
        //            if (index == -1)
        //            {
        //                return null;
        //            }
        //            else
        //            {
        //                return this.guids[index];
        //            }
        //        }

        //        /// <summary>
        //        /// Adds a new reference.
        //        /// </summary>
        //        /// <param name="guid">The guid</param>
        //        /// <param name="obj">The object</param>
        //        /// <returns>The GUID or the generated GUID if the given GUID is null or empty</returns>
        //        public virtual string Add(string guid, UnityObject obj)
        //        {
        //            if (this.dependencies.Contains(obj))
        //            {
        //                return null;
        //            }
        //            if (this.guids.Contains(guid))
        //            {
        //                return null;
        //            }
        //            string newGuid = guid;
        //            if (string.IsNullOrEmpty(guid))
        //            {
        //                newGuid = Guid.NewGuid().ToString("N");
        //                this.guids.Add(newGuid);
        //            }
        //            else
        //            {
        //                this.guids.Add(guid);
        //            }
        //            this.dependencies.Add(obj);
        //            return newGuid;
        //        }

        //        /// <summary>
        //        /// Adds a new object reference by generating a new GUID.
        //        /// </summary>
        //        /// <param name="obj"></param>
        //        /// <returns>The generated GUID</returns>
        //        public virtual string Add(UnityObject obj)
        //        {
        //            if (this.dependencies.Contains(obj))
        //            {
        //                return null;
        //            }
        //            Guid guid = Guid.NewGuid();
        //            this.guids.Add(guid.ToString("N"));
        //            this.dependencies.Add(obj);
        //            return guid.ToString("N");
        //        }

    }

}
