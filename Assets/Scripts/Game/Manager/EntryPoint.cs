using Contexts;
using Data;
using Unity.Netcode;
using UnityEngine;

namespace Game.Manager
{
    public class EntryPoint : MonoBehaviour
    {
        private void OnGUI()
        {
            if (NetworkStartUpSettingsProvider.NetworkStartUpSettings != null)
            {
                Destroy(this);
                return;
            }
            GUILayout.BeginArea(new Rect(10, 10, 300, 300));

            var networkManager = NetworkManager.Singleton;
            if (!networkManager.IsClient && !networkManager.IsServer)
            {
                if (GUILayout.Button("Host"))
                {
                    networkManager.StartHost();
                    Destroy(this);
                    return;
                }

                if (GUILayout.Button("Client"))
                {
                    networkManager.StartClient();
                    ServerContext.Clear();
                    Destroy(this);
                    return;
                }

                if (GUILayout.Button("Server"))
                {
                    networkManager.StartServer();
                    ClientContext.Clear();
                    Destroy(this);
                    return;
                }
            }

            GUILayout.EndArea();
        }
    }
}