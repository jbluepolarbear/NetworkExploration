using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Extensions.GameObjects;
using Game.GameMode;
using Utilities;
using Unity.Netcode;
using UnityEngine;

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

        public bool Interactable
        {
            get => _interactable.Value;
            set
            {
                // May only be set on server
                Assert.True(IsServer);
                _interactable.Value = value;
            }
        }

        private Dictionary<InteractionType, IInteraction> _interactions = new();
        public bool AllowedToInteract(InteractionType type, GameModes gameMode)
        {
            if (_interactions.TryGetValue(type, out var interaction))
            {
                return (interaction.RequiredGameModes & gameMode) != 0;
            }

            return false;
        }

        public bool HasInteraction(InteractionType type) => _interactions.TryGetValue(type, out _);

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