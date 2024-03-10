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
                _lastSaveTime = Time.time;
                IsReady();
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
        private float _saveInterval = 60.0f;
        protected override void NetworkFixedUpdate()
        {
            base.NetworkFixedUpdate();
            var changed = _worldUserState.Changed;
            if (IsServer && (changed || Time.time - _lastSaveTime >= _saveInterval))
            {
                _lastSaveTime = Time.time;
                _worldUserState.ClearChanged();
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