using System.Collections.Generic;

using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Bayat.Core
{

#if BAYAT_DEVELOPER
    [CreateAssetMenu(menuName = "Bayat/Core/Material Properties Resolver")]
#endif
    public class MaterialPropertiesResolver : ScriptableObject
    {

        private static MaterialPropertiesResolver current;

        /// <summary>
        /// Gets the current material properties reference resolver.
        /// </summary>
        public static MaterialPropertiesResolver Current
        {
            get
            {
                if (current == null)
                {
                    MaterialPropertiesResolver instance = null;
#if UNITY_EDITOR
                    string[] instanceGuids = AssetDatabase.FindAssets("t: MaterialPropertiesResolver");
                    if (instanceGuids.Length > 1)
                    {
                        Debug.LogError("There is more than one MaterialPropertiesResolver in this project, but there must only be one.");
                        Debug.Log("Deleting other instances of MaterialPropertiesResolver and keeping only one instance.");
                        Debug.LogFormat("The selected instance is: {0}", AssetDatabase.GUIDToAssetPath(instanceGuids[0]));
                        instance = AssetDatabase.LoadAssetAtPath<MaterialPropertiesResolver>(AssetDatabase.GUIDToAssetPath(instanceGuids[0]));

                        // Delete other instances
                        for (int i = 1; i < instanceGuids.Length; i++)
                        {
                            string instanceGuid = instanceGuids[i];
                            AssetDatabase.DeleteAsset(AssetDatabase.GUIDToAssetPath(instanceGuid));
                        }
                    }
                    else if (instanceGuids.Length == 0)
                    {
                        instance = ScriptableObject.CreateInstance<MaterialPropertiesResolver>();
                        instance.CollectMaterials();
                        System.IO.Directory.CreateDirectory("Assets/Resources/Bayat/Core");
                        AssetDatabase.CreateAsset(instance, "Assets/Resources/Bayat/Core/MaterialPropertiesResolver.asset");
                        AssetDatabase.SaveAssets();
                        AssetDatabase.Refresh();
                    }
                    else
                    {
                        instance = AssetDatabase.LoadAssetAtPath<MaterialPropertiesResolver>(AssetDatabase.GUIDToAssetPath(instanceGuids[0]));
                    }
#else
                    instance = Resources.Load<MaterialPropertiesResolver>("Bayat/Core/MaterialPropertiesResolver");
#endif
                    current = instance;
                }
                return current;
            }
        }

        [SerializeField]
        protected List<Material> materials = new List<Material>();
        [SerializeField]
        protected List<RuntimeMaterialProperties> materialsProperties = new List<RuntimeMaterialProperties>();

        protected Dictionary<string, RuntimeMaterialProperties> materialsLookup = new Dictionary<string, RuntimeMaterialProperties>();

        /// <summary>
        /// Gets the materials.
        /// </summary>
        public virtual List<Material> Materials
        {
            get
            {
                return this.materials;
            }
        }

        /// <summary>
        /// Gets the materials properties.
        /// </summary>
        public virtual List<RuntimeMaterialProperties> MaterialsProperties
        {
            get
            {
                return this.materialsProperties;
            }
        }

        public virtual Dictionary<string, RuntimeMaterialProperties> MaterialsLookup
        {
            get
            {
                if (this.materialsLookup.Count != this.materials.Count)
                {
                    this.materialsLookup.Clear();
                    for (int i = 0; i < this.materials.Count; i++)
                    {
                        this.materialsLookup.Add(this.materials[i].name, this.materialsProperties[i]);
                    }
                }
                return this.materialsLookup;
            }
        }

        public virtual void Reset()
        {
#if UNITY_EDITOR
            CollectMaterials();
#endif
        }

#if UNITY_EDITOR
        /// <summary>
        /// Collects the project materials and their properties.
        /// </summary>
        public void CollectMaterials()
        {

            // Remove NULL references
            if (RemoveNullReferences() > 0)
            {
                Undo.RecordObject(this, "Update MaterialPropertiesResolver List");
            }

            var materials = new List<Material>();
            foreach (var item in AssetReferenceResolver.Current.GuidToReference)
            {
                if (item.Value is Material)
                {
                    materials.Add((Material)item.Value);
                }
            }

            foreach (var item in materials)
            {
                var asset = item;
                if (asset is Material)
                {
                    Material material = (Material)asset;
                    MaterialProperty[] properties = MaterialEditor.GetMaterialProperties(new UnityEngine.Object[] { material });
                    RuntimeMaterialProperties runtimeProperties = null;
                    if (!this.materials.Contains(material))
                    {
                        this.materials.Add(material);
                        runtimeProperties = new RuntimeMaterialProperties();
                        this.materialsProperties.Add(runtimeProperties);
                    }
                    else
                    {
                        int index = this.materials.IndexOf(material);

                        if (index >= this.materialsProperties.Count)
                        {
                            runtimeProperties = new RuntimeMaterialProperties();

                            this.materialsProperties.Add(runtimeProperties);
                        }
                        else
                        {
                            runtimeProperties = this.materialsProperties[index];
                        }
                    }
                    runtimeProperties.Name = material.name;
                    for (int j = 0; j < properties.Length; j++)
                    {
                        MaterialProperty property = properties[j];
                        RuntimeMaterialProperty runtimeProperty = new RuntimeMaterialProperty(property.name, property.type);
                        int index = runtimeProperties.Properties.FindIndex(theProperty => theProperty.Name == property.name);
                        if (index == -1)
                        {
                            runtimeProperties.Properties.Add(runtimeProperty);
                        }
                        else
                        {
                            runtimeProperties.Properties[index] = runtimeProperty;
                        }
                    }
                }
            }

            // Fix for unity serialization
            EditorUtility.SetDirty(this);
        }
#endif

        /// <summary>
        /// Gets the material properties.
        /// </summary>
        /// <param name="material">The material</param>
        /// <returns>The material properties <see cref="RuntimeMaterialProperties"/></returns>
        public virtual RuntimeMaterialProperties GetMaterialProperties(Material material)
        {
            if (material.name.EndsWith(" (Instance)", System.StringComparison.InvariantCultureIgnoreCase))
            {
                this.MaterialsLookup.TryGetValue(material.name.Replace(" (Instance)", string.Empty), out var properties);
                return properties;
            }
            int index = this.materials.IndexOf(material);
            if (index == -1)
            {
                return null;
            }
            else
            {
                return this.materialsProperties[index];
            }
        }

        /// <summary>
        /// Checks whether if has any null references or not.
        /// </summary>
        /// <returns>True if has null references otherwise false</returns>
        public virtual bool HasNullReferences()
        {
            int index = 0;
            while (index < this.materials.Count)
            {
                if (this.materials[index] == null)
                {
                    return true;
                }
                else
                {
                    index++;
                }
            }
            return false;
        }

        /// <summary>
        /// Checks whether if has any unused references or not.
        /// </summary>
        /// <returns>True if has unused references otherwise false</returns>
        public virtual bool HasUnusedReferences()
        {
            int index = 0;
            while (index < this.materials.Count)
            {
                if (this.materials[index] != null && !AssetReferenceResolver.Current.Contains(this.materials[index]))
                {
                    return true;
                }
                else
                {
                    index++;
                }
            }
            return false;
        }

        /// <summary>
        /// Removes the null references.
        /// </summary>
        /// <returns>The count of removed references</returns>
        public virtual int RemoveNullReferences()
        {
            int index = 0;
            int removedCount = 0;
            while (index < this.materials.Count)
            {
                if (this.materials[index] == null)
                {
                    this.materials.RemoveAt(index);
                    this.materialsProperties.RemoveAt(index);
                    removedCount++;
                }
                else
                {
                    index++;
                }
            }
            return removedCount;
        }

        /// <summary>
        /// Removes all the unused references.
        /// </summary>
        /// <returns>The count of removed references</returns>
        public virtual int RemoveUnusedReferences()
        {
            int index = 0;
            int removedCount = 0;
            while (index < this.materials.Count)
            {
                if (this.materials[index] != null && !AssetReferenceResolver.Current.Contains(this.materials[index]))
                {
                    this.materials.RemoveAt(index);
                    this.materialsProperties.RemoveAt(index);
                    removedCount++;
                }
                else
                {
                    index++;
                }
            }
            return removedCount;
        }

    }

}