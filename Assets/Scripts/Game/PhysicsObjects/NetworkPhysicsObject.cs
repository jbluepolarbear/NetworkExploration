using Extensions;
using Unity.Netcode;
using UnityEngine;

namespace Game.PhysicsObjects
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(NetworkTransformExt))]
    public class NetworkPhysicsObject : NetworkBehaviour
    {
        private ulong _ownerClientId;
        private float _lastOwnerChangeTime;
        
        [ServerRpc(RequireOwnership = false)]
        private void ChangeOwnerServerRpc(ulong networkObjectId)
        {
            ChangeOwner(networkObjectId);
        }
        
        private void ChangeOwner(ulong networkObjectId)
        {
            NetworkObject.ChangeOwnership(networkObjectId);
            ClearBuffer();
            ClearBufferClientRpc();
        }
        
        [ClientRpc]
        private void ClearBufferClientRpc()
        {
            ClearBuffer();
        }
        
        private void ClearBuffer()
        {
            GetComponent<NetworkTransformExt>();
        }
        
        public void OnCollisionEnter(Collision other)
        {
            if (!IsClient || IsOwner)
            {
                return;
            }
            
            var networkObject = other.gameObject.GetComponent<NetworkObject>();
            var rigidBody = other.rigidbody;
            if (rigidBody == null || networkObject == null || networkObject.OwnerClientId != NetworkManager.LocalClientId)
            {
                return;
            }
            
            Debug.Log($"GameObject {gameObject.name} collided with {other.gameObject.name} and is taking ownership of {networkObject.OwnerClientId}");
            
            ChangeOwnerServerRpc(networkObject.OwnerClientId);
        }
    }
}