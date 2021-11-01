using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

using Bayat.Core.Utilities;

using UnityObject = UnityEngine.Object;

namespace Bayat.Core
{

    /// <summary>
    /// The scene reference resolver.
    /// </summary>
    [AddComponentMenu("Bayat/Core/Scene Reference Resolver")]
    [DisallowMultipleComponent]
    public class SceneReferenceResolver : MonoBehaviour, ISerializationCallbackReceiver
    {

        protected static Dictionary<Scene, SceneReferenceResolver> referenceResolvers = new Dictionary<Scene, SceneReferenceResolver>();

        private static SceneReferenceResolver current;

        /// <summary>
        /// Gets the current scene reference resolver.
        /// </summary>
        public static SceneReferenceResolver Current
        {
            get
            {
                if (current == null)
                {
                    SceneReferenceResolver[] instances = FindObjectsOfType<SceneReferenceResolver>();
#if !UNITY_EDITOR
                    if (instances.Length == 0)
                    {
                        CreateNewInstance();
                    }
#endif
                    if (instances.Length == 1)
                    {
                        current = instances[0];
                    }
                    else if (instances.Length > 1)
                    {
                        throw new InvalidOperationException("There is more than one SceneReferenceResolver in this scene, but there must only be one.");
                    }
                }
                return current;
            }
        }

        /// <summary>
        /// A dictionary of all the available Scene Reference Resolvers based on the Scene they are managing.
        /// </summary>
        /// <remarks>
        /// Use <see cref="GetReferenceResolver(Scene)"/> to get the appropriate reference resolver for a scene, it works on both Editor and Play mode but this property does not.
        /// </remarks>
        public static Dictionary<Scene, SceneReferenceResolver> ReferenceResolvers
        {
            get
            {
                return referenceResolvers;
            }
        }

        [SerializeField]
        protected string[] invalidGameObjectTags = new string[] { "EditorOnly" };
        [HideInInspector]
        [SerializeField]
        protected List<string> guids = new List<string>();
        [HideInInspector]
        [SerializeField]
        protected List<UnityEngine.Object> sceneDependencies = new List<UnityEngine.Object>();

        [SerializeField]
        protected GuidToReferenceDictionary guidToReference = new GuidToReferenceDictionary();
        protected ReferenceToGuidDictionary referenceToGuid = new ReferenceToGuidDictionary();

#if UNITY_EDITOR
        [SerializeField]
        [Obsolete("This property will be removed in future releases, use GuidToReference or ReferenceToGuid instead.", false)]
        protected List<UnityEngine.Object> availableDependencies = new List<UnityEngine.Object>();
        [SerializeField]
        protected ReferenceResolverMode mode = ReferenceResolverMode.Auto;
        [SerializeField]
        protected bool updateOnSceneSaving = true;
        [SerializeField]
        protected bool updateOnEnteringPlayMode = true;
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
        /// Gets the scene dependencies list.
        /// </summary>
        [Obsolete("This property will be removed in future releases, use GuidToReference or ReferenceToGuid instead.", false)]
        public virtual List<UnityEngine.Object> SceneDependencies
        {
            get
            {
                return this.sceneDependencies;
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
                            this.guidToReference.Add(this.guids[i], this.sceneDependencies[i]);
                        }
                        this.guids.Clear();
                        this.sceneDependencies.Clear();
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
        /// <summary>
        /// Gets the available scene dependencies list.
        /// </summary>
        [Obsolete("This property will be removed in future releases.", false)]
        public virtual List<UnityEngine.Object> AvailableDependencies
        {
            get
            {
                return this.availableDependencies;
            }
        }

        public virtual ReferenceResolverMode Mode
        {
            get
            {
                return this.mode;
            }
        }

        public virtual bool UpdateOnSceneSaving
        {
            get
            {
                return this.updateOnSceneSaving;
            }
        }

        public virtual bool UpdateOnEnteringPlayMode
        {
            get
            {
                return this.updateOnEnteringPlayMode;
            }
        }
#endif

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        protected virtual void OnEnable()
        {
            if (referenceResolvers.ContainsValue(this))
            {
                return;
            }
            referenceResolvers.Add(this.gameObject.scene, this);
        }

        protected virtual void OnDisable()
        {
            referenceResolvers.Remove(this.gameObject.scene);
        }

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
            Debug.Log("Scene Reference Resolver has been reset");
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
                Add(this.sceneDependencies[i], this.guids[i]);
            }
            this.guids.Clear();
            this.sceneDependencies.Clear();
        }

        /// <summary>
        /// Refreshes the dependencies database and fetches the new ones from the scene.
        /// </summary>
        public virtual void RefreshDependencies()
        {
            RemoveNullValues();

            var gos = EditorSceneManager.GetActiveScene().GetRootGameObjects();

            // Remove Save System Manager from dependency list
            AddDependencies(gos, this.refreshDependenciesTimeoutInSeconds);


            MaterialPropertiesResolver.Current.CollectMaterials();

            AssetDatabase.SaveAssets();
        }

        /// <summary>
        /// Adds the specified objects dependencies to the database.
        /// </summary>
        /// <param name="objs"></param>
        /// <param name="timeoutSecs"></param>
        public virtual void AddDependencies(UnityEngine.Object[] objs, float timeoutSecs = 2)
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
                    EditorUtility.DisplayDialog("Add Scene References Timeout", string.Format("The process of adding references took more than {0} seconds and has been terminated, if you need to reference the remaining objects, increase the timeout.", timeoutSecs), "Done");
                    break;
                }

                var dependencies = EditorUtility.CollectDependencies(new UnityEngine.Object[] { obj });
                float rate = objRate / dependencies.Length;

                foreach (var dependency in dependencies)
                {
                    if (EditorUtility.IsPersistent(dependency))
                    {
                        continue;
                    }

                    if (dependency == null || !IsValidUnityObject(dependency) || AssetReferenceResolver.Current.Contains(dependency) || Contains(dependency))
                    {
                        continue;
                    }
                    progress += rate;

                    Add(dependency);
                }
            }

            AssetReferenceResolver.Current.AddDependencies(objs);
            Undo.RecordObject(this, "Update Save System Scene Reference List");
        }
#endif

        /// <summary>
        /// Gets the given object reference GUID.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public virtual string Get(UnityEngine.Object obj)
        {
            string guid;
            if (!this.ReferenceToGuid.TryGetValue(obj, out guid))
                return string.Empty;
            return guid;
        }

        /// <summary>
        /// Gets the object associated with the given GUID.
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        public virtual UnityEngine.Object Get(string guid)
        {
            if (string.IsNullOrEmpty(guid))
                return null;
            UnityEngine.Object obj;
            if (!this.guidToReference.TryGetValue(guid, out obj))
                return null;
            return obj;
        }

        /// <summary>
        /// Adds the object to the database by generating a new GUID if it is not already in the database.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public virtual string Add(UnityEngine.Object obj)
        {
            string guid;
            // If it already exists in the list, do nothing.
            if (this.ReferenceToGuid.TryGetValue(obj, out guid))
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
        public virtual void Add(UnityEngine.Object obj, string guid)
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
        public virtual void Remove(UnityEngine.Object obj)
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
            UnityEngine.Object obj;
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
            this.GuidToReference.RemoveNullValues();
            this.ReferenceToGuid.RemoveNullValues();
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
            this.ReferenceToGuid.Clear();
            foreach (var item in uniqueDictionary)
            {
                this.guidToReference.Add(item.Key, item.Value);
                this.ReferenceToGuid.Add(item.Value, item.Key);
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
            this.ReferenceToGuid.Clear();
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
        public virtual bool Contains(UnityEngine.Object obj)
        {
            return this.ReferenceToGuid.ContainsKey(obj);
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
        public static bool CanBeSaved(UnityEngine.Object obj)
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
        /// And then checks whether the given Unity object is a GameObject, if it is, then uses the <see cref="invalidGameObjectTags"/> to determine if the GameObject is valid.
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
            if (unityObject is GameObject)
            {
                var gameObject = unityObject as GameObject;
                return !HasInvalidTag(gameObject);
            }
            if (unityObject is Component)
            {
                var gameObject = (unityObject as Component).gameObject;
                return !HasInvalidTag(gameObject);
            }
            return true;
        }

        /// <summary>
        /// Checks whether the given GameObject has any invalid tags determined using <see cref="invalidGameObjectTags"/> field.
        /// </summary>
        /// <param name="gameObject">The GameObject to check</param>
        /// <returns>Returns true if the GameObject has any invalid tag, otherwise false</returns>
        public virtual bool HasInvalidTag(GameObject gameObject)
        {
            for (int i = 0; i < this.invalidGameObjectTags.Length; i++)
            {
                if (gameObject.CompareTag(invalidGameObjectTags[i]))
                {
                    return true;
                }
            }
            return false;
        }
#endif

        /// <summary>
        /// Creates a new instance of <see cref="SceneReferenceResolver"/> in the current scene.
        /// </summary>
        public static GameObject CreateNewInstance()
        {
            GameObject go = new GameObject("Scene Reference Resolver");
            current = go.AddComponent<SceneReferenceResolver>();
            return go;
        }

#if UNITY_EDITOR
        [MenuItem("GameObject/Bayat/Core/Scene Reference Resolver", false, 10)]
        public static void CreateNewInstanceMenu()
        {
            if (Current != null)
            {
                return;
            }
            Selection.activeGameObject = CreateNewInstance();
        }

        [MenuItem("GameObject/Bayat/Core/Scene Reference Resolver", true)]
        public static bool CreateNewInstanceValidation()
        {
            return current == null;
        }
#endif

        /// <summary>
        /// Gets the reference resolver for the specified <paramref name="scene"/>.
        /// </summary>
        /// <param name="scene">The scene to look for the reference resolver</param>
        /// <returns>Returns the reference resolver in the provided scene if available, otherwise null</returns>
        public static SceneReferenceResolver GetReferenceResolver(Scene scene)
        {
            if (Application.isPlaying)
            {
                referenceResolvers.TryGetValue(scene, out SceneReferenceResolver referenceResolver);
                return referenceResolver;
            }
            else
            {
                return Array.Find(FindObjectsOfType<SceneReferenceResolver>(), x =>
                {
                    return x.gameObject.scene == scene;
                });
            }
        }

        /// <summary>
        /// Gets the Unity object from the specified <paramref name="scene"/>'s reference resolver if available.
        /// </summary>
        /// <param name="scene">The scene to look for the reference resolver</param>
        /// <param name="guid">The referenced Unity object GUID</param>
        /// <returns>Returns the referenced Unity object if available, otherwise null</returns>
        public static UnityObject GetSceneUnityObjectReference(Scene scene, string guid)
        {
            if (string.IsNullOrEmpty(guid))
            {
                throw new ArgumentNullException(nameof(guid));
            }
            var referenceResolver = GetReferenceResolver(scene);
            if (referenceResolver == null)
            {
                return null;
            }
            return referenceResolver.Get(guid);
        }

        /// <summary>
        /// Gets the referenced <paramref name="unityObject"/> GUID from the specified <paramref name="scene"/>'s reference resolver if available.
        /// </summary>
        /// <param name="scene">The scene to look for the reference resolver</param>
        /// <param name="unityObject">The referenced Unity object</param>
        /// <returns>Returns the referenced <paramref name="unityObject"/> GUID's if available, otherwise null</returns>
        public static string GetSceneGuidReference(Scene scene, UnityObject unityObject)
        {
            if (unityObject == null)
            {
                throw new ArgumentNullException(nameof(unityObject));
            }
            var referenceResolver = GetReferenceResolver(scene);
            if (referenceResolver == null)
            {
                return null;
            }
            return referenceResolver.Get(unityObject);
        }

        /// <summary>
        /// Searches through all reference resolvers and gets the Unity object by the specified <paramref name="guid"/>.
        /// </summary>
        /// <param name="guid">The referenced Unity object GUID</param>
        /// <returns>Returns the referenced Unity object if available, otherwise null</returns>
        public static UnityObject GetAnyUnityObjectReference(string guid)
        {
            if (Application.isPlaying)
            {
                UnityObject unityObject;
                foreach (var pair in referenceResolvers)
                {
                    unityObject = pair.Value.Get(guid);
                    if (unityObject != null)
                    {
                        return unityObject;
                    }
                }
                return null;
            }
            else
            {
                UnityObject unityObject;
                foreach (var referenceResolver in FindObjectsOfType<SceneReferenceResolver>())
                {
                    unityObject = referenceResolver.Get(guid);
                    if (unityObject != null)
                    {
                        return unityObject;
                    }
                }
                return null;
            }
        }

        /// <summary>
        /// Searches through all reference resolvers and gets the referenced <paramref name="unityObject"/>'s GUID.
        /// </summary>
        /// <param name="unityObject">The referenced Unity object</param>
        /// <returns>Returns the referenced <paramref name="unityObject"/> GUID's if available, otherwise null</returns>
        public static string GetAnyGuidReference(UnityObject unityObject)
        {
            if (Application.isPlaying)
            {
                string guid;
                foreach (var pair in referenceResolvers)
                {
                    guid = pair.Value.Get(unityObject);
                    if (!string.IsNullOrEmpty(guid))
                    {
                        return guid;
                    }
                }
                return null;
            }
            else
            {
                string guid;
                foreach (var referenceResolver in FindObjectsOfType<SceneReferenceResolver>())
                {
                    guid = referenceResolver.Get(unityObject);
                    if (!string.IsNullOrEmpty(guid))
                    {
                        return guid;
                    }
                }
                return null;
            }
        }

        /// <summary>
        /// Gets the Unity object's scene.
        /// </summary>
        /// <remarks>
        /// First checks if the provided Unity object is a GameObject or Component, 
        /// then uses the <see href="https://docs.unity3d.com/ScriptReference/GameObject-scene.html">GameObject.scene</see> property to retrieve the scene the Unity object is residing at.
        /// </remarks>
        /// <param name="unityObject">The Unity object</param>
        /// <returns>Returns the Unity object's scene if it is a GameObject or Component, otherwise null</returns>
        public static Scene? GetUnityObjectScene(UnityObject unityObject)
        {
            if (unityObject is GameObject)
            {
                return (unityObject as GameObject).scene;
            }
            else if (unityObject is Component)
            {
                return (unityObject as Component).gameObject.scene;
            }
            return null;
        }

        /// <summary>
        /// Gets all referenced Unity objects from all the currently available reference resolvers.
        /// </summary>
        /// <remarks>
        /// It retrieves the referenced Unity objects from the currently open scenes that have a <see cref="SceneReferenceResolver"/> in them.
        /// </remarks>
        /// <returns>Returns a List of currently referenced Unity objecs from the available reference resolvers</returns>
        public static List<UnityObject> GetAllUnityObjects()
        {
            if (Application.isPlaying)
            {
                List<UnityObject> unityObjects = new List<UnityObject>();
                foreach (var pair in referenceResolvers)
                {
                    foreach (var reference in pair.Value.GuidToReference)
                    {
                        if (!unityObjects.Contains(reference.Value))
                        {
                            unityObjects.Add(reference.Value);
                        }
                    }
                }
                return unityObjects;
            }
            else
            {
                List<UnityObject> unityObjects = new List<UnityObject>();
                foreach (var referenceResolver in FindObjectsOfType<SceneReferenceResolver>())
                {
                    foreach (var reference in referenceResolver.GuidToReference)
                    {
                        if (!unityObjects.Contains(reference.Value))
                        {
                            unityObjects.Add(reference.Value);
                        }
                    }
                }
                return unityObjects;
            }
        }

#if UNITY_EDITOR
        /// <summary>
        /// Adds the specified objects and their dependencies to the reference resolver.
        /// </summary>
        /// <param name="objs"></param>
        //public virtual void AddDependencies(UnityEngine.Object[] objs)
        //{
        //    bool assetsModified = false;
        //    foreach (var obj in objs)
        //    {
        //        var dependencies = EditorUtility.CollectDependencies(new UnityEngine.Object[] { obj });

        //        foreach (var dependency in dependencies)
        //        {
        //            if (EditorUtility.IsPersistent(dependency))
        //            {
        //                if (!AssetReferenceResolver.Current.Contains(dependency))
        //                {
        //                    AssetReferenceResolver.Current.Add(dependency);
        //                }
        //                continue;
        //            }

        //            if (dependency == null || !CanBeSaved(dependency) || AssetReferenceResolver.Current.Contains(dependency))
        //            {
        //                continue;
        //            }

        //            AddReference(dependency);
        //        }
        //    }
        //    if (assetsModified)
        //    {
        //        AssetDatabase.SaveAssets();
        //    }
        //    Undo.RecordObject(this, "Update SceneReferenceResolver List");
        //}

        ///// <summary>
        ///// Collects the scene dependencies.
        ///// </summary>
        //public virtual void CollectSceneDependencies()
        //{
        //    bool undoRecorded = false;

        //    // Remove NULL values from Dictionary.
        //    if (RemoveNullReferences() > 0)
        //    {
        //        Undo.RecordObject(this, "Update SceneReferenceResolver List");
        //        undoRecorded = true;
        //    }

        //    var sceneObjects = this.gameObject.scene.GetRootGameObjects();
        //    var dependencies = EditorUtility.CollectDependencies(sceneObjects);
        //    //var deepHierarchy = new List<UnityEngine.Object>(EditorUtility.CollectDeepHierarchy(sceneObjects));
        //    bool assetsModified = false;

        //    for (int i = 0; i < dependencies.Length; i++)
        //    {
        //        var obj = dependencies[i];

        //        if (EditorUtility.IsPersistent(obj))
        //        {
        //            if (!AssetReferenceResolver.Current.Contains(obj))
        //            {
        //                AssetReferenceResolver.Current.Add(obj);
        //                assetsModified = true;
        //            }
        //            continue;
        //        }

        //        if (obj == null || !CanBeSaved(obj) || AssetReferenceResolver.Current.Contains(obj))
        //        {
        //            continue;
        //        }

        //        // If we're adding a new item to the type list, make sure we've recorded an undo for the object.
        //        if (string.IsNullOrEmpty(ResolveGuid(obj)))
        //        {
        //            if (!undoRecorded)
        //            {
        //                Undo.RecordObject(this, "Update SceneReferenceResolver List");
        //                undoRecorded = true;
        //            }
        //            AddReference(obj);
        //        }
        //    }

        //    MaterialPropertiesResolver.Current.CollectMaterials();
        //    if (assetsModified)
        //    {
        //        AssetDatabase.SaveAssets();
        //    }
        //    GetAvailableDependencies();
        //}

        //public virtual void GetAvailableDependencies()
        //{
        //    var availableSceneDependencies = new List<UnityEngine.Object>();
        //    var sceneObjects = this.gameObject.scene.GetRootGameObjects();
        //    var dependencies = EditorUtility.CollectDependencies(sceneObjects);
        //    foreach (var dependency in dependencies)
        //    {
        //        if (EditorUtility.IsPersistent(dependency) || this.sceneDependencies.Contains(dependency))
        //        {
        //            continue;
        //        }
        //        availableSceneDependencies.Add(dependency);
        //    }
        //    this.availableDependencies = availableSceneDependencies;
        //}
#endif

        ///// <summary>
        ///// Checks whether if has any null references or not.
        ///// </summary>
        ///// <returns>True if has null references otherwise false</returns>
        //public virtual bool HasNullReferences()
        //{
        //    int index = 0;
        //    while (index < this.guids.Count)
        //    {
        //        if (this.sceneDependencies[index] == null)
        //        {
        //            return true;
        //        }
        //        else
        //        {
        //            index++;
        //        }
        //    }
        //    return false;
        //}

        ///// <summary>
        ///// Removes the null references.
        ///// </summary>
        ///// <returns>The count of removed references</returns>
        //public virtual int RemoveNullReferences()
        //{
        //    int index = 0;
        //    int removedCount = 0;
        //    while (index < this.guids.Count)
        //    {
        //        if (this.sceneDependencies[index] == null)
        //        {
        //            this.guids.RemoveAt(index);
        //            this.sceneDependencies.RemoveAt(index);
        //            removedCount++;
        //        }
        //        else
        //        {
        //            index++;
        //        }
        //    }
        //    return removedCount;
        //}

        ///// <summary>
        ///// Checks whether contains the GUID or not.
        ///// </summary>
        ///// <param name="guid">The GUID</param>
        ///// <returns>True if has GUID otherwise false</returns>
        //public virtual bool Contains(string guid)
        //{
        //    return this.guids.Contains(guid);
        //}

        ///// <summary>
        ///// Checks whether contains the object or not.
        ///// </summary>
        ///// <param name="obj">The scene object</param>
        ///// <returns>True if has object otherwise false</returns>
        //public virtual bool Contains(UnityEngine.Object obj)
        //{
        //    return this.sceneDependencies.Contains(obj);
        //}

        ///// <summary>
        ///// Resolves the GUID and gets the scene object associated to it.
        ///// </summary>
        ///// <param name="guid">The GUID</param>
        ///// <returns>The scene object associated to this GUID</returns>
        //public virtual UnityEngine.Object ResolveReference(string guid)
        //{
        //    int index = this.guids.IndexOf(guid);
        //    if (index == -1)
        //    {
        //        return null;
        //    }
        //    else
        //    {
        //        return this.sceneDependencies[index];
        //    }
        //}

        ///// <summary>
        ///// Resolves the object and gets the GUID associated to it.
        ///// </summary>
        ///// <param name="obj"></param>
        ///// <returns>The GUID associated to the object</returns>
        //public virtual string ResolveGuid(UnityEngine.Object obj)
        //{
        //    int index = this.sceneDependencies.IndexOf(obj);
        //    if (index == -1)
        //    {
        //        return null;
        //    }
        //    else
        //    {
        //        return this.guids[index];
        //    }
        //}

        ///// <summary>
        ///// Adds a new reference.
        ///// </summary>
        ///// <param name="guid">The guid</param>
        ///// <param name="obj">The object</param>
        ///// <returns>The GUID or the generated GUID if the given GUID is null or empty</returns>
        //public virtual string AddReference(string guid, UnityEngine.Object obj)
        //{
        //    if (this.sceneDependencies.Contains(obj))
        //    {
        //        return null;
        //    }
        //    if (this.guids.Contains(guid))
        //    {
        //        return null;
        //    }
        //    string newGuid = guid;
        //    if (string.IsNullOrEmpty(guid))
        //    {
        //        newGuid = Guid.NewGuid().ToString("N");
        //        this.guids.Add(newGuid);
        //    }
        //    else
        //    {
        //        this.guids.Add(guid);
        //    }
        //    this.sceneDependencies.Add(obj);
        //    return newGuid;
        //}

        ///// <summary>
        ///// Adds a new object reference by generating a new GUID.
        ///// </summary>
        ///// <param name="obj"></param>
        ///// <returns>The generated GUID</returns>
        //public virtual string AddReference(UnityEngine.Object obj)
        //{
        //    if (this.sceneDependencies.Contains(obj))
        //    {
        //        return null;
        //    }
        //    Guid guid = Guid.NewGuid();
        //    this.guids.Add(guid.ToString("N"));
        //    this.sceneDependencies.Add(obj);
        //    return guid.ToString("N");
        //}

        ///// <summary>
        ///// Sets the reference with the guid.
        ///// </summary>
        ///// <param name="guid">The guid</param>
        ///// <param name="obj">The object</param>
        ///// <returns>True if succeed, otherwise false</returns>
        //public virtual bool Set(string guid, UnityEngine.Object obj)
        //{
        //    if (!this.guids.Contains(guid) || obj == null)
        //    {
        //        return false;
        //    }
        //    int index = this.guids.IndexOf(guid);
        //    this.sceneDependencies[index] = obj;
        //    return true;
        //}

    }

}