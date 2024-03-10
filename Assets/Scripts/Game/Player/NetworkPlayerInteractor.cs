using System;
using System.Collections;
using System.Collections.Generic;
using Contexts;
using Extensions.GameObjects;
using Game.GameMode;
using UnityEngine;
using Utilities;

namespace Game.Interactables
{
    [DisallowMultipleComponent]
    public class NetworkPlayerInteractor : NetworkBehaviourExt
    {
        private HashSet<IInteractable> _interactables = new ();
        public IInteractable Target { get; private set; }
        public bool CanRunInteraction => Target is { Interactable: true } && !RunningInteraction;
        public bool RunningInteraction { get; private set; }
        private void OnTriggerEnter(Collider other)
        {
            var interactable = other.GetComponent<IInteractable>();
            if (interactable != null)
            {
                _interactables.Add(interactable);
                UpdateTarget();
            }
        }
        
        private void OnTriggerExit(Collider other)
        {
            var interactable = other.GetComponent<IInteractable>();
            if (interactable != null)
            {
                _interactables.Remove(interactable);
                UpdateTarget();
            }
        }

        protected override void NetworkFixedUpdate()
        {
            base.NetworkFixedUpdate();
            PruneInteractables();
            UpdateTarget();
        }
        
        private void PruneInteractables()
        {
            _interactables.RemoveWhere(x => x.IsUnityNull());
        }
        
        public void ClearInteractables()
        {
            _interactables.Clear();
            UpdateTarget();
        }

        private void UpdateTarget()
        {
            if (RunningInteraction)
            {
                return;
            }
            
            var forward = transform.forward;
            var position = transform.position;
            var closest = float.MaxValue;
            IInteractable target = null;
            foreach (var interactable in _interactables)
            {
                if (interactable.Interactable == false)
                {
                    continue;
                }
                
                var vector = interactable.Position - position;
                var distance = Mathf.Abs(Vector3.Dot(forward, vector));
                if (distance < closest)
                {
                    closest = distance;
                    target = interactable;
                }
            }

            Target = target;
        }

        public void RunInteraction(InteractionType type)
        {
            if (Target == null || RunningInteraction)
            {
                return;
            }

            RunningInteraction = true;
            StartCoroutine(RunInteractionRoutine(type));
        }
        
        private IEnumerator RunInteractionRoutine(InteractionType type)
        {
            Debug.Log($"Running Interaction: {type}");
            var promise = Target.RunInteraction(type);
            yield return promise;
            if (promise.Error != null)
            {
                Debug.LogError(promise.Error.Reason);
            }
            RunningInteraction = false;
            Debug.Log($"Finished Running Interaction: {type}");
        }
        
        protected override IEnumerator StartServer()
        {
            yield break;
        }

        protected override IEnumerator StartClient()
        {
            yield break;
        }
    }
}