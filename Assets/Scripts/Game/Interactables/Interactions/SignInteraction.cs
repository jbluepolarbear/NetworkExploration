using System.Collections;
using System.Collections.Generic;
using Contexts;
using Extensions.GameObjects;
using Extensions.GameObjects.Rpc;
using Game.GameMode;
using Unity.Netcode;
using UnityEngine;
using Utilities;

namespace Game.Interactables.Interactions
{
    public class SignInteraction : NetworkBehaviourExt, IInteraction
    {
        private IEnumerator RunInteractionCoroutine(Promise<IInteractionResult> promise)
        {
            var gameModeManager = ClientContext.Get<GameModeManager>();
            var previousGameMode = gameModeManager.GetActiveGameMode;
            var gameModeRpcPromise = gameModeManager
                .SetGameModeServer(GameMode.GameMode.PausedInteraction);
            yield return gameModeRpcPromise;
            if (gameModeRpcPromise.Error != null)
            {
                promise.FillError(new PromiseError
                {
                    Code = PromiseErrorCodes.RpcError,
                    Reason = $"Code: {gameModeRpcPromise.Error.Code}, Reason: {gameModeRpcPromise.Error.Reason}"
                });
                yield break;
            }
            
            var rpcPromise = CallOnServer(ExecuteServerRoutine);
            yield return rpcPromise;
            if (rpcPromise.Error != null)
            {
                promise.FillError(new PromiseError
                {
                    Code = PromiseErrorCodes.RpcError,
                    Reason = $"Code: {rpcPromise.Error.Code}, Reason: {rpcPromise.Error.Reason}"
                });
                yield break;
            }
            
            gameModeRpcPromise = gameModeManager
                .SetGameModeServer(previousGameMode);
            yield return gameModeRpcPromise;
            if (gameModeRpcPromise.Error != null)
            {
                promise.FillError(new PromiseError
                {
                    Code = PromiseErrorCodes.RpcError,
                    Reason = $"Code: {gameModeRpcPromise.Error.Code}, Reason: {gameModeRpcPromise.Error.Reason}"
                });
                yield break;
            }
            
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