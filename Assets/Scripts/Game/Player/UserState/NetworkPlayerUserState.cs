using System;
using System.Collections;
using Contexts;
using Extensions.GameObjects;
using Game.Inventory;
using UnityEngine;
using UserState;

namespace Game.Player.UserState
{
    public class NetworkPlayerUserState : NetworkBehaviourExt
    {
        private UserStateStorage _playerUserState;
        private UserStateInventory _inventory;
        
        protected override IEnumerator StartServer()
        {
            while (!ServerContext.Has<UserStateProvider>() || !ServerContext.Get<UserStateProvider>().Ready)
            {
                yield return null;
            }
            
            if (!ServerContext.Get<UserStateProvider>().HasUserStateEntry(new UserStateId
            {
                Id = SystemInfo.deviceName
            }, UserStateType.Player))
            {
                _playerUserState = new UserStateStorage
                {
                    Version = Application.version
                };
                yield return ServerContext.Get<UserStateProvider>().SetUserState(new UserStateId
                {
                    Id = SystemInfo.deviceName
                }, _playerUserState, UserStateType.Player);
                yield break;
            }
            
            var promise = ServerContext.Get<UserStateProvider>().GetUserState<UserStateStorage>(new UserStateId
            {
                Id = SystemInfo.deviceName
            }, UserStateType.Player);
            yield return promise;
            _playerUserState = promise.GetValue();
            _inventory = _playerUserState.GetOrMakeUserStateEntryForOwnerId<UserStateInventory>(0);
            _lastSaveTime = Time.time;
        }

        protected override IEnumerator StartClient()
        {
            yield break;
        }

        private float _lastSaveTime;
        private float _saveInterval = 5.0f;
        protected override void NetworkFixedUpdate()
        {
            base.NetworkFixedUpdate();
            if (IsServer && Time.time - _lastSaveTime >= _saveInterval)
            {
                _lastSaveTime = Time.time;
                ServerContext.Get<UserStateProvider>().SetUserState(new UserStateId
                {
                    Id = SystemInfo.deviceName
                }, _playerUserState, UserStateType.Player);
            }
        }
    }
}