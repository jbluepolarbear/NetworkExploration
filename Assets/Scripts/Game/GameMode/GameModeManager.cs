using System.Collections;
using Contexts;
using Extensions.GameObjects;
using UnityEngine;

namespace Game.GameMode
{
    public class GameModeManager : ContextProvider<GameModeManager>, IServerContextProvider, IClientContextProvider
    {
        public IGameMode ActiveGameMode { get; private set; }

        public IEnumerator SetGameMode(IGameMode nextGameMode)
        {
            var previousGameMode = ActiveGameMode;
            ActiveGameMode = null;
            if (previousGameMode != null)
            {
                yield return previousGameMode.ExitGameMode();
            }

            yield return nextGameMode.EnterGameMode();
            ActiveGameMode = nextGameMode;
        }

        protected override IEnumerator StartServer()
        {
            yield return null;
        }

        protected override IEnumerator StartClient()
        {
            yield return null;
        }
    }
}