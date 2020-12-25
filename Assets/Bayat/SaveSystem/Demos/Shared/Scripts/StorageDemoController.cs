using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bayat.SaveSystem.Demos
{

    public abstract class StorageDemoController : MonoBehaviour
    {

        [SerializeField]
        protected SaveSystemSettingsPreset settings;
        [SerializeField]
        protected string[] dummyItems = new string[] { "demo.txt", "demo2.txt", "demo3.txt", "demo-folder/demo4.txt" };

        protected virtual void Awake()
        {
            if (settings == null)
            {
                return;
            }
            settings.ApplyTo(SaveSystemSettings.DefaultSettings);
        }

        public virtual async void CreateDummyItems()
        {
            for (int i = 0; i < this.dummyItems.Length; i++)
            {
                await SaveSystemAPI.WriteAllTextAsync(this.dummyItems[i], "Sample text data");
                Debug.LogFormat("Created dummy item with identifier: {0}", this.dummyItems[i]);
            }
        }

        public abstract void DoAction();

    }

}