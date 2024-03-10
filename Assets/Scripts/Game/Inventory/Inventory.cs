using System;
using System.Collections.Generic;
using System.Linq;
using Utilities;
using Math = System.Math;

namespace Game.Inventory
{
    public enum MetaData
    {
        None,
        Equipped,
        QuestItem,
        Restorable,
    }
    
    [Serializable]
    public class InventoryItemStack
    {
        public int ItemId;
        /// <summary>
        /// Id of the inventory item in this inventory
        /// Useful for splitting stacks and merging stacks
        /// </summary>
        public int InventoryId;
        public int Quantity;
        public MetaData MetaData;
        public uint MetaDataId;
    }
    
    public interface IInventory
    {
        IReadOnlyList<InventoryItemStack> Items { get; }
        int Count { get; }
        bool HasItemId(int itemId);
        int GetQuantity(int itemId);
        /// <summary>
        /// Add quantity to an existing stack or create a new stack
        /// </summary>
        /// <param name="itemId"></param>
        /// <param name="quantity"></param>
        /// <returns>stack inventory Id</returns>
        int AddQuantity(int itemId, int quantity);
        /// <summary>
        /// Add quantity to an existing stack if inventoryId is provided; otherwise, create a new stack
        /// </summary>
        /// <param name="itemId"></param>
        /// <param name="quantity"></param>
        /// <param name="inventoryId"></param>
        /// <returns>stack inventory Id</returns>
        int AddQuantityToStack(int itemId, int quantity, int inventoryId = -1);
        /// <summary>
        /// Set quantity of an existing stack
        /// </summary>
        /// <param name="itemId"></param>
        /// <param name="quantity"></param>
        /// <returns>stack inventory Id</returns>
        int SubtractQuantity(int itemId, int quantity);
        /// <summary>
        /// Subtract quantity from an existing stack if inventoryId is provided; otherwise, subtract from first matching stacks
        /// </summary>
        /// <param name="itemId"></param>
        /// <param name="quantity"></param>
        /// <param name="inventoryId"></param>
        /// <returns>stack inventory Id</returns>
        int SubtractQuantityFromStack(int itemId, int quantity, int inventoryId = -1);
        IEnumerable<InventoryItemStack> GetItemStacks(int itemId);
        void SplitStack(int inventoryId, int quantity);
        void MergeStacks(int inventoryId1, int inventoryId2);
        bool Changed { get; }
        void ClearChanged();
    }
    
    [Serializable]
    public class Inventory : IInventory
    {
        private int _inventoryId = 1;
        private List<InventoryItemStack> _items = new ();
        public IReadOnlyList<InventoryItemStack> Items => _items;
        public int Count => _items.Count;
        public bool HasItemId(int itemId) => _items.Exists(item => item.ItemId == itemId);
        
        public void SetQuantityOfStack(int itemId, int quantity, int inventoryId = -1)
        {
            if (quantity <= 0)
            {
                return;
            }

            if (inventoryId == -1)
            {
                AddQuantity(itemId, quantity);
                return;
            }
            
            for (var i = 0; i < _items.Count; i++)
            {
                var inventoryItem = _items[i];
                if (inventoryItem.InventoryId != inventoryId)
                {
                    continue;
                }
                inventoryItem.Quantity = quantity;
                _items[i] = inventoryItem;
                SetChanged();
                return;
            }
            
            _items.Add(new InventoryItemStack
            {
                ItemId = itemId,
                InventoryId = inventoryId,
                Quantity = quantity
            });
            SetChanged();
        }

        public int GetQuantity(int itemId)
        {
            return _items.Where(item => item.ItemId == itemId).Sum(item => item.Quantity);
        }
        
        public int AddQuantity(int itemId, int quantity)
        {
            Assert.True(itemId > 0);
            
            if (quantity <= 0)
            {
                return -1;
            }
            
            var existingItem = _items.FirstOrDefault(item => item.ItemId == itemId);
            if (existingItem == null)
            {
                var newInventoryId = _inventoryId++;
                _items.Add(new InventoryItemStack
                {
                    ItemId = itemId,
                    InventoryId = newInventoryId,
                    Quantity = quantity
                });
                SetChanged();
                return newInventoryId;
            }
            
            existingItem.Quantity += quantity;
            SetChanged();
            return existingItem.InventoryId;
        }
        
        public int AddQuantityToStack(int itemId, int quantity, int inventoryId = -1)
        {
            if (quantity <= 0)
            {
                return -1;
            }

            if (inventoryId == -1)
            {
                var newInventoryId = _inventoryId++;
                _items.Add(new InventoryItemStack
                {
                    ItemId = itemId,
                    InventoryId = newInventoryId,
                    Quantity = quantity
                });
                SetChanged();
                return newInventoryId;
            }
            
            var existingItem = _items.FirstOrDefault(item => item.InventoryId == inventoryId);
            Assert.True(existingItem != null);
            
            existingItem.Quantity += quantity;
            SetChanged();
            return existingItem.InventoryId;
        }
        
        public int SubtractQuantity(int itemId, int quantity)
        {
            if (quantity <= 0)
            {
                return -1;
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
                if (quantityToRemove <= 0)
                {
                    break;
                }
                
                if (inventoryItem.Quantity >= 0)
                {
                    var amountToRemove = Math.Min(quantityToRemove, inventoryItem.Quantity);
                    inventoryItem.Quantity -= amountToRemove;
                    _items[i] = inventoryItem;
                    quantityToRemove -= amountToRemove;
                    if (inventoryItem.Quantity <= 0)
                    {
                        _items.RemoveAt(i);
                        continue;
                    }
                    else if (quantityToRemove == 0)
                    {
                        SetChanged();
                        return inventoryItem.InventoryId;
                    }
                }
            }
            SetChanged();
            return -1;
        }
        
        public int SubtractQuantityFromStack(int itemId, int quantity, int inventoryId = -1)
        {
            if (quantity <= 0)
            {
                return -1;
            }

            if (inventoryId == -1)
            {
                return SubtractQuantity(itemId, quantity);
            }
            
            var existingItem = _items.FirstOrDefault(item => item.InventoryId == inventoryId);
            Assert.True(existingItem != null);
            
            existingItem.Quantity -= quantity;
            var stack = existingItem.InventoryId;
            if (existingItem.Quantity <= 0)
            {
                _items.Remove(existingItem);
                stack = -1;
            }
            SetChanged();
            return stack;
        }
        
        public IEnumerable<InventoryItemStack> GetItemStacks(int itemId)
        {
            return _items.Where(item => item.ItemId == itemId);
        }
        
        public void SplitStack(int inventoryId, int quantity)
        {
            var existingItem = _items.FirstOrDefault(item => item.InventoryId == inventoryId);
            Assert.True(existingItem != null);
            Assert.True(existingItem.Quantity > quantity);
            
            existingItem.Quantity -= quantity;
            _items.Add(new InventoryItemStack
            {
                ItemId = existingItem.ItemId,
                InventoryId = _inventoryId++,
                Quantity = quantity
            });
            SetChanged();
        }
        
        public void MergeStacks(int inventoryId1, int inventoryId2)
        {
            var stack1 = _items.FirstOrDefault(item => item.InventoryId == inventoryId1);
            var stack2 = _items.FirstOrDefault(item => item.InventoryId == inventoryId2);
            Assert.True(stack1 != null);
            Assert.True(stack2 != null);
            Assert.True(stack1.ItemId == stack2.ItemId);
            
            stack1.Quantity += stack2.Quantity;
            _items.Remove(stack2);
            SetChanged();
        }

        protected void SetChanged()
        {
            Changed = true;
        }
        
        public bool Changed { get; private set; }
        public void ClearChanged()
        {
            Changed = false;
        }
    }
}