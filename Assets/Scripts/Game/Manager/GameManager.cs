using System;
using System.Collections;
using Contexts;
using Unity.Netcode;
using UnityEngine;
using NetworkPlayer = Game.Player.NetworkPlayer;

namespace Game.Manager
{
    // Main start and ready of game
    public class GameManager : ContextProvider<GameManager>, IServerContextProvider, IClientContextProvider
    {
    }
}