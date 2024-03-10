using System;
using Unity.Netcode;
using UnityEngine;
using Utilities;

namespace Game.World
{
    public class WorldIdentifier : NetworkBehaviour
    {
        [SerializeField] private NetworkVariable<uint> _worldId = new();

        public uint WorldId
        {
            get => _worldId.Value;
            set
            {
                _worldId.Value = value;
            }
        }
        
        #if UNITY_EDITOR
        public void OnValidate()
        {
            var prop = typeof(NetworkObject).GetField("GlobalObjectIdHash", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var value = (uint) prop.GetValue(GetComponent<NetworkObject>());
            _worldId.Value = value;
        }
        #endif
    }
}