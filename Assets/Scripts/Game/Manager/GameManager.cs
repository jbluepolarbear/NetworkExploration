using System.Collections;
using Unity.Netcode;
using UnityEngine;
using NetworkPlayer = Game.Player.NetworkPlayer;

namespace Game.Manager
{
    public class GameManager : MonoBehaviour
    {
        private void OnGUI()
        {
            GUILayout.BeginArea(new Rect(10, 10, 300, 300));

            var networkManager = NetworkManager.Singleton;
            if (!networkManager.IsClient && !networkManager.IsServer)
            {
                if (GUILayout.Button("Host"))
                {
                    networkManager.StartHost();
                }

                if (GUILayout.Button("Client"))
                {
                    networkManager.StartClient();
                }

                if (GUILayout.Button("Server"))
                {
                    networkManager.StartServer();
                }
            }
            else
            {
                GUILayout.Label($"Mode: {(networkManager.IsHost ? "Host" : networkManager.IsServer ? "Server" : "Client")}");

                // "Random Teleport" button will only be shown to clients
                if (networkManager.IsClient)
                {
                    if (GUILayout.Button("Random Teleport"))
                    {
                        if (networkManager.LocalClient != null)
                        {
                            // Get `BootstrapPlayer` component from the player's `PlayerObject`
                            if (networkManager.LocalClient.PlayerObject.TryGetComponent(out NetworkPlayer bootstrapPlayer))
                            {
                                // Invoke a `ServerRpc` from client-side to teleport player to a random position on the server-side
                                bootstrapPlayer.RandomTeleportServerRpc();
                                StartCoroutine(PingPong(bootstrapPlayer));
                            }
                        }
                    }
                }
            }

            GUILayout.EndArea();
        }

        public IEnumerator PingPong(NetworkPlayer bootstrapPlayer)
        {
            var promise = bootstrapPlayer.StartYourEngines("Ping");
            yield return promise;
            if (promise.Fulfilled)
            {
                Debug.Log($"{promise.GetValue()}");
            }
            promise.Dispose();
        }
    }
}