using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Bayat.SaveSystem.Demos
{

    public class StorageCopyDemoController : StorageDemoController
    {

        [SerializeField]
        protected InputField identifierField;
        [SerializeField]
        protected InputField secondIdentifierField;
        [SerializeField]
        protected Toggle replaceToggle;

        public override void DoAction()
        {
            SaveSystemAPI.CopyAsync(this.identifierField.text, this.secondIdentifierField.text, this.replaceToggle.isOn);
        }

    }

}