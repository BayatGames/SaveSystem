using UnityEditor;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace Bayat.Core
{

    public class ReferenceCheckerWindow : EditorWindow
    {

        protected UnityObject unityObject;

        [MenuItem("Window/Bayat/Core/Reference Checker")]
        public static void Init()
        {
            var window = EditorWindow.GetWindow<ReferenceCheckerWindow>();
            window.titleContent = new GUIContent("Reference Checker");
            window.minSize = new Vector2(320, 180);
            window.ShowUtility();
        }

        private void OnGUI()
        {
            GUILayout.Label("Reference Checker", EditorStyles.boldLabel);
            GUILayout.Label("Check if an object is referenced by scene or by asset database.", EditorStyles.wordWrappedLabel);
            this.unityObject = EditorGUILayout.ObjectField(this.unityObject, typeof(UnityEngine.Object), true);
            if (this.unityObject == null)
            {
                EditorGUILayout.HelpBox("Select an object to check if this object is referenced by scene or asset.", MessageType.Info);
                return;
            }
            bool isReferenced = false;
            bool canBeReferencedByScene = !EditorUtility.IsPersistent(this.unityObject);
            bool isReferencedByScene = false;
            bool isReferencedByAsset = false;
            if (SceneReferenceResolver.Current != null)
            {
                isReferenced |= isReferencedByScene = SceneReferenceResolver.Current.Contains(this.unityObject);
            }
            if (AssetReferenceResolver.Current != null)
            {
                isReferenced |= isReferencedByAsset = AssetReferenceResolver.Current.Contains(this.unityObject);
            }
            GUILayout.BeginHorizontal(EditorStyles.helpBox);
            GUILayout.Label(string.Format("Is Persistent: {0}", EditorUtility.IsPersistent(this.unityObject)));
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal(EditorStyles.helpBox);
            GUILayout.Label(string.Format("Is Referenced: {0}", isReferenced));
            GUILayout.EndHorizontal();

            if (SceneReferenceResolver.Current == null)
            {
                EditorGUILayout.HelpBox("There are no scene reference resolver in this scene.", MessageType.Warning);
                if (GUILayout.Button("Add to Scene", EditorStyles.miniButton))
                {
                    SceneReferenceResolver.CreateNewInstance();
                }
            }
            else
            {
                GUILayout.BeginHorizontal(EditorStyles.helpBox);
                GUILayout.Label(string.Format("Is Referenced By Scene: {0}", isReferencedByScene));
                if (!isReferencedByScene && canBeReferencedByScene)
                {
                    if (GUILayout.Button("Add Reference", EditorStyles.miniButton))
                    {
                        string guid = SceneReferenceResolver.Current.Add(this.unityObject);
                        Debug.LogFormat("New reference added to Scene with '{0}' GUID.", guid);
                    }
                }
                GUILayout.EndHorizontal();
            }

            GUILayout.BeginHorizontal(EditorStyles.helpBox);
            GUILayout.Label(string.Format("Is Referenced By Asset: {0}", isReferencedByAsset));
            if (!isReferencedByAsset && !canBeReferencedByScene)
            {
                if (GUILayout.Button("Add Reference", EditorStyles.miniButton))
                {
                    string guid = AssetReferenceResolver.Current.Add(this.unityObject);
                    Debug.LogFormat("New reference added to Asset Reference Database with '{0}' GUID.", guid);
                }
            }
            GUILayout.EndHorizontal();

            string path = AssetDatabase.GetAssetPath(this.unityObject);
            GUILayout.BeginHorizontal(EditorStyles.helpBox);
            GUILayout.Label("Asset Path:");
            EditorGUILayout.SelectableLabel(path, EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));
            GUILayout.EndHorizontal();

            GUILayout.FlexibleSpace();
            GUILayout.Label("Made with ❤️ by Bayat", EditorStyles.centeredGreyMiniLabel);
        }

    }

}