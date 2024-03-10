using System.Collections;
using System.Collections.Generic;
using Contexts;
using Extensions.GameObjects.Rpc;
using Game.GameMode.Mode;
using Game.Interactables;
using UnityEngine;
using UnityEngine.InputSystem;
using Utilities;

namespace Game.GameMode
{
    public class GameModeManager : ContextProvider<GameModeManager>, IServerContextProvider, IClientContextProvider
    {
        private Dictionary<ulong, IGameMode> _gameModes = new Dictionary<ulong, IGameMode>();

        public bool TryGetGameMode(out IGameMode gameMode)
        {
            gameMode = null;
            return NetworkManager.LocalClient != null && GetGameMode(NetworkManager.LocalClient.ClientId, out gameMode);
        }

        public GameModes GetActiveGameMode
        {
            get
            {
                if (NetworkManager.LocalClient != null && GetGameMode(NetworkManager.LocalClient.ClientId, out IGameMode gameMode))
                {
                    return gameMode.GameMode;
                }

                return GameModes.None;
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

        public RpcPromise SetGameModeServer(GameModes gameModes)
        {
            return CallOnServer(SetGameModeServerRoutine, gameModes);
        }

        [RpcTargetServer(1)]
        public IEnumerator SetGameModeServerRoutine(GameModes gameModes, ulong clientId, RpcPromise promise)
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
                yield return CallOnClient(StartGameModeClientRoutine, clientId, gameModes);
            }
            yield return StartGameMode(clientId, gameModes);

            promise.Fulfill();
        }

        protected override void NetworkFixedUpdate()
        {
            InputSystem.Update();
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

        private IEnumerator StartGameMode(ulong clientId, GameModes gameModes)
        {
            var newGameMode = GameModeProvider.NewGameModeInstance(gameModes);
            newGameMode.ClientId = clientId;
            newGameMode.GameModeManager = this;
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
        public IEnumerator StartGameModeClientRoutine(GameModes gameModes, RpcPromise promise)
        {
            yield return StartGameMode(NetworkManager.LocalClient.ClientId, gameModes);
            promise.Fulfill();
        }

        [RpcTargetClient(2)]
        public IEnumerator ExitGameModeClientRoutine(RpcPromise promise)
        {
            yield return ExitGameMode(NetworkManager.LocalClient.ClientId);
            promise.Fulfill();
        }

        public Promise<IGameMode> SwitchGameMode(GameModes gameModes)
        {
            var promise = new Promise<IGameMode>();
            StartCoroutine(SwitchGameModeRoutine(gameModes, promise));
            return promise;
        }

        private IEnumerator SwitchGameModeRoutine(GameModes gameModes, Promise<IGameMode> promise)
        {
            var gameModeRpcPromise = SetGameModeServer(gameModes);
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

            if (TryGetGameMode(out var gameMode))
            {
                Debug.Log($"Game Mode: {gameMode}");
                promise.Fulfill(gameMode);
                yield break;
            }

            promise.FillError(new PromiseError
            {
                Code = PromiseErrorCodes.NotFound,
                Reason = $"Game Mode not found after switching to {gameModes}"
            });
        }
    }
}