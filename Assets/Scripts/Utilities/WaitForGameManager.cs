using Contexts;
using Game.Manager;
using UnityEngine;

namespace Utilities
{
    public class WaitForGameManagerServer : CustomYieldInstruction
    {
        public override bool keepWaiting => !ServerContext.Has<GameManager>();
    }
    
    public class WaitForGameManagerClient : CustomYieldInstruction
    {
        public override bool keepWaiting => !ClientContext.Has<GameManager>();
    }
}