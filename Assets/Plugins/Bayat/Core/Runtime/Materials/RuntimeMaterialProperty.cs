using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using UnityEngine.Rendering;
#endif

namespace Bayat.Core
{

    /// <summary>
    /// The runtime material property.
    /// </summary>
    [Serializable]
    public struct RuntimeMaterialProperty
    {

        [SerializeField]
        private string name;
        [SerializeField]
        private RuntimeMaterialPropertyType type;

        /// <summary>
        /// The property name.
        /// </summary>
        public string Name
        {
            get
            {
                return this.name;
            }
        }

        /// <summary>
        /// The material property type.
        /// </summary>
        public RuntimeMaterialPropertyType Type
        {
            get
            {
                return this.type;
            }
        }

        public RuntimeMaterialProperty(string name, RuntimeMaterialPropertyType type)
        {
            this.name = name;
            this.type = type;
        }

#if UNITY_EDITOR
        public RuntimeMaterialProperty(string name, ShaderPropertyType propType)
            : this(name, ConvertPropType(propType))
        {
        }

#pragma warning disable 618 // Keep the existing overloads for callers on older Unity versions.
        public RuntimeMaterialProperty(string name, MaterialProperty.PropType propType)
            : this(name, ConvertPropType(propType))
        {
        }

        public static RuntimeMaterialPropertyType ConvertPropType(MaterialProperty.PropType propType)
        {
            return ConvertPropType((ShaderPropertyType)propType);
        }
#pragma warning restore 618

        /// <summary>
        /// Converts a Unity shader property type to the type stored in the material database.
        /// </summary>
        /// <param name="propType">The prop type</param>
        /// <returns><see cref="RuntimeMaterialPropertyType"/></returns>
        public static RuntimeMaterialPropertyType ConvertPropType(ShaderPropertyType propType)
        {
            RuntimeMaterialPropertyType type = RuntimeMaterialPropertyType.Color;
            switch (propType)
            {
                case ShaderPropertyType.Color:
                    type = RuntimeMaterialPropertyType.Color;
                    break;
                case ShaderPropertyType.Vector:
                    type = RuntimeMaterialPropertyType.Vector;
                    break;
                case ShaderPropertyType.Float:
                    type = RuntimeMaterialPropertyType.Float;
                    break;
                case ShaderPropertyType.Range:
                    type = RuntimeMaterialPropertyType.Range;
                    break;
                case ShaderPropertyType.Texture:
                    type = RuntimeMaterialPropertyType.Texture;
                    break;
            }
            return type;
        }
#endif

    }

}
