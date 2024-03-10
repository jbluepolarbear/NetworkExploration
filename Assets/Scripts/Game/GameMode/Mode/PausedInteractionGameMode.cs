using System.Collections;
using UnityEngine;

namespace Game.GameMode.Mode
{
    public class PausedInteractionGameMode : IGameMode
    {
        public ulong ClientId { get; set; }
        public Transform OcclusionTarget { get; set; }
        public float OcclusionRadius => 2.0f;
        public GameModeManager GameModeManager { get; set; }
        public GameModes GameMode => GameModes.PausedInteraction;
        
        public IEnumerator EnterGameMode()
        {
            yield break;
        }

        public void UpdateGameMode()
        {
        }

        public IEnumerator ExitGameMode()
        {
            yield break;
        }
    }
}