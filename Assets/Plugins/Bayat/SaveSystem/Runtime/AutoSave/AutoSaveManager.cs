using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor;
#endif

using Bayat.Core.Utilities;

namespace Bayat.SaveSystem
{

    [AddComponentMenu("Bayat/Save System/Auto Save Manager")]
    public class AutoSaveManager : MonoBehaviour
    {

        private static AutoSaveManager current;

        /// <summary>
        /// Gets the current scene reference resolver.
        /// </summary>
        public static AutoSaveManager Current
        {
            get
            {
                if (current == null)
                {
                    AutoSaveManager[] instances = FindObjectsOfType<AutoSaveManager>();
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
                        throw new InvalidOperationException("There is more than one AutoSaveManager in this scene, but there must only be one.");
                    }
                }
                return current;
            }
        }

        /// <summary>
        /// The predefined save event types for Auto Save.
        /// </summary>
        public enum SaveEventType
        {

            /// <summary>
            /// Manually call the <see cref="Save"/> method on Auto Save Manager to save the data.
            /// </summary>
            Manual,

            /// <summary>
            /// Uses the Unity <see cref="https://docs.unity3d.com/ScriptReference/MonoBehaviour.OnDisable.html">OnDisable</see> event function
            /// </summary>
            /// <remarks>
            /// This event function is unreliable on mobile devices, becase the game or app can be terminated directly by the user, so none of these end of lifecycle events will be fired by Unity such as OnDisable, OnDestroy, OnApplicationQuit.
            /// </remarks>
            OnDisable,

            /// <summary>
            /// Uses the Unity <see cref="https://docs.unity3d.com/ScriptReference/MonoBehaviour.OnDestroy.html">OnDestroy</see> event function
            /// </summary>
            /// <remarks>
            /// This event function is unreliable on mobile devices, becase the game or app can be terminated directly by the user, so none of these end of lifecycle events will be fired by Unity such as OnDisable, OnDestroy, OnApplicationQuit.
            /// </remarks>
            OnDestroy,

            /// <summary>
            /// Uses the Unity <see cref="https://docs.unity3d.com/ScriptReference/MonoBehaviour.OnApplicationQuit.html">OnApplicationQuit</see> event function
            /// </summary>
            /// <remarks>
            /// This event function is unreliable on mobile devices, becase the game or app can be terminated directly by the user, so none of these end of lifecycle events will be fired by Unity such as OnDisable, OnDestroy, OnApplicationQuit.
            /// </remarks>
            OnApplicationQuit,

            /// <summary>
            /// Uses the Unity <see cref="https://docs.unity3d.com/ScriptReference/MonoBehaviour.OnApplicationPause.html">OnApplicationPause</see> event function
            /// </summary>
            /// <remarks>
            /// This event function is more reliable on mobile devices, because it is called once the user goes out of the game, like the moment they want to close the game or app.
            /// </remarks>
            OnApplicationPause,

            /// <summary>
            /// Uses the Unity <see cref="https://docs.unity3d.com/ScriptReference/SceneManagement.SceneManager-sceneUnloaded.html">SceneManager.sceneUnloaded</see> event
            /// </summary>
            OnSceneUnloaded
        }

        /// <summary>
        /// The predefined load event types for Auto Save.
        /// </summary>
        public enum LoadEventType
        {

            /// <summary>
            /// Manually call the <see cref="Load"/> method on Auto Save Manager to load the data.
            /// </summary>
            Manual,

            /// <summary>
            /// Uses the Unity <see cref="https://docs.unity3d.com/ScriptReference/MonoBehaviour.OnEnable.html">OnEnable</see> event function
            /// </summary>
            OnEnable,

            /// <summary>
            /// Uses the Unity <see cref="https://docs.unity3d.com/ScriptReference/MonoBehaviour.Awake.html">Awake</see> event function
            /// </summary>
            Awake,

            /// <summary>
            /// Uses the Unity <see cref="https://docs.unity3d.com/ScriptReference/MonoBehaviour.Start.html">Start</see> event function
            /// </summary>
            Start,

            /// <summary>
            /// Uses the Unity <see cref="https://docs.unity3d.com/ScriptReference/SceneManagement.SceneManager-sceneLoaded.html">SceneManager.sceneLoaded</see> event
            /// </summary>
            OnSceneLoaded
        }

        #region Events

        /// <summary>
        /// Occurs when started saving.
        /// </summary>
        public event EventHandler Saving;

        /// <summary>
        /// Occurs when the data is saved
        /// </summary>
        public event EventHandler Saved;

        /// <summary>
        /// Occurs when started loading.
        /// </summary>
        public event EventHandler Loading;

        /// <summary>
        /// Occurs when the data is loaded.
        /// </summary>
        public event EventHandler Loaded;

        /// <summary>
        /// Occurs when started deleting.
        /// </summary>
        public event EventHandler Deleting;

        /// <summary>
        /// Occurs when the data is deleted.
        /// </summary>
        public event EventHandler Deleted;

        #endregion

        #region Fields

        [SerializeField]
        protected string identifier = "autosave.dat";
        [SerializeField]
        protected SaveSystemSettingsPreset settingsPreset;
        [SerializeField]
        protected List<AutoSave> autoSaves = new List<AutoSave>();
        [SerializeField]
        protected SaveEventType saveEvent = SaveEventType.OnApplicationQuit;
        [SerializeField]
        protected LoadEventType loadEvent = LoadEventType.Start;
        [SerializeField]
        protected UnityEvent savedEvent;
        [SerializeField]
        protected UnityEvent loadedEvent;
        [SerializeField]
        protected UnityEvent deletedEvent;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the identifier used for saving, loading or deleting the data.
        /// </summary>
        public virtual string Identifier
        {
            get { return this.identifier; }
            set { this.identifier = value; }
        }

        /// <summary>
        /// Gets or sets the settings preset used.
        /// </summary>
        public virtual SaveSystemSettingsPreset SettingsPreset
        {
            get { return this.settingsPreset; }
            set { this.settingsPreset = value; }
        }

        /// <summary>
        /// Gets the list of Auto Saves available.
        /// </summary>
        public virtual List<AutoSave> AutoSaves
        {
            get { return this.autoSaves; }
        }

        /// <summary>
        /// Gets or sets the predefined save event.
        /// </summary>
        public virtual SaveEventType SaveEvent
        {
            get { return this.saveEvent; }
            set { this.saveEvent = value; }
        }

        /// <summary>
        /// Gets or sets the predefined load event.
        /// </summary>
        public virtual LoadEventType LoadEvent
        {
            get { return this.loadEvent; }
            set { this.loadEvent = value; }
        }

        #endregion

        #region Unity Messages

        protected virtual void Reset()
        {
            FetchAutoSaves();
            this.settingsPreset = SaveSystemSettingsPreset.DefaultPreset;
        }

        protected virtual void OnValidate()
        {
            SortAutoSaves();
        }

        protected virtual void OnEnable()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
            SceneManager.sceneUnloaded += OnSceneUnloaded;
            if (this.loadEvent == LoadEventType.OnEnable)
            {
                Load();
            }
        }

        protected virtual void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            SceneManager.sceneUnloaded -= OnSceneUnloaded;
        }

        protected virtual void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (this.loadEvent == LoadEventType.OnSceneLoaded)
            {
                Load();
            }
        }

        protected virtual void OnSceneUnloaded(Scene scene)
        {
            if (this.saveEvent == SaveEventType.OnSceneUnloaded)
            {
                Save();
            }
        }

        protected virtual void Start()
        {
            if (this.loadEvent == LoadEventType.Start)
            {
                Load();
            }
        }

        protected virtual void Awake()
        {
            if (this.loadEvent == LoadEventType.Awake)
            {
                Load();
            }
        }

        protected virtual void OnApplicationQuit()
        {
            if (this.saveEvent == SaveEventType.OnApplicationQuit)
            {
                Save();
            }
        }

        protected virtual void OnApplicationPause(bool paused)
        {
            if ((saveEvent == SaveEventType.OnApplicationPause || (Application.isMobilePlatform && saveEvent == SaveEventType.OnApplicationQuit)) && paused)
            {
                Save();
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Sorts the Auto Saves list based on their hierarchical sorting.
        /// </summary>
        public virtual void SortAutoSaves()
        {
            HierarchicalSorting.Sort(this.autoSaves);
        }

        /// <summary>
        /// Fetches all Auto Save components from the scene.
        /// </summary>
        public virtual void FetchAutoSaves()
        {
            this.autoSaves = new List<AutoSave>(FindObjectsOfType<AutoSave>());
        }

        /// <summary>
        /// Adds the provided <paramref name="autoSave"/> to the list
        /// </summary>
        /// <param name="autoSave"></param>
        public virtual void AddAutoSave(AutoSave autoSave)
        {
            if (this.autoSaves.Contains(autoSave))
            {
                return;
            }
            this.autoSaves.Add(autoSave);

            SortAutoSaves();
        }

        /// <summary>
        /// Removes the specified <paramref name="autoSave"/> from the list.
        /// </summary>
        /// <param name="autoSave"></param>
        public virtual void RemoveAutoSave(AutoSave autoSave)
        {
            if (!this.autoSaves.Contains(autoSave))
            {
                return;
            }
            this.autoSaves.Remove(autoSave);

            SortAutoSaves();
        }

        /// <summary>
        /// Saves the GameObjects from the list of Auto Saves.
        /// </summary>
        public virtual async void Save()
        {
            SortAutoSaves();

            GameObject[] gameObjects = new GameObject[this.autoSaves.Count];
            for (int i = 0; i < this.autoSaves.Count; i++)
            {
                if (this.autoSaves[i] == null)
                {
                    continue;
                }
                gameObjects[i] = this.autoSaves[i].gameObject;
            }
            OnSaving();
            var task = SaveSystemAPI.SaveAsync(this.identifier, gameObjects, this.settingsPreset.CustomSettings);
            await task;
            if (task.Exception != null)
            {
                Debug.LogException(task.Exception);
                for (int i = 0; i < task.Exception.InnerExceptions.Count; i++)
                {
                    Debug.LogException(task.Exception.InnerExceptions[i]);
                }
            }
            else
            {
                OnSaved();
            }
        }

        protected virtual void OnSaving()
        {
            Saving?.Invoke(this, null);
        }

        protected virtual void OnSaved()
        {
            this.savedEvent?.Invoke();
            Saved?.Invoke(this, null);
        }

        /// <summary>
        /// Loads the saved data if it exists.
        /// </summary>
        public virtual async void Load()
        {
            if (!await SaveSystemAPI.ExistsAsync(this.identifier))
            {
                return;
            }
            OnLoading();
            var task = SaveSystemAPI.LoadAsync<GameObject[]>(this.identifier, this.settingsPreset.CustomSettings);
            await task;
            if (task.Exception != null)
            {
                Debug.LogException(task.Exception);
                for (int i = 0; i < task.Exception.InnerExceptions.Count; i++)
                {
                    Debug.LogException(task.Exception.InnerExceptions[i]);
                }
            }
            else
            {
                OnLoaded();
            }
        }

        protected virtual void OnLoading()
        {
            this.Loading?.Invoke(this, null);
        }

        protected virtual void OnLoaded()
        {
            this.loadedEvent?.Invoke();
            Loaded?.Invoke(this, null);
        }

        /// <summary>
        /// Deletes the saved data if it exists.
        /// </summary>
        public virtual async void Delete()
        {
            if (!await SaveSystemAPI.ExistsAsync(this.identifier))
            {
                return;
            }
            OnDeleting();
            var task = SaveSystemAPI.DeleteAsync(this.identifier, this.settingsPreset.CustomSettings);
            await task;
            if (task.Exception != null)
            {
                Debug.LogException(task.Exception);
                for (int i = 0; i < task.Exception.InnerExceptions.Count; i++)
                {
                    Debug.LogException(task.Exception.InnerExceptions[i]);
                }
            }
            else
            {
                OnDeleted();
            }
        }

        protected virtual void OnDeleting()
        {
            this.Deleting?.Invoke(this, null);
        }

        protected virtual void OnDeleted()
        {
            this.deletedEvent?.Invoke();
            Deleted?.Invoke(this, null);
        }

        /// <summary>
        /// Creates a new instance of <see cref="AutoSaveManager"/> in the current scene.
        /// </summary>
        public static GameObject CreateNewInstance()
        {
            GameObject go = new GameObject("Auto Save Manager");
            current = go.AddComponent<AutoSaveManager>();
            return go;
        }

#if UNITY_EDITOR
        [MenuItem("GameObject/Bayat/Save System/Auto Save Manager", false, 10)]
        public static void CreateNewInstanceMenu()
        {
            if (Current != null)
            {
                return;
            }
            Selection.activeGameObject = CreateNewInstance();
        }
#endif
        #endregion

    }

}