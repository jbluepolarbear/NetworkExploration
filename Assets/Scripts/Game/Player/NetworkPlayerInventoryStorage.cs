using System.Collections;
using Contexts;
using Game.Inventory;
using Game.Player.UserState;
using Game.World;
using UserState;

namespace Game.Player
{
    public class NetworkPlayerInventoryStorage : NetworkInventoryStorage
    {
        protected override IEnumerator StartClient()
        {
            yield return base.StartClient();
            AutoSync = true;
        }
        
        protected override UserStateStorage GetUserStateStorage()
        {
            return GetComponent<NetworkPlayerUserState>().PlayerUserState;
        }
        
        protected override UserStateInventory GetInventory()
        {
            var userState = GetUserStateStorage();
            return userState?.GetOrMakeSingleUserStateEntry<UserStateInventory>();
        }
    }
}