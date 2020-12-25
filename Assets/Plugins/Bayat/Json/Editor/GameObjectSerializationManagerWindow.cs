using Bayat.Core.EditorWindows;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace Bayat.Json
{

    public class GameObjectSerializationManagerWindow : EditorWindowWrapper
    {

        [SerializeField]
        protected TreeViewState treeViewState;
        protected GameObjectSerializationManagerTreeView treeView;
        protected Vector2 serializationHandlerScrollPosition;
        protected Vector2 serializationHandlerComponentsScrollPosition;
        protected Vector2 serializationHandlerChildrenScrollPosition;

        protected Object unityObj;

        [MenuItem("Window/Bayat/Json/GameObject Serialization Manager")]
        private static void Initialize()
        {
            var window = new GameObjectSerializationManagerWindow();
            window.Show();
        }

        public new void Show()
        {
            if (window == null)
            {
                ShowUtility();
                window.Center();
            }
            else
            {
                window.Focus();
            }
        }

        protected override void ConfigureWindow()
        {
            window.titleContent = new GUIContent("GameObject Serialization Manager");
            window.minSize = new Vector2(630, 400);
        }

        public override void OnShow()
        {
            if (this.treeViewState == null)
            {
                this.treeViewState = new TreeViewState();
            }

            this.treeView = new GameObjectSerializationManagerTreeView(this.treeViewState);
        }

        public override void OnSelectionChange()
        {
            if (this.treeView != null)
            {
                this.treeView.SetSelection(Selection.instanceIDs);
            }
            this.window.Repaint();
        }

        public override void OnHierarchyChange()
        {
            if (this.treeView != null)
            {
                this.treeView.Reload();
            }
            this.window.Repaint();
        }

        public override void OnGUI()
        {
            GUILayout.Label("GameObject Serialization <b>Manager</b>", Styles.CenteredLargeLabel);

            DoToolbar();

            EditorGUILayout.BeginHorizontal();
            DoTreeView();
            this.serializationHandlerScrollPosition = EditorGUILayout.BeginScrollView(this.serializationHandlerScrollPosition, Styles.InspectorStyle, GUILayout.Width(window.position.width / 2f));
            DrawSerializationHandler();
            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndHorizontal();

            GUILayout.FlexibleSpace();

            GUILayout.Label("Made with ❤️ by Bayat", EditorStyles.centeredGreyMiniLabel);
        }

        void DoTreeView()
        {
            Rect rect = GUILayoutUtility.GetRect(0, window.position.width / 2f, 0, 10000);
            this.treeView.OnGUI(rect);
        }

        void DoToolbar()
        {
            GUILayout.BeginHorizontal(EditorStyles.toolbar);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }

        void DrawSerializationHandler()
        {
            if (Selection.activeGameObject == null)
            {
                GUILayout.Label("Select a GameObject to edit", Styles.CenteredMiniLabel);
                return;
            }
            if (Selection.instanceIDs.Length > 1)
            {
                GUILayout.Label("Select only 1 object to edit", Styles.CenteredMiniLabel);
                return;
            }

            GUILayout.Label(
                new GUIContent(Selection.activeGameObject.name, AssetPreview.GetMiniThumbnail(Selection.activeGameObject)), EditorStyles.boldLabel);

            Component[] components = Selection.activeGameObject.GetComponents<Component>();
            GameObjectSerializationHandler serializationHandler = Selection.activeGameObject.GetComponent<GameObjectSerializationHandler>();
            if (serializationHandler == null && GUILayout.Button("Add GameObject Serialization Handler"))
            {
                serializationHandler = Selection.activeGameObject.AddComponent<GameObjectSerializationHandler>();
            }

            if (serializationHandler == null)
            {
                GUILayout.Label("Add a serialization handler component to this GameObject to manage it's serialization behaviour, such as excluding children or components.", EditorStyles.wordWrappedMiniLabel);
                return;
            }

            if (GUILayout.Button("Remove GameObject Serialization Handler"))
            {
                UnityEngine.Object.DestroyImmediate(serializationHandler);
                return;
            }

            EditorGUILayout.Separator();

            GUILayout.Label("Settings", EditorStyles.boldLabel);
            GUILayout.Label("Adjust the serialization handler settings.", EditorStyles.wordWrappedMiniLabel);

            SerializedObject serializedObject = new SerializedObject(serializationHandler);
            serializedObject.Update();

            SerializedProperty serializeChildrenProperty = serializedObject.FindProperty("serializeChildren");
            SerializedProperty serializeComponentsProperty = serializedObject.FindProperty("serializeComponents");
            SerializedProperty serializeExcludedChildrenProperty = serializedObject.FindProperty("serializeExcludedChildren");
            SerializedProperty serializeExcludedComponentsProperty = serializedObject.FindProperty("serializeExcludedComponents");
            serializeChildrenProperty.boolValue = EditorGUILayout.ToggleLeft(serializeChildrenProperty.displayName, serializeChildrenProperty.boolValue);
            serializeComponentsProperty.boolValue = EditorGUILayout.ToggleLeft(serializeComponentsProperty.displayName, serializeComponentsProperty.boolValue);
            serializeExcludedChildrenProperty.boolValue = EditorGUILayout.ToggleLeft(serializeExcludedChildrenProperty.displayName, serializeExcludedChildrenProperty.boolValue);
            serializeExcludedComponentsProperty.boolValue = EditorGUILayout.ToggleLeft(serializeExcludedComponentsProperty.displayName, serializeExcludedComponentsProperty.boolValue);

            serializedObject.ApplyModifiedProperties();

            // Remove null references
            int index = 0;
            while (index < serializationHandler.ExcludedChildren.Count)
            {
                if (serializationHandler.ExcludedChildren[index] == null)
                {
                    serializationHandler.ExcludedChildren.RemoveAt(index);
                }
                else
                {
                    index++;
                }
            }
            index = 0;
            while (index < serializationHandler.ExcludedComponents.Count)
            {
                if (serializationHandler.ExcludedComponents[index] == null)
                {
                    serializationHandler.ExcludedComponents.RemoveAt(index);
                }
                else
                {
                    index++;
                }
            }

            EditorGUILayout.Separator();

            GUILayout.Label("Components", EditorStyles.boldLabel);

            if (!serializationHandler.SerializeComponents)
            {
                EditorGUILayout.HelpBox("Enable Serialize Components to exclude components in serialization.", MessageType.Warning);
            }

            GUILayout.Label("Use the checkboxes below to determine which components to be serialized.", EditorStyles.wordWrappedMiniLabel);

            this.serializationHandlerComponentsScrollPosition = EditorGUILayout.BeginScrollView(this.serializationHandlerComponentsScrollPosition);
            for (int i = 0; i < components.Length; i++)
            {
                Component component = components[i];
                EditorGUI.BeginChangeCheck();
                bool oldShouldSerialize = serializationHandler.ShouldSerializeComponent(component);
                bool shouldSerialize = EditorGUILayout.ToggleLeft(new GUIContent(ObjectNames.GetInspectorTitle(component), AssetPreview.GetMiniThumbnail(component)), oldShouldSerialize);
                if (EditorGUI.EndChangeCheck())
                {

                    // Remove from excluded
                    if (shouldSerialize && !oldShouldSerialize)
                    {
                        if (serializationHandler.SerializeExcludedComponents)
                        {
                            serializationHandler.ExcludedComponents.Add(component);
                        }
                        else
                        {
                            serializationHandler.ExcludedComponents.Remove(component);
                        }
                    }

                    // Add to excluded
                    else if (!shouldSerialize && oldShouldSerialize)
                    {
                        if (serializationHandler.SerializeExcludedComponents)
                        {
                            serializationHandler.ExcludedComponents.Remove(component);
                        }
                        else
                        {
                            serializationHandler.ExcludedComponents.Add(component);
                        }
                    }
                }
            }
            EditorGUILayout.EndScrollView();

            EditorGUILayout.Separator();

            GUILayout.Label("Children", EditorStyles.boldLabel);

            if (!serializationHandler.SerializeChildren)
            {
                EditorGUILayout.HelpBox("Enable Serialize Children to exclude children in serialization.", MessageType.Warning);
            }

            GUILayout.Label("Use the checkboxes below to determine which children to be serialized.", EditorStyles.wordWrappedMiniLabel);

            this.serializationHandlerChildrenScrollPosition = EditorGUILayout.BeginScrollView(this.serializationHandlerChildrenScrollPosition);
            Transform transform = Selection.activeTransform;
            for (int i = 0; i < transform.childCount; i++)
            {
                Transform child = transform.GetChild(i);
                EditorGUI.BeginChangeCheck();
                bool oldShouldSerialize = serializationHandler.ShouldSerializeChild(child);
                bool shouldSerialize = EditorGUILayout.ToggleLeft(new GUIContent(ObjectNames.GetInspectorTitle(child.gameObject), AssetPreview.GetMiniThumbnail(child.gameObject)), oldShouldSerialize);
                if (EditorGUI.EndChangeCheck())
                {

                    // Remove from excluded
                    if (shouldSerialize && !oldShouldSerialize)
                    {
                        if (serializationHandler.SerializeExcludedChildren)
                        {
                            serializationHandler.ExcludedChildren.Add(child);
                        }
                        else
                        {
                            serializationHandler.ExcludedChildren.Remove(child);
                        }
                    }

                    // Add to excluded
                    else if (!shouldSerialize && oldShouldSerialize)
                    {
                        if (serializationHandler.SerializeExcludedChildren)
                        {
                            serializationHandler.ExcludedChildren.Remove(child);
                        }
                        else
                        {
                            serializationHandler.ExcludedChildren.Add(child);
                        }
                    }
                }
            }
            EditorGUILayout.EndScrollView();
        }

        public static class Styles
        {
            static Styles()
            {
                CenteredLargeLabel = new GUIStyle(EditorStyles.largeLabel);
                CenteredLargeLabel.alignment = TextAnchor.MiddleCenter;
                CenteredLargeLabel.richText = true;
                InspectorStyle = new GUIStyle(GUI.skin.box);
                InspectorStyle.padding = new RectOffset(10, 10, 3, 3);
                InspectorStyle.margin = new RectOffset(0, 0, 0, 0);
                InspectorStyle.border = new RectOffset(InspectorStyle.border.left, 1, 1, 1);
                CenteredMiniLabel = EditorStyles.centeredGreyMiniLabel;
                CenteredMiniLabel.alignment = TextAnchor.MiddleCenter;
            }

            public static readonly GUIStyle CenteredLargeLabel;
            public static readonly GUIStyle CenteredMiniLabel;
            public static readonly GUIStyle InspectorStyle;

        }

    }

}