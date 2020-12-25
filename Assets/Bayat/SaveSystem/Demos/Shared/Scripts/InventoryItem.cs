using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bayat.SaveSystem.Demos
{

    [CreateAssetMenu(menuName = "Bayat/Save System/Demos/Inventory Item")]
    public class InventoryItem : ScriptableObject
    {

        public string itemName = string.Empty;
        public Texture2D image;

    }

}