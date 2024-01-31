using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Extensions.GameObjects;
using Unity.Netcode;
using UnityEngine;
using Utilities;

namespace Game.Interactables
{
    [DisallowMultipleComponent]
    public class InteractionRunner : NetworkBehaviourExt, IInteractable
    {
        private NetworkVariable<bool> _interactable = new();
        
        private void Awake()
        {
            var interactions = GetComponents<IInteraction>();
            foreach (var interaction in interactions)
            {
                _interactions.Add(interaction.Type, interaction);
            }
        }
        
        protected override IEnumerator StartServer()
        {
            yield return new WaitForGameManagerServer();
            _interactable.Value = true;
        }

        protected override IEnumerator StartClient()
        {
            yield return new WaitForGameManagerClient();
        }

        public Vector3 Position => transform.position;
        public bool Interactable => _interactable.Value;
        private Dictionary<InteractionType, IInteraction> _interactions = new();
        public bool HasInteraction(InteractionType type) => _interactions.Any(x => x.Key == type);

        public Promise<IInteractionResult> RunInteraction(InteractionType type)
        {
            if (!IsClient)
            {
                var promise = new Promise<IInteractionResult>();
                promise.FillError(new PromiseError
                {
                    Code = PromiseErrorCodes.NotAllowed,
                    Reason = "Not client"
                });
                return promise;
            }
            
            if (!_interactions.TryGetValue(type, out var interaction))
            {
                var promise = new Promise<IInteractionResult>();
                promise.FillError(new PromiseError
                {
                    Code = PromiseErrorCodes.NotFound,
                    Reason = "Interaction not found"
                });
                return promise;
            }

            return interaction.ExecuteClient();
        }
    }
}