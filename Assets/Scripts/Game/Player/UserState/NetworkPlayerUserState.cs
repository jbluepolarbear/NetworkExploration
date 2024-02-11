using System;
using System.Collections;
using Contexts;
using Extensions.GameObjects;
using UnityEngine;
using UserState;

namespace Game.Player.UserState
{
    public class NetworkPlayerUserState : NetworkBehaviourExt
    {
        private PlayerUserState _playerUserState;
        
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
                _playerUserState = new PlayerUserState
                {
                    CreatedDate = DateTime.Now,
                    EditedDate = DateTime.Now,
                    LastDevice = SystemInfo.deviceName,
                    Version = Application.version
                };
                yield return ServerContext.Get<UserStateProvider>().SetUserState(new UserStateId
                {
                    Id = SystemInfo.deviceName
                }, _playerUserState, UserStateType.Player);
                yield break;
            }
            
            var promise = ServerContext.Get<UserStateProvider>().GetUserState<PlayerUserState>(new UserStateId
            {
                Id = SystemInfo.deviceName
            }, UserStateType.Player);
            yield return promise;
            _playerUserState = promise.GetValue();
        }

        protected override IEnumerator StartClient()
        {
            yield break;
        }
    }
}