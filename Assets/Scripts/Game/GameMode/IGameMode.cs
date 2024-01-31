using System.Collections;
using Input;

namespace Game.GameMode
{
    public enum GameMode
    {
        None,
        PlayerControlled,
    }
    public interface IGameMode
    {
        ulong ClientId { get; set; }
        GameModeManager GameModeManager { get; set; }
        GameMode GameMode { get; }
        IEnumerator EnterGameMode();
        void UpdateGameMode(); 
        IEnumerator ExitGameMode();
    }
}