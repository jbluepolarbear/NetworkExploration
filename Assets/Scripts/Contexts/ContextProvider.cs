using System;
using System.Collections;
using Extensions.GameObjects;
using UnityEngine;

namespace Contexts
{
    public abstract class ContextProvider : NetworkBehaviourExt, IContextProvider
    {
        public abstract Type ProviderType { get; }
        public abstract bool Active { get; }
        public abstract bool Ready { get; }
    }
    
    public abstract class ContextProvider<T> : ContextProvider
    {
        public override Type ProviderType => typeof(T);
        public override bool Active => _active;
        public override bool Ready => _ready;

        protected void IsReady()
        {
            _ready = true;
        }

        private void OnValidate()
        {
            IContextProvider self = GetComponent<IContextProvider>();
            ServerContext.Clear();
            ClientContext.Clear();
        }
        
#if UNITY_EDITOR
        private new void OnDestroy()
        {
            _active = false;
            ServerContext.Clear();
            ClientContext.Clear();
        }
#endif

        private bool _active = true;
        private bool _ready = false;
    }
}