using System;
using System.Collections;
using Contexts;
using Data;
using Unity.Netcode;
using UnityEngine;
using NetworkPlayer = Game.Player.NetworkPlayer;

namespace Game.Manager
{
    // Main start and ready of game
    public class GameManager : ContextProvider<GameManager>, IServerContextProvider, IClientContextProvider
    {
        public void Start()
        {
            var networkManager = NetworkManager.Singleton;
            if (NetworkStartUpSettingsProvider.NetworkStartUpSettings != null)
            {
                
            }
        }
        
        protected override IEnumerator StartServer()
        {
            IsReady();
            yield return null;
        }

        protected override IEnumerator StartClient()
        {
            IsReady();
            yield return null;
        }
    }
}