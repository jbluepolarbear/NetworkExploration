using System;

namespace Contexts
{
    public class ClientContext : Context<ClientContext>
    {
        protected override Type ContextProviderType => typeof(IClientContextProvider);
    }
}