using System;
using System.Collections.Generic;
using System.Linq;
using Utilities;
using UnityEngine;

namespace Contexts
{
    public abstract class Context<TContext> : UnitySingleton<TContext> where TContext : Context<TContext>
    {
        [SerializeField]
        private IContextProvider[] _contextProvidersInChildren;

        private void OnValidate()
        {
            Clear();
        }
        
        public static T Get<T>() where T : class, IContextProvider
        {
            if (Instance == null)
            {
                return null;
            }
            if (!Has<T>())
            {
                Debug.LogWarning($"IContextProvider for type {typeof(T).Name} was not found in the registry.");
                return null;
            }
            return (T) Instance._contextProviders[typeof(T)];
        }
        
        public static bool Has<T>() where T : IContextProvider
        {
            if (Instance == null)
            {
                return false;
            }
            return Instance._contextProviders.ContainsKey(typeof(T));
        }

        public static void Initialize()
        {
            Instance.RegisterContextProviders();
        }

        public static void Clear()
        {
            if (Instance == null)
            {
                return;
            }
            Instance._contextProviders.Clear();
#if UNITY_EDITOR
            Instance._contextProvidersInChildren = Instance._contextProviders.Select(pair => (IContextProvider) pair.Value).ToArray();
#endif
            Instance.ResetInstance();
        }

        protected abstract Type ContextProviderType { get; }
        private void RegisterContextProviders()
        {
            IContextProvider[] contextProviders = GetComponentsInChildren(ContextProviderType).Cast<IContextProvider>().ToArray();
            foreach (IContextProvider contextProvider in contextProviders)
            {
                RegisterProvider(contextProvider);
            }
        }
        
        private void RegisterProvider(IContextProvider contextProvider)
        {
            if (!contextProvider.Active)
            {
                Debug.LogWarning($"IContextProvider for type {contextProvider.GetType().Name} was inactive; skipping registration. Most likely component was removed from GameObject.");
                return;
            }
            
            _contextProviders[contextProvider.ProviderType] = contextProvider;
        }

        private Dictionary<Type, IContextProvider> _contextProviders = new Dictionary<Type, IContextProvider>();
    }
}