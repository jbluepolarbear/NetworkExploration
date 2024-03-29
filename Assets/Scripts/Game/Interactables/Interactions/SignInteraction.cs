﻿using System.Collections;
using Contexts;
using Extensions.GameObjects;
using Extensions.GameObjects.Rpc;
using Game.GameMode;
using Game.GameMode.Mode;
using UnityEngine;
using Utilities;
using NetworkPlayer = Game.Player.NetworkPlayer;

namespace Game.Interactables.Interactions
{
    public class SignInteraction : NetworkBehaviourExt, IInteraction
    {
        private IEnumerator RunInteractionCoroutine(Promise<IInteractionResult> promise)
        {
            var gameModeManager = ClientContext.Get<GameModeManager>();
            var previousGameMode = gameModeManager.GetActiveGameMode;
            var gameModeRpcPromise = gameModeManager
                .SetGameModeServer(GameMode.GameModes.PausedInteraction);
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
            
            gameModeManager.TryGetGameMode(out var gameMode);
            Debug.Log($"Game Mode: {gameMode}");
            var pausedGameMode = (PausedInteractionGameMode) gameMode;
            
            var networkPlayer = NetworkManager.LocalClient.PlayerObject.GetComponent<NetworkPlayer>();
            pausedGameMode.OcclusionTarget = networkPlayer.transform;
            
            var rpcPromise = CallOnServer<string>(ExecuteServerRoutine);
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
            
            Debug.Log($"Sign Interaction: {rpcPromise.Value}");
            
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
        public IEnumerator ExecuteServerRoutine(ulong clientId, RpcPromise<string> promise)
        {
            promise.Fulfill("Sign Interaction From Server");
            yield break;
        }

        public InteractionType _interactionType = InteractionType.Action;
        public InteractionType Type => _interactionType;
        public GameModes RequiredGameModes => GameModes.PlayerControlled;

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