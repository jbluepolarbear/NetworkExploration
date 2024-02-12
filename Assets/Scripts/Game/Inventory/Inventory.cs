using System;
using System.Collections.Generic;
using System.Linq;
using NetLib.Utility;

namespace Game.Inventory
{
    [Serializable]
    public struct InventoryItemStack
    {
        public int ItemId;
        /// <summary>
        /// Id of the inventory item in this inventory
        /// Useful for splitting stacks and merging stacks
        /// </summary>
        public int InventoryId;
        public int Quantity;
    }
    
    [Serializable]
    public class Inventory
    {
        private int _inventoryId = 0;
        private List<InventoryItemStack> _items = new ();
        public IReadOnlyList<InventoryItemStack> Items => _items;
        public int Count => _items.Count;
        public bool HasItemId(int itemId) => _items.Exists(item => item.ItemId == itemId);

        public int GetQuantity(int itemId)
        {
            return _items.Where(item => item.ItemId == itemId).Sum(item => item.Quantity);
        }
        
        public void AddQuantity(int itemId, int quantity)
        {
            if (quantity <= 0)
            {
                return;
            }
            
            var existingItem = _items.FirstOrDefault(item => item.ItemId == itemId);
            if (existingItem.ItemId == 0)
            {
                _items.Add(new InventoryItemStack
                {
                    ItemId = itemId,
                    InventoryId = _inventoryId++,
                    Quantity = quantity
                });
                return;
            }
            
            existingItem.Quantity += quantity;
        }
        
        public void AddQuantityToStack(int itemId, int quantity, int inventoryId = -1)
        {
            if (quantity <= 0)
            {
                return;
            }

            if (inventoryId == -1)
            {
                _items.Add(new InventoryItemStack
                {
                    ItemId = itemId,
                    InventoryId = _inventoryId++,
                    Quantity = quantity
                });
                return;
            }
            
            var existingItem = _items.FirstOrDefault(item => item.InventoryId == inventoryId);
            Assert.True(existingItem.ItemId != 0);
            
            existingItem.Quantity += quantity;
        }
        
        public void SubtractQuantity(int itemId, int quantity)
        {
            if (quantity <= 0)
            {
                return;
            }

            Assert.True(quantity <= GetQuantity(itemId));
            
            var quantityToRemove = quantity;
            for (var i = 0; i < _items.Count;)
            {
                var inventoryItem = _items[i];
                if (inventoryItem.ItemId != itemId)
                {
                    i++;
                    continue;
                }
                if (inventoryItem.Quantity >= 0)
                {
                    inventoryItem.Quantity -= quantityToRemove;
                    _items[i] = inventoryItem;
                    return;
                }
                quantityToRemove -= inventoryItem.Quantity;
                _items.RemoveAt(i);
            }
        }
        
        public void SubtractQuantityFromStack(int itemId, int quantity, int inventoryId = -1)
        {
            if (quantity <= 0)
            {
                return;
            }

            if (inventoryId == -1)
            {
                SubtractQuantity(itemId, quantity);
                return;
            }
            
            var existingItem = _items.FirstOrDefault(item => item.InventoryId == inventoryId);
            Assert.True(existingItem.ItemId != 0);
            
            existingItem.Quantity -= quantity;
        }
        
        public IEnumerable<InventoryItemStack> GetItemStacks(int itemId)
        {
            return _items.Where(item => item.ItemId == itemId);
        }
        
        public void SplitStack(int inventoryId, int quantity)
        {
            var existingItem = _items.FirstOrDefault(item => item.InventoryId == inventoryId);
            Assert.True(existingItem.ItemId != 0);
            Assert.True(existingItem.Quantity > quantity);
            
            existingItem.Quantity -= quantity;
            _items.Add(new InventoryItemStack
            {
                ItemId = existingItem.ItemId,
                InventoryId = _inventoryId++,
                Quantity = quantity
            });
        }
        
        public void MergeStacks(int inventoryId1, int inventoryId2)
        {
            var stack1 = _items.FirstOrDefault(item => item.InventoryId == inventoryId1);
            var stack2 = _items.FirstOrDefault(item => item.InventoryId == inventoryId2);
            Assert.True(stack1.ItemId != 0);
            Assert.True(stack2.ItemId != 0);
            Assert.True(stack1.ItemId == stack2.ItemId);
            
            stack1.Quantity += stack2.Quantity;
            _items.Remove(stack2);
        }
    }
}