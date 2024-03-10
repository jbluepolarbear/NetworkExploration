using System;
using Game.GameMode.Mode;

namespace Game.GameMode
{
    public static class GameModeProvider
    {
        public static IGameMode NewGameModeInstance(GameModes gameModes)
        {
            switch (gameModes)
            {
                case GameModes.PlayerControlled:
                    return new PlayerControlledGameMode();
                case GameModes.PausedInteraction:
                    return new PausedInteractionGameMode();
                default:
                    throw new ArgumentOutOfRangeException(nameof(gameModes), gameModes, $"GameMode: {gameModes} not found.");
            }
        }
    }
}