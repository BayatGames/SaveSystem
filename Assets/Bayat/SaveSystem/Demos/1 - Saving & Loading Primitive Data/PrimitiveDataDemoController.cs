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

        public override void Save()
        {
            SaveSystemAPI.SaveAsync(this.identifierField.text, this.dataField.text);
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