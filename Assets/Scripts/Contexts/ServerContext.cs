using System;

namespace Contexts
{
    public class ServerContext : Context<ServerContext>
    {
        protected override Type ContextProviderType => typeof(IServerContextProvider);
    }
}