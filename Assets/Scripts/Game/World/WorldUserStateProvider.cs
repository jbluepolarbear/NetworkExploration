using System;
using System.Collections;
using Contexts;
using UnityEngine;
using UserState;

namespace Game.World
{
    public class WorldUserStateProvider : ContextProvider<WorldUserStateProvider>, IServerContextProvider, IClientContextProvider
    {
        private UserStateStorage _worldUserState;
        
        public UserStateStorage WorldUserState => _worldUserState;
        
        protected override IEnumerator StartServer()
        {
            while (!ServerContext.Has<UserStateProvider>() || !ServerContext.Get<UserStateProvider>().Ready)
            {
                yield return null;
            }
            
            if (!ServerContext.Get<UserStateProvider>().HasUserStateEntry(new UserStateId
                {
                    Id = SystemInfo.deviceName
                }, UserStateType.World))
            {
                _worldUserState = new UserStateStorage
                {
                    Version = Application.version
                };
                yield return ServerContext.Get<UserStateProvider>().SetUserState(new UserStateId
                {
                    Id = SystemInfo.deviceName
                }, _worldUserState, UserStateType.World);
                yield break;
            }
            
            var promise = ServerContext.Get<UserStateProvider>().GetUserState<UserStateStorage>(new UserStateId
            {
                Id = SystemInfo.deviceName
            }, UserStateType.World);
            yield return promise;
            _worldUserState = promise.GetValue();
            _lastSaveTime = Time.time;
            IsReady();
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
                }, _worldUserState, UserStateType.World);
            }
        }

        protected override IEnumerator StartClient()
        {
            yield break;
        }
    }
}