using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bayat.SaveSystem.Demos
{

    public abstract class DemoController : MonoBehaviour
    {

        [SerializeField]
        protected SaveSystemSettingsPreset settings;

        protected virtual void Awake()
        {
            if (settings == null)
            {
                return;
            }
            settings.ApplyTo(SaveSystemSettings.DefaultSettings);
        }

        public abstract void Save();

        public abstract void Load();

        public abstract void Delete();

    }

}