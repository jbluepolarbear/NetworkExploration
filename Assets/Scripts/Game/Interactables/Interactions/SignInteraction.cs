using System.Collections;
using System.Collections.Generic;
using Extensions.GameObjects;
using Extensions.GameObjects.Rpc;
using Unity.Netcode;
using UnityEngine;
using Utilities;

namespace Game.Interactables.Interactions
{
    public class SignInteraction : NetworkBehaviourExt, IInteraction
    {
        private IEnumerator RunInteractionCoroutine(Promise<IInteractionResult> promise)
        {
            var rpcPromise = CallOnServer(ExecuteServerRoutine);
            yield return rpcPromise;
            promise.Fulfill();
        }
        
        [RpcTargetServer(1)]
        public IEnumerator ExecuteServerRoutine(ulong clientId, RpcPromise promise)
        {
            Debug.Log("Sign Interaction");
            yield return new WaitForSeconds(1);
            promise.Fulfill();
        }

        public InteractionType _interactionType = InteractionType.Action;
        public InteractionType Type => _interactionType;

        public Promise<IInteractionResult> ExecuteClient()
        {
            var promise = new Promise<IInteractionResult>();
            StartCoroutine(RunInteractionCoroutine(promise));
            return promise;
        }

        protected override IEnumerator StartServer()
        {
            yield break;
        }

        protected override IEnumerator StartClient()
        {
            yield break;
        }
    }
}