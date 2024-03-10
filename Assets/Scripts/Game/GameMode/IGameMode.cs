using System;
using System.Collections;
using UnityEngine;

namespace Game.GameMode
{
    [Flags]
    public enum GameModes
    {
        None = 0,
        PlayerControlled = 1 << 0,
        PausedInteraction = 1 << 1, // This is a game mode to prevent the player from moving while interacting with an object
    }
    public interface IGameMode
    {
        ulong ClientId { get; set; }
        Transform OcclusionTarget { get; }
        float OcclusionRadius { get; }
        GameModeManager GameModeManager { get; set; }
        GameModes GameMode { get; }
        IEnumerator EnterGameMode();
        void UpdateGameMode(); 
        IEnumerator ExitGameMode();
    }
}