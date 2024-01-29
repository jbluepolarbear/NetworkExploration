﻿using System.Collections;
using Contexts;
using Extensions;
using Extensions.GameObjects;
using Extensions.GameObjects.Rpc;
using Game.GameMode;
using Game.Manager;
using Unity.Netcode;
using UnityEngine;

namespace Game.Player
{
    public class NetworkPlayer : NetworkBehaviourExt
    {
        /// <summary>
        /// If this method is invoked on the client instance of this player, it will invoke a `ServerRpc` on the server-side.
        /// If this method is invoked on the server instance of this player, it will teleport player to a random position.
        /// </summary>
        /// <remarks>
        /// Since a `NetworkTransform` component is attached to this player, and the authority on that component is set to "Server",
        /// this transform's position modification can only be performed on the server, where it will then be replicated down to all clients through `NetworkTransform`.
        /// </remarks>
        [ServerRpc]
        public void RandomTeleportServerRpc()
        {
            var oldPosition = transform.position;
            transform.position = GetRandomPositionOnXYPlane();
            var newPosition = transform.position;
            print($"{nameof(RandomTeleportServerRpc)}() -> {nameof(OwnerClientId)}: {OwnerClientId} --- {nameof(oldPosition)}: {oldPosition} --- {nameof(newPosition)}: {newPosition}");
        }

        private static Vector3 GetRandomPositionOnXYPlane()
        {
            return new Vector3(Random.Range(-3f, 3f), Random.Range(-3f, 3f), 0f);
        }

        public RpcPromise<string> StartYourEngines(string start)
        {
            return CallOnServer<string, string>(StartYourEnginesServerRoutine, start);
        }

        [RpcTargetServer(1)]
        public IEnumerator StartYourEnginesServerRoutine(string start, ulong clientId, RpcPromise<string> promise)
        {
            var i = 0;
            while (i < 10)
            {
                ++i;
                yield return null;
            }

            Debug.Log($"{start}");
            promise.Fulfill("Pong");
        }

        protected override IEnumerator StartServer()
        {
            yield return null;
        }

        protected override IEnumerator StartClient()
        {
            while (!ClientContext.Has<GameManager>())
            {
                yield return null;
            }
            yield return ClientContext.Get<GameModeManager>().SetGameModeServer(GameMode.GameMode.PlayerControlled);
            yield return null;
        }
    }
}