using System.Collections.Generic;
using Unity.Netcode;

namespace Extensions
{
    public static class NetworkBehaviourExtensions
    {
        public static NetworkObject GetNetworkObjectForId(this NetworkBehaviour networkBehaviour, ulong networkObjectId)
        {
            return networkBehaviour.NetworkManager.SpawnManager.SpawnedObjects.GetValueOrDefault(networkObjectId);
        }
    }
}