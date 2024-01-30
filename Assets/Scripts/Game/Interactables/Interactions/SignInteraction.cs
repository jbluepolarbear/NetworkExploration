using System.Collections;
using System.Collections.Generic;
using Extensions.GameObjects;
using Extensions.GameObjects.Rpc;
using Unity.Netcode;
using UnityEngine;
using Utilities;

namespace Game.Interactables.Interactions
{
    public class SignInteraction : NetworkBehaviourExt, IInteractable, IInteraction
    {
        private NetworkVariable<bool> _interactable = new NetworkVariable<bool>(false);
        protected override IEnumerator StartServer()
        {
            yield return new WaitForGameManager();
            _interactable.Value = true;
        }

        protected override IEnumerator StartClient()
        {
            yield return new WaitForGameManager();
        }

        public bool Interactable => _interactable.Value;
        public IReadOnlyList<IInteraction> Interactions { get; }
        
        /// <summary>
        /// Expected to be run by client
        /// </summary>
        /// <param name="interaction"></param>
        /// <returns></returns>
        public Promise<IInteractionResult> RunInteraction(IInteraction interaction)
        {
            if (!IsClient)
            {
                var promise = new Promise<IInteractionResult>();
                promise.FillError(new PromiseError
                {
                    Code = PromiseErrorCodes.NotAllowed,
                    Reason = "Not client"
                });
                return promise;
            }

            return interaction.ExecuteClient();
        }
        
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
        
        public Promise<IInteractionResult> ExecuteClient()
        {
            var promise = new Promise<IInteractionResult>();
            StartCoroutine(RunInteractionCoroutine(promise));
            return promise;
        }
    }
}