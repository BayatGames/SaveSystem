using UnityEditor;
using UnityEngine;

namespace Bayat.Core.EditorWindows
{

    public class WrappedEditorWindow : EditorWindow
    {

        // The wrapper reference is not serialized and will be lost
        // on assembly reload. Hence why we lock assembly reload while
        // this window is open. 
        public EditorWindowWrapper wrapper { get; set; }

        private void Awake() { }

        private void OnDestroy() { }

        private void OnEnable()
        {
            EditorApplicationUtility.LockReloadAssemblies();
            wrapper?.OnShow();
        }

        private void OnDisable()
        {
            wrapper?.OnClose();
            EditorApplicationUtility.UnlockReloadAssemblies();
        }

        private void Update()
        {
            if (wrapper == null)
            {
                Close();
                return;
            }

            try
            {
                wrapper.Update();
            }
            catch (WindowClose)
            {
                Close();
            }
        }

        private void OnGUI()
        {
            try
            {
                wrapper?.OnGUI();
            }
            catch (ExitGUIException) { }
            catch (WindowClose)
            {
                Close();
            }
        }

        private void OnFocus()
        {
            try
            {
                wrapper?.OnFocus();
            }
            catch (WindowClose)
            {
                Close();
            }
        }

        private void OnLostFocus()
        {
            try
            {
                wrapper?.OnLostFocus();
            }
            catch (WindowClose)
            {
                Close();
            }
        }

        private void OnHierarchyChange()
        {
            try
            {
                wrapper?.OnHierarchyChange();
            }
            catch (WindowClose)
            {
                Close();
            }
        }

        private void OnInspectorUpdate()
        {
            try
            {
                wrapper?.OnInspectorUpdate();
            }
            catch (WindowClose)
            {
                Close();
            }
        }

        private void OnProjectChange()
        {
            try
            {
                wrapper?.OnProjectChange();
            }
            catch (WindowClose)
            {
                Close();
            }
        }

        private void OnSelectionChange()
        {
            try
            {
                wrapper?.OnSelectionChange();
            }
            catch (WindowClose)
            {
                Close();
            }
        }

    }

}