using System.Collections;
using System.Collections.Generic;
using Extensions.GameObjects;
using UnityEngine;

namespace Game.Interactables
{
    [DisallowMultipleComponent]
    public class Interactor : NetworkBehaviourExt
    {
        private List<IInteractable> _interactables = new List<IInteractable>();
        
        public IInteractable Target { get; private set; }
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
            UpdateTarget();
        }

        private void UpdateTarget()
        {
            var forward = transform.forward;
            var position = transform.position;
            var closest = float.MaxValue;
            IInteractable target = null;
            foreach (var interactable in _interactables)
            {
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