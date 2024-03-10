using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Contexts;
using Extensions.GameObjects;
using Extensions.GameObjects.Rpc;
using Game.World;
using Unity.Netcode;
using UnityEngine;
using UserState;
using Utilities;

namespace Game.Inventory
{
    public class NetworkInventoryStorage : NetworkBehaviourExt, IInventory
    {
        private UserStateInventory _inventory;
        private UserStateInventory Inventory => _inventory;
        public bool ClientSynced => !_serverHasSync && _syncCoroutine == null;
        public bool AutoSync { get; set; } = false;
        protected override IEnumerator StartServer()
        {
            _inventory = GetInventory();
            while (_inventory == null)
            {
                _inventory = GetInventory();
                yield return null;
            }
            NeedsSyncClientRpc();
        }

        protected override IEnumerator StartClient()
        {
            _serverHasSync = true;
            yield break;
        }

        private bool _clientNeedsSync;
        private bool _serverHasSync;
        protected override void NetworkFixedUpdate()
        {
            base.NetworkFixedUpdate();
            if (IsServer && _clientNeedsSync)
            {
                _clientNeedsSync = false;
                NeedsSyncClientRpc();
            }
            
            if (AutoSync && IsClient && !ClientSynced)
            {
                SyncClient();
            }
        }

        private Promise _syncPromise;
        public Promise SyncClient()
        {
            if (IsHost || ClientSynced)
            {
                return Promise.FullfilledPromise;
            }
            
            if (_syncPromise != null)
            {
                return _syncPromise;
            }
            
            _serverHasSync = false;
            _syncPromise = new Promise();
            _syncCoroutine = StartCoroutine(SyncCoroutine());
            return _syncPromise;
        }
        
        private IEnumerator SyncCoroutine()
        {
            var promise = CallOnServer<List<InventoryItemStack>>(SyncServerRoutine);
            yield return promise;
            if (promise.Error != null)
            {
                _syncPromise.FillError(new PromiseError
                {
                    Code = PromiseErrorCodes.RpcError,
                    Reason = $"Code: {promise.Error.Code}, Reason: {promise.Error.Reason}"
                });
                yield break;
            }

            _inventory ??= new UserStateInventory();
            _inventory.Inventory = new Inventory();
            foreach (var inventoryItemStack in promise.GetValue())
            {
                _inventory.Inventory.SetQuantityOfStack(inventoryItemStack.ItemId, inventoryItemStack.Quantity, inventoryItemStack.InventoryId);
            }
            
            _syncCoroutine = null;
            _syncPromise.Fulfill();
        }

        [RpcTargetServer(1)]
        public IEnumerator SyncServerRoutine(ulong clientId, RpcPromise<List<InventoryItemStack>> promise)
        {
            promise.Fulfill(_inventory.Inventory.Items);
            yield break;
        }

        private Coroutine _syncCoroutine;
        [ClientRpc]
        public void NeedsSyncClientRpc()
        {
            if (IsHost)
            {
                return;
            }
            
            _serverHasSync = true;
        }

        protected virtual UserStateStorage GetUserStateStorage()
        {
            if (ServerContext.Get<WorldUserStateProvider>()?.Ready == true)
            {
                return ServerContext.Get<WorldUserStateProvider>().WorldUserState;
            }
            return null;
        }
        
        protected virtual UserStateInventory GetInventory()
        {
            var userState = GetUserStateStorage();
            var worldIdentifier = GetComponent<WorldIdentifier>();
            return userState?.GetOrMakeUserStateEntryForOwnerId<UserStateInventory>(worldIdentifier.WorldId);
        }

        public IReadOnlyList<InventoryItemStack> Items => Inventory.Inventory.Items;
        public int Count => Inventory.Inventory.Count;
        public bool Empty => !Inventory.Inventory.Items.Any(stack => stack.Quantity > 0);

        public bool HasItemId(int itemId)
        {
            return Inventory.Inventory.HasItemId(itemId);
        }

        public int GetQuantity(int itemId)
        {
            return Inventory.Inventory.GetQuantity(itemId);
        }

        public int AddQuantity(int itemId, int quantity)
        {
            if (IsClient && !IsHost)
            {
                throw new InvalidOperationException("Cannot AddQuantity on client");
            }
            
            _clientNeedsSync = true;
            return Inventory.Inventory.AddQuantity(itemId, quantity);
        }

        public int AddQuantityToStack(int itemId, int quantity, int inventoryId = -1)
        {
            if (IsClient && !IsHost)
            {
                throw new InvalidOperationException("Cannot AddQuantityToStack on client");
            }
            
            _clientNeedsSync = true;
            return Inventory.Inventory.AddQuantityToStack(itemId, quantity, inventoryId);
        }

        public int SubtractQuantity(int itemId, int quantity)
        {
            if (IsClient && !IsHost)
            {
                throw new InvalidOperationException("Cannot SubtractQuantity on client");
            }
            
            _clientNeedsSync = true;
            return Inventory.Inventory.SubtractQuantity(itemId, quantity);
        }

        public int SubtractQuantityFromStack(int itemId, int quantity, int inventoryId = -1)
        {
            if (IsClient && !IsHost)
            {
                throw new InvalidOperationException("Cannot SubtractQuantityFromStack on client");
            }
            
            _clientNeedsSync = true;
            return Inventory.Inventory.SubtractQuantityFromStack(itemId, quantity, inventoryId);
        }

        public IEnumerable<InventoryItemStack> GetItemStacks(int itemId)
        {
            return Inventory.Inventory.GetItemStacks(itemId);
        }

        public void SplitStack(int inventoryId, int quantity)
        {
            if (IsClient && !IsHost)
            {
                throw new InvalidOperationException("Cannot SplitStack on client");
            }
            
            Inventory.Inventory.SplitStack(inventoryId, quantity);
            _clientNeedsSync = true;
        }

        public void MergeStacks(int inventoryId1, int inventoryId2)
        {
            if (IsClient && !IsHost)
            {
                throw new InvalidOperationException("Cannot MergeStacks on client");
            }
            
            Inventory.Inventory.MergeStacks(inventoryId1, inventoryId2);
            _clientNeedsSync = true;
        }

        public bool Changed => Inventory.Inventory.Changed;
        public void ClearChanged()
        {
            Inventory.Inventory.ClearChanged();
        }
    }
}