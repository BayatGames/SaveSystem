using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityObject = UnityEngine.Object;

#if UNITY_6000_3_OR_NEWER
using ObjectId = UnityEngine.EntityId;
using TreeView = UnityEditor.IMGUI.Controls.TreeView<UnityEngine.EntityId>;
using TreeViewItem = UnityEditor.IMGUI.Controls.TreeViewItem<UnityEngine.EntityId>;
using TreeViewState = UnityEditor.IMGUI.Controls.TreeViewState<UnityEngine.EntityId>;
#elif UNITY_6000_2_OR_NEWER
using ObjectId = System.Int32;
using TreeView = UnityEditor.IMGUI.Controls.TreeView<int>;
using TreeViewItem = UnityEditor.IMGUI.Controls.TreeViewItem<int>;
using TreeViewState = UnityEditor.IMGUI.Controls.TreeViewState<int>;
#else
using ObjectId = System.Int32;
#endif

namespace Bayat.Json
{

    public class GameObjectSerializationManagerTreeView : TreeView
    {

        public GameObjectSerializationManagerTreeView(TreeViewState state)
            : base(state)
        {
            Reload();
        }

        protected override TreeViewItem BuildRoot()
        {
            return new TreeViewItem { id = default(ObjectId), depth = -1 };
        }

        protected override IList<TreeViewItem> BuildRows(TreeViewItem root)
        {
            var rows = GetRows() ?? new List<TreeViewItem>(200);

            Scene scene = SceneManager.GetSceneAt(0);

            rows.Clear();
            var gameObjectRoots = scene.GetRootGameObjects();
            foreach (var gameObject in gameObjectRoots)
            {
                var item = CreateTreeViewItemForGameObject(gameObject);
                root.AddChild(item);
                rows.Add(item);
                if (gameObject.transform.childCount > 0)
                {
                    if (IsExpanded(item.id))
                    {
                        AddChildrenRecursive(gameObject, item, rows);
                    }
                    else
                    {
                        item.children = CreateChildListForCollapsedParent();
                    }
                }
            }

            SetupDepthsFromParentsAndChildren(root);
            return rows;
        }

        void AddChildrenRecursive(GameObject go, TreeViewItem item, IList<TreeViewItem> rows)
        {
            int childCount = go.transform.childCount;

            item.children = new List<TreeViewItem>(childCount);
            for (int i = 0; i < childCount; ++i)
            {
                var childTransform = go.transform.GetChild(i);
                var childItem = CreateTreeViewItemForGameObject(childTransform.gameObject);
                item.AddChild(childItem);
                rows.Add(childItem);

                if (childTransform.childCount > 0)
                {
                    if (IsExpanded(childItem.id))
                    {
                        AddChildrenRecursive(childTransform.gameObject, childItem, rows);
                    }
                    else
                    {
                        childItem.children = CreateChildListForCollapsedParent();
                    }
                }
            }
        }

        static TreeViewItem CreateTreeViewItemForGameObject(GameObject gameObject)
        {
            return new TreeViewItem(GetObjectId(gameObject), -1, gameObject.name);
        }

        protected override IList<ObjectId> GetAncestors(ObjectId id)
        {
            // The backend needs to provide us with this info since the item with id
            // may not be present in the rows
            var ancestors = new List<ObjectId>();
            var gameObject = GetGameObject(id);
            if (gameObject == null)
            {
                return ancestors;
            }

            var transform = gameObject.transform;
            while (transform.parent != null)
            {
                ancestors.Add(GetObjectId(transform.parent.gameObject));
                transform = transform.parent;
            }

            return ancestors;
        }

        protected override IList<ObjectId> GetDescendantsThatHaveChildren(ObjectId id)
        {
            var parents = new List<ObjectId>();
            var gameObject = GetGameObject(id);
            if (gameObject == null)
            {
                return parents;
            }

            var stack = new Stack<Transform>();
            stack.Push(gameObject.transform);
            while (stack.Count > 0)
            {
                Transform current = stack.Pop();
                parents.Add(GetObjectId(current.gameObject));
                for (int i = 0; i < current.childCount; ++i)
                {
                    if (current.childCount > 0)
                        stack.Push(current.GetChild(i));
                }
            }

            return parents;
        }

        private static ObjectId GetObjectId(GameObject gameObject)
        {
#if UNITY_6000_3_OR_NEWER
            return gameObject.GetEntityId();
#else
            return gameObject.GetInstanceID();
#endif
        }

        private static GameObject GetGameObject(ObjectId id)
        {
#if UNITY_6000_3_OR_NEWER
            return EditorUtility.EntityIdToObject(id) as GameObject;
#else
            return EditorUtility.InstanceIDToObject(id) as GameObject;
#endif
        }

        // Custom GUI

        protected override void RowGUI(RowGUIArgs args)
        {
            Event evt = Event.current;
            //extraSpaceBeforeIconAndLabel = 18f;

            // GameObject isStatic toggle 
            var gameObject = GetGameObject(args.item.id);
            if (gameObject == null)
                return;

            //Rect toggleRect = args.rowRect;
            //toggleRect.x += GetContentIndent(args.item);
            //toggleRect.width = 16f;

            //// Ensure row is selected before using the toggle (usability)
            //if (evt.type == EventType.MouseDown && toggleRect.Contains(evt.mousePosition))
            //    SelectionClick(args.item, false);

            //EditorGUI.BeginChangeCheck();
            //bool isStatic = EditorGUI.Toggle(toggleRect, gameObject.isStatic);
            //if (EditorGUI.EndChangeCheck())
            //    gameObject.isStatic = isStatic;

            // Text
            base.RowGUI(args);
        }

        void DrawItemBackground(Rect bgRect)
        {
            if (Event.current.type == EventType.Repaint)
            {
                var rect = bgRect;
                rect.height = Styles.headerBackground.fixedHeight;
                Styles.headerBackground.Draw(rect, false, false, false, false);

                rect.y += rect.height;
                rect.height = bgRect.height - rect.height;
                Styles.background.Draw(rect, false, false, false, false);
            }
        }

        void HeaderGUI(Rect headerRect, string label, TreeViewItem item, GameObject gameObject)
        {
            headerRect.y += 1f;

            // Do toggle
            Rect toggleRect = headerRect;
            toggleRect.width = 16;
            EditorGUI.BeginChangeCheck();

            gameObject.SetActive(EditorGUI.Toggle(toggleRect, gameObject.activeSelf)); // hide when outside cell rect
            if (EditorGUI.EndChangeCheck())
                RefreshCustomRowHeights();

            Rect labelRect = headerRect;
            labelRect.xMin += toggleRect.width + 2f;
            GUI.Label(labelRect, label);
        }

        // Selection

        public void SyncEditorSelection()
        {
#if UNITY_6000_3_OR_NEWER
            SetSelection(Selection.entityIds);
#else
            SetSelection(Selection.instanceIDs);
#endif
        }

        protected override void SelectionChanged(IList<ObjectId> selectedIds)
        {
#if UNITY_6000_3_OR_NEWER
            Selection.entityIds = selectedIds.ToArray();
#else
            Selection.instanceIDs = selectedIds.ToArray();
#endif
        }

        // Reordering

        protected override bool CanStartDrag(CanStartDragArgs args)
        {
            return true;
        }

        protected override void SetupDragAndDrop(SetupDragAndDropArgs args)
        {
            DragAndDrop.PrepareStartDrag();

            var sortedDraggedIDs = SortItemIDsInRowOrder(args.draggedItemIDs);

            List<UnityObject> objList = new List<UnityObject>(sortedDraggedIDs.Count);
            foreach (var id in sortedDraggedIDs)
            {
                var obj = GetGameObject(id);
                if (obj != null)
                {
                    objList.Add(obj);
                }
            }

            if (objList.Count == 0)
            {
                return;
            }

            DragAndDrop.objectReferences = objList.ToArray();

            string title = objList.Count > 1 ? "<Multiple>" : objList[0].name;
            DragAndDrop.StartDrag(title);
        }

        protected override DragAndDropVisualMode HandleDragAndDrop(DragAndDropArgs args)
        {
            // First check if the dragged objects are GameObjects
            var draggedObjects = DragAndDrop.objectReferences;
            var transforms = new List<Transform>(draggedObjects.Length);
            foreach (var obj in draggedObjects)
            {
                var go = obj as GameObject;
                if (go == null)
                {
                    return DragAndDropVisualMode.None;
                }

                transforms.Add(go.transform);
            }

            // Filter out any unnecessary transforms before the reparent operation
            RemoveItemsThatAreDescendantsFromOtherItems(transforms);

            // Reparent
            if (args.performDrop)
            {
                switch (args.dragAndDropPosition)
                {
                    case DragAndDropPosition.UponItem:
                    case DragAndDropPosition.BetweenItems:
                        var parentObject = args.parentItem != null ? GetGameObject(args.parentItem.id) : null;
                        Transform parent = parentObject != null ? parentObject.transform : null;

                        if (!IsValidReparenting(parent, transforms))
                            return DragAndDropVisualMode.None;

                        foreach (var trans in transforms)
                            trans.SetParent(parent);

                        if (args.dragAndDropPosition == DragAndDropPosition.BetweenItems)
                        {
                            int insertIndex = args.insertAtIndex;
                            for (int i = transforms.Count - 1; i >= 0; i--)
                            {
                                var transform = transforms[i];
                                insertIndex = GetAdjustedInsertIndex(parent, transform, insertIndex);
                                transform.SetSiblingIndex(insertIndex);
                            }
                        }
                        break;

                    case DragAndDropPosition.OutsideItems:
                        foreach (var trans in transforms)
                        {
                            trans.SetParent(null); // make root when dragged to empty space in treeview
                        }
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                Reload();
                SetSelection(transforms.Select(t => GetObjectId(t.gameObject)).ToList(), TreeViewSelectionOptions.RevealAndFrame);
            }

            return DragAndDropVisualMode.Move;
        }

        int GetAdjustedInsertIndex(Transform parent, Transform transformToInsert, int insertIndex)
        {
            if (transformToInsert.parent == parent && transformToInsert.GetSiblingIndex() < insertIndex)
                return --insertIndex;
            return insertIndex;
        }

        bool IsValidReparenting(Transform parent, List<Transform> transformsToMove)
        {
            if (parent == null)
                return true;

            foreach (var transformToMove in transformsToMove)
            {
                if (transformToMove == parent)
                    return false;

                if (IsHoveredAChildOfDragged(parent, transformToMove))
                    return false;
            }

            return true;
        }


        bool IsHoveredAChildOfDragged(Transform hovered, Transform dragged)
        {
            Transform t = hovered.parent;
            while (t)
            {
                if (t == dragged)
                    return true;
                t = t.parent;
            }
            return false;
        }


        // Returns true if there is an ancestor of transform in the transforms list
        static bool IsDescendantOf(Transform transform, List<Transform> transforms)
        {
            while (transform != null)
            {
                transform = transform.parent;
                if (transforms.Contains(transform))
                    return true;
            }
            return false;
        }

        static void RemoveItemsThatAreDescendantsFromOtherItems(List<Transform> transforms)
        {
            transforms.RemoveAll(t => IsDescendantOf(t, transforms));
        }

        public static class Styles
        {

            public static GUIStyle background = "RL Background";
            public static GUIStyle headerBackground = "RL Header";

        }

    }

}
