using System.Collections;

namespace Game.GameMode.Mode
{
    public class PausedInteractionGameMode : IGameMode
    {
        public ulong ClientId { get; set; }
        public GameModeManager GameModeManager { get; set; }
        public GameMode GameMode => GameMode.PausedInteraction;
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