using Contexts;
using Game.Manager;
using UnityEngine;

namespace Utilities
{
    public class WaitForGameManager : CustomYieldInstruction
    {
        public override bool keepWaiting => !ClientContext.Has<GameManager>();
    }
}