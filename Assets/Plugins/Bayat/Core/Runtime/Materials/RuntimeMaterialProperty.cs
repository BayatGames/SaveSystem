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
        public RuntimeMaterialProperty(string name, MaterialProperty.PropType propType)
        {
            this.name = name;
            this.type = ConvertPropType(propType);
        }

        /// <summary>
        /// Converts the <see cref="MaterialProperty.PropType"/> to <see cref="RuntimeMaterialPropertyType"/>
        /// </summary>
        /// <param name="propType">The prop type</param>
        /// <returns><see cref="RuntimeMaterialPropertyType"/></returns>
        public static RuntimeMaterialPropertyType ConvertPropType(MaterialProperty.PropType propType)
        {
            RuntimeMaterialPropertyType type = RuntimeMaterialPropertyType.Color;
            switch (propType)
            {
                case MaterialProperty.PropType.Color:
                    type = RuntimeMaterialPropertyType.Color;
                    break;
                case MaterialProperty.PropType.Vector:
                    type = RuntimeMaterialPropertyType.Vector;
                    break;
                case MaterialProperty.PropType.Float:
                    type = RuntimeMaterialPropertyType.Float;
                    break;
                case MaterialProperty.PropType.Range:
                    type = RuntimeMaterialPropertyType.Range;
                    break;
                case MaterialProperty.PropType.Texture:
                    type = RuntimeMaterialPropertyType.Texture;
                    break;
            }
            return type;
        }
#endif

    }

}