using System.Collections;
using System.Collections.Generic;
using Contexts;
using Game.Inventory;
using UnityEngine.Serialization;

namespace Game.Manager
{
    public class InventoryDatabase : ContextProvider<InventoryDatabase>, IServerContextProvider, IClientContextProvider
    {
        public List<InventoryItemScriptableObject> InventoryItemDefinitions;
        protected override IEnumerator StartServer()
        {
            yield break;
        }

        protected override IEnumerator StartClient()
        {
            yield break;
        }
        
        public InventoryItemScriptableObject GetInventoryItem(int id)
        {
            return InventoryItemDefinitions.Find(item => item.Id == id);
        }
    }
}