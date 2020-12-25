using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Bayat.SaveSystem.Demos
{

    public class RawDataDemoController : DemoController
    {

        [SerializeField]
        protected InputField identifierField;
        [SerializeField]
        protected InputField dataField;

        public override void Save()
        {
            SaveSystemAPI.WriteAllTextAsync(this.identifierField.text, this.dataField.text);

            // You can use SaveSystemAPI.WriteAllBytes for writting raw binary data
        }

        public override async void Load()
        {
            this.dataField.text = await SaveSystemAPI.ReadAllTextAsync(this.identifierField.text);

            // You can use SaveSystemAPI.ReadAllBytes to read the raw binary data
        }

        public override void Delete()
        {
            SaveSystemAPI.DeleteAsync(this.identifierField.text);
        }

    }

}