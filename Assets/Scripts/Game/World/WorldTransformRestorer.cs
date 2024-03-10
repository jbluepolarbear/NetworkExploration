using System;
using System.Collections;
using Contexts;
using Extensions.GameObjects;
using Unity.Netcode;
using UnityEngine;
using UserState;
using Utilities;

namespace Game.World
{
    [Serializable]
    public class UserStateWorldTransform : IUserStateEntry
    {
        public int Id { get; set; }
        public ulong OwnerId { get; set; }
        public bool Changed { get; private set; }
        public void ClearChanged()
        {
            Changed = false;
        }

        private SerializableVector3 _position;

        public Vector3 Position
        {
            get
            {
                return _position;
            }
            set
            {
                if (value != _position)
                {
                    _position = value;
                    Changed = true;
                }
            }
        }
        private SerializableQuaternion _rotation;
        public Quaternion Rotation 
        {
            get
            {
                return _rotation;
            }
            set
            {
                if (value != _rotation)
                {
                    _rotation = value;
                    Changed = true;
                }
            }
        }
        private bool _alive = true;
        public bool Alive 
        {
            get
            {
                return _alive;
            }
            set
            {
                if (value != _alive)
                {
                    _alive = value;
                    Changed = true;
                }
            }
        }
        
        private bool _first = true;
        public bool First
        {
            get
            {
                return _first;
            }
            set
            {
                if (value != _first)
                {
                    _first = value;
                    Changed = true;
                }
            }
        }
    }
    
    public class WorldTransformRestorer : NetworkBehaviourExt
    {
        private UserStateWorldTransform _worldTransform;
        protected override IEnumerator StartServer()
        {
            while (!ServerContext.Has<WorldUserStateProvider>() || !ServerContext.Get<WorldUserStateProvider>().Ready)
            {
                yield return null;
            }
            
            _worldTransform = GetWorldTransform();
            if (!_worldTransform.Alive)
            {
                NetworkObject.Despawn();
            }
            if (_worldTransform.First)
            {
                _worldTransform.First = false;
                _worldTransform.Position = transform.position;
                _worldTransform.Rotation = transform.rotation;
            }
            else
            {
                transform.position = _worldTransform.Position;
                transform.rotation = _worldTransform.Rotation;
            }
        }

        private void LateUpdate()
        {
            if (_worldTransform == null)
            {
                return;
            }
            
            _worldTransform.Position = transform.position;
            _worldTransform.Rotation = transform.rotation;
        }

        protected override IEnumerator StartClient()
        {
            yield break;
        }
        
        protected virtual UserStateStorage GetUserStateStorage()
        {
            if (ServerContext.Get<WorldUserStateProvider>()?.Ready == true)
            {
                return ServerContext.Get<WorldUserStateProvider>().WorldUserState;
            }
            return null;
        }
        
        protected virtual UserStateWorldTransform GetWorldTransform()
        {
            var userState = GetUserStateStorage();
            var worldIdentifier = GetComponent<WorldIdentifier>();
            return userState?.GetOrMakeUserStateEntryForOwnerId<UserStateWorldTransform>(worldIdentifier.WorldId);
        }
        
        public void SetAlive(bool alive)
        {
            if (_worldTransform != null)
            {
                _worldTransform.Alive = alive;
            }
        }
        
        public static void SetAlive(uint worldId, bool alive)
        {
            var userState = ServerContext.Get<WorldUserStateProvider>().WorldUserState;
            var worldTransform = userState.GetOrMakeUserStateEntryForOwnerId<UserStateWorldTransform>(worldId);
            worldTransform.Alive = alive;
        }
    }
}