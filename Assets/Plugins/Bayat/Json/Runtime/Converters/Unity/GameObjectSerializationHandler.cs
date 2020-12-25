using System.Collections.Generic;
using UnityEngine;

namespace Bayat.Json
{

    /// <summary>
    /// Handles the serialization of GameObject.
    /// </summary>
    public class GameObjectSerializationHandler : MonoBehaviour
    {

        [SerializeField]
        protected bool serializeChildren = false;
        [SerializeField]
        protected bool serializeComponents = false;
        [SerializeField]
        protected bool serializeExcludedChildren = false;
        [SerializeField]
        protected bool serializeExcludedComponents = false;
        [SerializeField]
        protected List<Transform> excludedChildren = new List<Transform>();
        [SerializeField]
        protected List<Component> excludedComponents = new List<Component>();

        /// <summary>
        /// Gets whether serialize children or not.
        /// </summary>
        public virtual bool SerializeChildren
        {
            get
            {
                return this.serializeChildren;
            }
        }

        /// <summary>
        /// Gets whether serialize components or not.
        /// </summary>
        public virtual bool SerializeComponents
        {
            get
            {
                return this.serializeComponents;
            }
        }

        /// <summary>
        /// Gets whether serialize excluded children instead or not.
        /// </summary>
        public virtual bool SerializeExcludedChildren
        {
            get
            {
                return this.serializeExcludedChildren;
            }
        }

        /// <summary>
        /// Gets whether serialize excluded components instead or not.
        /// </summary>
        public virtual bool SerializeExcludedComponents
        {
            get
            {
                return this.serializeExcludedComponents;
            }
        }

        /// <summary>
        /// Gets the excluded components list.
        /// </summary>
        public virtual List<Component> ExcludedComponents
        {
            get
            {
                return this.excludedComponents;
            }
        }

        /// <summary>
        /// Gets the excluded children list.
        /// </summary>
        public virtual List<Transform> ExcludedChildren
        {
            get
            {
                return this.excludedChildren;
            }
        }

        /// <summary>
        /// Checks whether should serialize the component or not.
        /// </summary>
        /// <param name="component">The component</param>
        /// <returns>True if should serialize otherwise false</returns>
        public virtual bool ShouldSerializeComponent(Component component)
        {
            if (this.SerializeExcludedComponents)
            {
                return this.ExcludedComponents.Contains(component);
            }
            return !this.ExcludedComponents.Contains(component);
        }

        /// <summary>
        /// Checks whether should serialize the child or not.
        /// </summary>
        /// <param name="child">The child</param>
        /// <returns>True if should serialize child otherwise false</returns>
        public virtual bool ShouldSerializeChild(Transform child)
        {
            if (this.SerializeExcludedChildren)
            {
                return this.ExcludedChildren.Contains(child);
            }
            return !this.ExcludedChildren.Contains(child);
        }

        private void OnValidate()
        {
            if (this.ExcludedComponents != null)
            {
                this.ExcludedComponents.RemoveAll(component =>
                {
                    return component.gameObject != this.gameObject;
                });
            }
            if (this.ExcludedChildren != null)
            {
                this.ExcludedChildren.RemoveAll(child =>
                {
                    return child.parent != this.transform;
                });
            }
        }

    }

}