using System;
using System.Collections;
using System.Collections.Generic;
using Contexts;
using Extensions.GameObjects;
using Extensions.GameObjects.Rpc;
using Unity.Netcode;
using UnityEngine;

namespace Game.GameMode
{
    public class GameModeManager : ContextProvider<GameModeManager>, IServerContextProvider, IClientContextProvider
    {
        private Dictionary<ulong, IGameMode> _gameModes = new Dictionary<ulong, IGameMode>();

        public GameMode GetActiveGameMode
        {
            get
            {
                if (NetworkManager.LocalClient != null && GetGameMode(NetworkManager.LocalClient.ClientId, out IGameMode gameMode))
                {
                    return gameMode.GameMode;
                }

                return GameMode.None;
            }
        }

        private bool GetGameMode(ulong clientId, out IGameMode gameMode)
        {
            return (_gameModes.TryGetValue(clientId, out gameMode)) ;
        }

        private void ClearGameMode(ulong clientId)
        {
            _gameModes.Remove(clientId);
        }

        public RpcPromise SetGameModeServer(GameMode gameMode)
        {
            return CallOnServer(SetGameModeServerRoutine, gameMode);
        }

        [RpcTargetServer(1)]
        public IEnumerator SetGameModeServerRoutine(GameMode gameMode, ulong clientId, RpcPromise promise)
        {
            if (GetGameMode(clientId, out var previousGameMode))
            {
                if (NetworkManager.LocalClient != null && NetworkManager.LocalClient.ClientId != clientId)
                {
                    yield return CallOnClient(ExitGameModeClientRoutine, clientId);
                }

                yield return previousGameMode.ExitGameMode();
            }

            if (NetworkManager.LocalClient != null && NetworkManager.LocalClient.ClientId != clientId)
            {
                yield return CallOnClient(StartGameModeClientRoutine, clientId, gameMode);
            }
            yield return StartCoroutine(StartGameMode(clientId, gameMode));

            promise.Fulfill();
        }

        private void FixedUpdate()
        {
            foreach (var gameMode in _gameModes)
            {
                gameMode.Value.UpdateGameMode();
            }
        }

        protected override IEnumerator StartServer()
        {
            yield return null;
        }

        protected override IEnumerator StartClient()
        {
            yield return null;
        }

        private IEnumerator StartGameMode(ulong clientId, GameMode gameMode)
        {
            var newGameMode = GameModeProvider.NewGameModeInstance(gameMode);
            newGameMode.ClientId = clientId;
            yield return newGameMode.EnterGameMode();
            _gameModes[clientId] = newGameMode;
            yield return null;
        }

        private IEnumerator ExitGameMode(ulong clientId)
        {
            if (GetGameMode(clientId, out var gameMode))
            {
                yield return gameMode.ExitGameMode();
            }

            ClearGameMode(clientId);
            yield return null;
        }

        [RpcTargetClient(1)]
        public IEnumerator StartGameModeClientRoutine(GameMode gameMode, RpcPromise promise)
        {
            yield return StartGameMode(NetworkManager.LocalClient.ClientId, gameMode);
            promise.Fulfill();
        }

        [RpcTargetClient(2)]
        public IEnumerator ExitGameModeClientRoutine(RpcPromise promise)
        {
            yield return ExitGameMode(NetworkManager.LocalClient.ClientId);
            promise.Fulfill();
        }
    }
}