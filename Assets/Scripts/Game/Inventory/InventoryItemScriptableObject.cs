using UnityEngine;

namespace Game.Inventory
{
    [CreateAssetMenu(fileName = "InventoryItem", menuName = "Inventory/New Item")]
    public class InventoryItemScriptableObject : ScriptableObject
    {
        public int Id;
        public string NameKey;
        public string DescriptionKey;
    }
}