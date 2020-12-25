using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Bayat.SaveSystem.Storage;

namespace Bayat.SaveSystem.Demos
{

    public class StorageBackupDemoController : StorageDemoController
    {

        [SerializeField]
        protected InputField identifierField;
        protected StorageBackup latestBackup;

        public override void DoAction()
        {
            if (latestBackup == null)
            {
                GetLatestBackup();
            }
            if (latestBackup == null)
            {
                CreateBackup();
            }
        }

        public virtual void CreateBackup()
        {
            SaveSystemAPI.CreateBackupAsync(this.identifierField.text);
        }

        public virtual void RestoreLatestBackup()
        {
            SaveSystemAPI.RestoreLatestBackupAsync(this.identifierField.text);
        }

        public virtual async void GetLatestBackup()
        {
            this.latestBackup = await SaveSystemAPI.GetLatestBackupAsync(this.identifierField.text);
        }

    }

}