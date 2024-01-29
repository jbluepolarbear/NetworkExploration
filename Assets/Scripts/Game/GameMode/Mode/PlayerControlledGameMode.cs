using System.Collections;
using Input;

namespace Game.GameMode.Mode
{
    public class PlayerControlledGameMode : IGameMode
    {
        public ulong ClientId { get; set; }
        public GameMode GameMode { get; }
        public IEnumerator EnterGameMode()
        {
            yield return null;
        }

        public void UpdateGameMode()
        {
        }

        public IEnumerator ExitGameMode()
        {
            yield return null;
        }
    }
}