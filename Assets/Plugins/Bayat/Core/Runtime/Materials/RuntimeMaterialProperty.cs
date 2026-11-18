using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
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
        public RuntimeMaterialProperty(string name, UnityEngine.Rendering.ShaderPropertyType propType)
        {
            this.name = name;
            this.type = ConvertPropType(propType);
        }

        /// <summary>
        /// Converts the <see cref="MaterialProperty.PropType"/> to <see cref="RuntimeMaterialPropertyType"/>
        /// </summary>
        /// <param name="propType">The prop type</param>
        /// <returns><see cref="RuntimeMaterialPropertyType"/></returns>
        public static RuntimeMaterialPropertyType ConvertPropType(UnityEngine.Rendering.ShaderPropertyType propType)
        {
            RuntimeMaterialPropertyType type = RuntimeMaterialPropertyType.Color;
            switch (propType)
            {
                case UnityEngine.Rendering.ShaderPropertyType.Color:
                    type = RuntimeMaterialPropertyType.Color;
                    break;
                case UnityEngine.Rendering.ShaderPropertyType.Vector:
                    type = RuntimeMaterialPropertyType.Vector;
                    break;
                case UnityEngine.Rendering.ShaderPropertyType.Float:
                    type = RuntimeMaterialPropertyType.Float;
                    break;
                case UnityEngine.Rendering.ShaderPropertyType.Range:
                    type = RuntimeMaterialPropertyType.Range;
                    break;
                case UnityEngine.Rendering.ShaderPropertyType.Texture:
                    type = RuntimeMaterialPropertyType.Texture;
                    break;
            }
            return type;
        }
#endif

    }

}