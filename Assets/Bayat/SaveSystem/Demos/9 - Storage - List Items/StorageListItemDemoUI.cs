using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Bayat.SaveSystem.Demos
{

    public class StorageListItemDemoUI : MonoBehaviour
    {

        [SerializeField]
        protected StorageListItemsDemoController controller;
        [SerializeField]
        protected Text identifierText;

        public virtual string Identifier
        {
            get
            {
                return this.identifierText.text;
            }
        }

        public virtual void Initialize(StorageListItemsDemoController controller, string identifier)
        {
            this.controller = controller;
            this.identifierText.text = identifier;
        }

        public virtual void Delete()
        {
            this.controller.Delete(this);
        }

    }

}