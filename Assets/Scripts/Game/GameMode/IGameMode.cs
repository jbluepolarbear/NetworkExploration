using System.Collections;
using Input;

namespace Game.GameMode
{
    public interface IGameMode
    {
        IEnumerator EnterGameMode();
        void UpdateGameMode(IInputState inputState); 
        IEnumerator ExitGameMode();
    }
}