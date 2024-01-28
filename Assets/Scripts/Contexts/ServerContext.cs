using System;
using Extensions.GameObjects;
using UnityEngine;

namespace Contexts
{
    public class ServerContext : Context<ServerContext>
    {
        protected override Type ContextProviderType => typeof(IServerContextProvider);
    }
}