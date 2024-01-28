using System;
using UnityEngine;

namespace Contexts
{
    public class ClientContext : Context<ClientContext>
    {
        protected override Type ContextProviderType => typeof(IClientContextProvider);
    }
}