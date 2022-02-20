using UnityEditor;

using UnityEngine;

namespace Bayat.Core
{

    [CustomEditor(typeof(MaterialPropertiesResolver), true)]
    public class MaterialPropertiesResolverEditor : Editor
    {

        protected MaterialPropertiesResolver materialPropertiesResolver;
        protected bool foldout = false;

        private void OnEnable()
        {
            this.materialPropertiesResolver = (MaterialPropertiesResolver)target;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (materialPropertiesResolver == null)
            {
                return;
            }

            int currentDependenciesCount = materialPropertiesResolver.Materials.Count;
            GUILayout.Label(string.Format("Current: {0}", currentDependenciesCount));
            if (this.materialPropertiesResolver.HasNullReferences())
            {
                EditorGUILayout.HelpBox("There are null references in the materials dependencies, remove them to stop causing further issues and errors.", MessageType.Warning);
                if (GUILayout.Button("Remove Null References"))
                {
                    this.materialPropertiesResolver.RemoveNullReferences();
                }
            }
            if (this.materialPropertiesResolver.HasUnusedReferences())
            {
                EditorGUILayout.HelpBox("There are unused references in the materials dependencies, if you haven't used them in any scene or they are not referenced by Asset Reference Resolver, then they'll be shown here as unused, but if you are willing to use them here anyway kindly ignore this warning.", MessageType.Warning);
                if (GUILayout.Button("Remove Unused References"))
                {
                    this.materialPropertiesResolver.RemoveUnusedReferences();
                }
            }
            if (GUILayout.Button("Reset"))
            {
                if (EditorUtility.DisplayDialog("Reset Material Properties Database?", "This action will reset whole material properties database, so there will be problems when loading previously saved data using this database.\n\nProceed at your own risk.", "Reset", "Cancel"))
                {
                    this.materialPropertiesResolver.Materials.Clear();
                    this.materialPropertiesResolver.MaterialsProperties.Clear();
                    this.materialPropertiesResolver.Reset();
                }
            }

            if (GUILayout.Button("Collect Materials"))
            {
                this.materialPropertiesResolver.CollectMaterials();
            }
        }

    }

}