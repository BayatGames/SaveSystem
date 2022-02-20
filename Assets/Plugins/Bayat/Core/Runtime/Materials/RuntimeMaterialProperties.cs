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
        [HideInInspector]
        [SerializeField]
        private string guid;
        [SerializeField]
        private List<RuntimeMaterialProperty> properties = new List<RuntimeMaterialProperty>();

        /// <summary>
        /// Gets or sets the material name.
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
        /// Gets or sets the material GUID.
        /// </summary>
        public string Guid
        {
            get
            {
                return this.guid;
            }
            set
            {
                this.guid = value;
            }
        }

        /// <summary>
        /// Gets the material properties.
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