using System.Collections;
using Input;

namespace Game.GameMode
{
    public enum GameMode
    {
        None,
        PlayerControlled,
        PausedInteraction, // This is a game mode to prevent the player from moving while interacting with an object
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