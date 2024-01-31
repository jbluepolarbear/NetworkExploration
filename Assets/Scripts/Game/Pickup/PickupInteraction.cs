using System.Collections;
using Extensions.GameObjects;
using Extensions.GameObjects.Rpc;
using Game.Interactables;
using UnityEngine;
using Utilities;

namespace Game.Pickup
{
    public class PickupInteraction : NetworkBehaviourExt, IInteraction
    {
        protected override IEnumerator StartServer()
        {
            yield return null;
        }

        protected override IEnumerator StartClient()
        {
            yield return null;
        }

        public InteractionType Type => InteractionType.Action;
        public Promise<IInteractionResult> ExecuteClient()
        {
            var promise = new Promise<IInteractionResult>();
            StartCoroutine(ExecuteClientRoutine(promise));
            return promise;
        }
        
        [RpcTargetServer(1)]
        public IEnumerator ExecuteServerRoutine(ulong clientId, RpcPromise promise)
        {
            Debug.Log($"Picked up {gameObject.name}");
            promise.Fulfill();
            yield return new WaitForSeconds(1);
            NetworkObject.Despawn(true);
        }

        public IEnumerator ExecuteClientRoutine(Promise<IInteractionResult> promise)
        {
            var rpcPromise = CallOnServer(ExecuteServerRoutine);
            yield return rpcPromise;
            if (rpcPromise.Error != null)
            {
                promise.FillError(new PromiseError
                {
                    Code = PromiseErrorCodes.RpcError,
                    Reason = $"Code: {rpcPromise.Error.Code}, Reason: {rpcPromise.Error.Reason}"
                });
            }
            else
            {
                promise.Fulfill();
            }
        }
    }
}