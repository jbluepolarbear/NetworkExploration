using System;
using UnityEngine;

namespace Game.Inventory
{
    [Serializable]
    [CreateAssetMenu(fileName = "InventoryItem", menuName = "Inventory/New Item")]
    public class InventoryItemScriptableObject : ScriptableObject
    {
        public int Id;
        public string NameKey;
        public string DescriptionKey;
        public Sprite Icon;
        public GameObject Prefab;
    }
}