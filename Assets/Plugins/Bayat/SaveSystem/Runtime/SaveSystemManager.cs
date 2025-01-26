using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

using Bayat.Core;

namespace Bayat.SaveSystem
{

    [AddComponentMenu("Bayat/Save System/Save System Manager")]
    [DisallowMultipleComponent]
    public class SaveSystemManager : MonoBehaviour
    {

        private static SaveSystemManager current;

        /// <summary>
        /// Gets the current scene reference resolver.
        /// </summary>
        public static SaveSystemManager Current
        {
            get
            {
                if (current == null)
                {
                    #if UNITY_6000_0_OR_NEWER
                    SaveSystemManager[] instances = FindObjectsByType<SaveSystemManager>(FindObjectsSortMode.None);
                    #else
                    SaveSystemManager[] instances = FindObjectsOfType<SaveSystemManager>();
                    #endif
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
                        throw new InvalidOperationException("There is more than one SaveSystemManager in this scene, but there must only be one.");
                    }
                }
                return current;
            }
        }

        /// <summary>
        /// Creates a new instance of <see cref="SceneReferenceResolver"/> in the current scene.
        /// </summary>
        public static GameObject CreateNewInstance()
        {
            GameObject go = new GameObject("Save System Manager");
            current = go.AddComponent<SaveSystemManager>();
            go.AddComponent<AutoSaveManager>();
            go.AddComponent<SceneReferenceResolver>();
            return go;
        }

#if UNITY_EDITOR
        [MenuItem("GameObject/Bayat/Save System/Save System Manager", false, 10)]
        public static void CreateNewInstanceMenu()
        {
            if (Current != null)
            {
                return;
            }
            Selection.activeGameObject = CreateNewInstance();
        }
#endif

    }

}