﻿using System;
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
    public class AutoSaveManager : MonoBehaviour, ISerializationCallbackReceiver
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
        [Obsolete("Use EventType instead.")]
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
        [Obsolete("Use EventType instead.")]
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

        /// <summary>
        /// A set of predefined event types.
        /// </summary>
        public enum EventType
        {

            /// <summary>
            /// Manually manage the event.
            /// </summary>
            Manual,

            /// <summary>
            /// Uses the Unity <see cref="https://docs.unity3d.com/ScriptReference/MonoBehaviour.Awake.html">Awake</see> event function
            /// </summary>
            Awake,

            /// <summary>
            /// Uses the Unity <see cref="https://docs.unity3d.com/ScriptReference/MonoBehaviour.Start.html">Start</see> event function
            /// </summary>
            Start,

            /// <summary>
            /// Uses the Unity <see cref="https://docs.unity3d.com/ScriptReference/MonoBehaviour.OnDisable.html">OnDisable</see> event function
            /// </summary>
            /// <remarks>
            /// This event function is unreliable on mobile devices, becase the game or app can be terminated directly by the user, so none of these end of lifecycle events will be fired by Unity such as OnDisable, OnDestroy, OnApplicationQuit.
            /// </remarks>
            OnDisable,

            /// <summary>
            /// Uses the Unity <see cref="https://docs.unity3d.com/ScriptReference/MonoBehaviour.OnEnable.html">OnEnable</see> event function
            /// </summary>
            OnEnable,

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
            /// Uses the Unity <see cref="https://docs.unity3d.com/ScriptReference/SceneManagement.SceneManager-sceneLoaded.html">SceneManager.sceneLoaded</see> event
            /// </summary>
            OnSceneLoaded,

            /// <summary>
            /// Uses the Unity <see cref="https://docs.unity3d.com/ScriptReference/SceneManagement.SceneManager-sceneUnloaded.html">SceneManager.sceneUnloaded</see> event
            /// </summary>
            OnSceneUnloaded

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

        [Tooltip("The storage identifier to save and load the data")]
        [SerializeField]
        protected string identifier = "autosave.dat";

        [Tooltip("The settings preset to be used for all the Save System API calls")]
        [SerializeField]
        protected SaveSystemSettingsPreset settingsPreset;

        [Tooltip("The list of auto saves to save and load")]
        [SerializeField]
        protected List<AutoSave> autoSaves = new List<AutoSave>();

#pragma warning disable 0612, 0618
        // To be removed in next patches
        // Used for migration
        [HideInInspector]
        [SerializeField]
        protected SaveEventType saveEvent = SaveEventType.OnApplicationQuit;
        [HideInInspector]
        [SerializeField]
        protected LoadEventType loadEvent = LoadEventType.Start;

        [HideInInspector]
        [SerializeField]
        protected bool migrated = false;
#pragma warning restore 0612, 0618

        [Tooltip("Specify when to save the data")]
        [SerializeField]
        protected EventType saveEventType = EventType.OnApplicationQuit;

        [Tooltip("Specify when to load the data")]
        [SerializeField]
        protected EventType loadEventType = EventType.Start;

        [Tooltip("Specify when to fetch new auto saves in the scene, like when the scene is loaded")]
        [SerializeField]
        protected EventType fetchEventType = EventType.Start;

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
        public virtual EventType SaveEvent
        {
            get { return this.saveEventType; }
            set { this.saveEventType = value; }
        }

        /// <summary>
        /// Gets or sets the predefined load event.
        /// </summary>
        public virtual EventType LoadEvent
        {
            get { return this.loadEventType; }
            set { this.loadEventType = value; }
        }

        /// <summary>
        /// Gets or sets the predefined fetch event.
        /// </summary>
        public virtual EventType FetchEvent
        {
            get { return this.fetchEventType; }
            set { this.fetchEventType = value; }
        }

        #endregion

        #region Unity Messages

        public void OnBeforeSerialize()
        {

        }

        public void OnAfterDeserialize()
        {
            if (this.migrated)
            {
                return;
            }

            // Migrate the enums from the old fields
            EventType eventType;
            if (Enum.TryParse(this.saveEvent.ToString(), out eventType))
            {
                this.saveEventType = eventType;
                this.migrated = true;
            }

            if (Enum.TryParse(this.loadEvent.ToString(), out eventType))
            {
                this.loadEventType = eventType;
                this.migrated = true;
            }
        }

        protected virtual void Reset()
        {
            this.settingsPreset = SaveSystemSettingsPreset.DefaultPreset;

            FetchAutoSaves();
        }

        protected virtual void OnValidate()
        {
            SortAutoSaves();
        }

        protected virtual void Awake()
        {
            HandleEventType(EventType.Awake);
        }

        protected virtual void Start()
        {
            HandleEventType(EventType.Start);
        }

        protected virtual void OnEnable()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
            SceneManager.sceneUnloaded += OnSceneUnloaded;

            HandleEventType(EventType.OnEnable);
        }

        protected virtual void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            SceneManager.sceneUnloaded -= OnSceneUnloaded;

            HandleEventType(EventType.OnDisable);
        }

        protected virtual void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            HandleEventType(EventType.OnSceneLoaded);
        }

        protected virtual void OnSceneUnloaded(Scene scene)
        {
            HandleEventType(EventType.OnSceneUnloaded);
        }

        protected virtual void OnApplicationQuit()
        {
            HandleEventType(EventType.OnApplicationQuit);
        }

        protected virtual void OnApplicationPause(bool paused)
        {
            if (!paused)
            {
                return;
            }

            if (Application.isMobilePlatform)
            {
                if (this.saveEventType == EventType.OnApplicationQuit)
                {
                    HandleEventType(EventType.OnApplicationQuit);
                }
                else
                {
                    HandleEventType(EventType.OnApplicationPause);
                }
            }
            else
            {
                HandleEventType(EventType.OnApplicationPause);
            }
        }

        protected virtual void HandleEventType(EventType type)
        {
            if (this.fetchEventType == type)
            {
                FetchAutoSaves();
            }
            if (this.saveEventType == type)
            {
                Save();
            }
            if (this.loadEventType == type)
            {
                Load();
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
            if (this.autoSaves == null)
            {
                this.autoSaves = new List<AutoSave>();
            }

            var fetchedAutoSaves = FindObjectsOfType<AutoSave>();
            for (int i = 0; i < fetchedAutoSaves.Length; i++)
            {
                var fetchedAutoSave = fetchedAutoSaves[i];
                if (!this.autoSaves.Contains(fetchedAutoSave))
                {
                    this.autoSaves.Add(fetchedAutoSave);
                }
            }

            // Resort auto saves
            SortAutoSaves();
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
            Loading?.Invoke(this, null);
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
            Deleting?.Invoke(this, null);
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