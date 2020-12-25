using System;
using System.Collections.Generic;
using UnityEngine;

namespace Bayat.Core
{

    /// <summary>
    /// A wrapper for material properties.
    /// </summary>
    [Serializable]
    public sealed class RuntimeMaterialProperties
    {

        [SerializeField]
        private string name;
        [SerializeField]
        private List<RuntimeMaterialProperty> properties = new List<RuntimeMaterialProperty>();

        /// <summary>
        /// The material name.
        /// </summary>
        public string Name
        {
            get
            {
                return this.name;
            }
            set
            {
                this.name = value;
            }
        }

        /// <summary>
        /// The material properties.
        /// </summary>
        public List<RuntimeMaterialProperty> Properties
        {
            get
            {
                return this.properties;
            }
        }

    }

}