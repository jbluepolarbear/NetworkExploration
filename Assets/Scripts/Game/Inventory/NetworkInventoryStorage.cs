using System.Collections;
using Contexts;
using Extensions.GameObjects;
using Game.World;

namespace Game.Inventory
{
    public class NetworkInventoryStorage : NetworkBehaviourExt
    {
        private UserStateInventory _inventory;
        public UserStateInventory Inventory => _inventory;
        protected override IEnumerator StartServer()
        {
            while (ServerContext.Get<WorldUserStateProvider>()?.Ready == false)
            {
                yield return null;
            }

            var worldUserState = ServerContext.Get<WorldUserStateProvider>().WorldUserState;
            _inventory =
                worldUserState.GetOrMakeUserStateEntryForOwnerId<UserStateInventory>(NetworkObject.NetworkObjectId);
        }

        protected override IEnumerator StartClient()
        {
            yield break;
        }
    }
}