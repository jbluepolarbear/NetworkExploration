using System;

namespace Contexts
{
    public interface IContextProvider
    {
        Type ProviderType { get; }
        bool Active { get; }
        bool Ready { get; }
    }
}