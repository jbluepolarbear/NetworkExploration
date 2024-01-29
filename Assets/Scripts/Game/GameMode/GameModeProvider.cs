using System;
using Game.GameMode.Mode;

namespace Game.GameMode
{
    public static class GameModeProvider
    {
        public static IGameMode NewGameModeInstance(GameMode gameMode)
        {
            switch (gameMode)
            {
                case GameMode.PlayerControlled:
                    return new PlayerControlledGameMode();
                default:
                    throw new ArgumentOutOfRangeException(nameof(gameMode), gameMode, null);
            }
        }
    }
}