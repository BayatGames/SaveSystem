using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

namespace Bayat.SaveSystem.Demos
{

    public class ComplexDataDemoController : DemoController
    {

        [System.Serializable]
        public class PlayerData
        {

            public string name = string.Empty;
            public float currentHealth = 0f;
            public InventoryData inventory = new InventoryData();

        }

        [System.Serializable]
        public class InventoryData
        {

            public List<InventorySlot> slots = new List<InventorySlot>();

        }

        [System.Serializable]
        public class InventorySlot
        {

            public InventoryItem item;
            public int amount = 0;

        }

        [SerializeField]
        protected InputField identifierField;
        [SerializeField]
        protected InputField playerNameField;
        [SerializeField]
        protected Slider playerCurrentHealthSlider;
        [SerializeField]
        protected InputField playerInventorySlotCount;
        [SerializeField]
        protected List<InventoryItem> availableItems;
        [SerializeField]
        protected PlayerData playerData = new PlayerData();
        [SerializeField]
        protected PlayerData defaultPlayerData = new PlayerData();

        public override void Save()
        {
            this.playerData.name = this.playerNameField.text;
            this.playerData.currentHealth = this.playerCurrentHealthSlider.value;
            SaveSystemAPI.SaveAsync(this.identifierField.text, this.playerData);
            Debug.Log("Player data saved successfully");
        }

        public override async void Load()
        {
            if (!await SaveSystemAPI.ExistsAsync(this.identifierField.text))
            {
                Debug.Log("Player data not found");
                Debug.Log("Using default player data instead");
                this.playerData = this.defaultPlayerData;
                return;
            }
            this.playerData = await SaveSystemAPI.LoadAsync<PlayerData>(this.identifierField.text);
            this.playerNameField.text = this.playerData.name;
            this.playerCurrentHealthSlider.value = this.playerData.currentHealth;
            this.playerInventorySlotCount.text = this.playerData.inventory.slots.Count.ToString();
            Debug.Log("Player data loaded successfully");
        }

        public override void Delete()
        {
            SaveSystemAPI.DeleteAsync(this.identifierField.text);
            Debug.Log("Player data deleted successfully");
        }

        public virtual void RandomizeInventory()
        {
            this.playerData.inventory.slots.Clear();
            int slotCount = int.Parse(this.playerInventorySlotCount.text);
            for (int i = 0; i < slotCount; i++)
            {
                InventorySlot slot = new InventorySlot();
                slot.item = GetRandomItem();
                slot.amount = Random.Range(0, 6);
                this.playerData.inventory.slots.Add(slot);
            }
        }

        public InventoryItem GetRandomItem()
        {
            return this.availableItems[Random.Range(0, this.availableItems.Count)];
        }

    }

}