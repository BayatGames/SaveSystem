using UnityEditor;

namespace Bayat.Core
{

    public static class EditorApplicationUtility
    {

        #region Assembly Lock

        public static bool isAssemblyReloadLocked { get; private set; }

        private static bool wantedScriptChangesDuringPlay;

        public static void LockReloadAssemblies()
        {
            isAssemblyReloadLocked = true;
            EditorApplication.LockReloadAssemblies();
        }

        public static void UnlockReloadAssemblies()
        {
            EditorApplication.UnlockReloadAssemblies();
            isAssemblyReloadLocked = false;
        }

        [MenuItem("Tools/Bayat/Developer/Force Unlock Assembly Reload")]
        public static void ClearProgressBar()
        {
            EditorApplication.UnlockReloadAssemblies();
            isAssemblyReloadLocked = false;
        }

        public static bool WantsScriptChangesDuringPlay()
        {
            return EditorPrefs.GetInt("ScriptCompilationDuringPlay", 0) == 0;
        }

        #endregion

    }

}
