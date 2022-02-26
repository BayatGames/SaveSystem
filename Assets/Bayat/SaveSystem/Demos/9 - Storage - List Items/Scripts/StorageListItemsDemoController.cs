using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Bayat.SaveSystem.Demos
{

    public class StorageListItemsDemoController : StorageDemoController
    {

        [SerializeField]
        protected StorageListItemDemoUI itemPrefab;
        [SerializeField]
        protected Transform container;
        [SerializeField]
        protected List<StorageListItemDemoUI> items;

        public override async void DoAction()
        {
            DestroyItems();
            List<string> itemsIdentifier = await SaveSystemAPI.LoadCatalogAsync();
            foreach (string itemIdentifier in itemsIdentifier)
            {
                CreateItem(itemIdentifier);
            }
        }

        public virtual void CreateItem(string identifier)
        {
            StorageListItemDemoUI item = Instantiate<StorageListItemDemoUI>(this.itemPrefab, this.container);
            item.Initialize(this, identifier);
            this.items.Add(item);
        }

        public virtual void DestroyItems()
        {
            for (int i = 0; i < this.items.Count; i++)
            {
                Destroy(this.items[i].gameObject);
            }
            this.items.Clear();
        }

        public virtual async void Delete(StorageListItemDemoUI item)
        {
            await SaveSystemAPI.DeleteAsync(item.Identifier);
            Destroy(item.gameObject);

            // Refresh the list of items
            DoAction();
        }

    }

}