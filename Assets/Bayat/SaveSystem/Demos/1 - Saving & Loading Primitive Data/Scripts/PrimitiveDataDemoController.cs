using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

namespace Bayat.SaveSystem.Demos
{

    public class PrimitiveDataDemoController : DemoController
    {

        [SerializeField]
        protected InputField identifierField;
        [SerializeField]
        protected InputField dataField;

        public override async void Save()
        {
            Debug.Log("Saving...");
            await SaveSystemAPI.SaveAsync(this.identifierField.text, this.dataField.text);
            Debug.Log("Saved successfully.");
        }

        public override async void Load()
        {
            this.dataField.text = await SaveSystemAPI.LoadAsync<string>(this.identifierField.text);
        }

        public override void Delete()
        {
            SaveSystemAPI.DeleteAsync(this.identifierField.text);
        }

    }

}