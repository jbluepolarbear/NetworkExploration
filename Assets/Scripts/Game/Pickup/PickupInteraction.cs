using System.Collections;
using Extensions.GameObjects;
using Game.GameMode;
using Game.Interactables;
using Unity.Netcode;
using UnityEngine;
using Utilities;

namespace Game.Pickup
{
    public class PickupInteraction : NetworkBehaviourExt, IInteraction
    {
        public GameObject View;
        protected override IEnumerator StartServer()
        {
            yield return null;
        }

        protected override IEnumerator StartClient()
        {
            yield return null;
        }

        public InteractionType Type => InteractionType.Action;
        public GameModes RequiredGameModes => GameModes.PlayerControlled;

        public Promise<IInteractionResult> ExecuteClient()
        {
            var promise = new Promise<IInteractionResult>();
            // using regular rpc for fire and forger
            ExecuteServerRpc();
            
            View.SetActive(false);
            
            promise.Fulfill();
            return promise;
        }
        
        [ServerRpc(RequireOwnership = false)]
        public void ExecuteServerRpc(ServerRpcParams serverRpcParams = default)
        {
            Debug.Log($"Picked up {gameObject.name}");
            NetworkObject.Despawn(true);
        }
    }
}