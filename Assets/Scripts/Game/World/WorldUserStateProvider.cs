using System;
using System.Collections;
using Contexts;
using UnityEngine;
using UserState;
using PlayerUserState = Game.Player.UserState.PlayerUserState;

namespace Game.World
{
    public class WorldUserStateProvider : ContextProvider<WorldUserStateProvider>, IServerContextProvider, IClientContextProvider
    {
        private WorldUserState _worldUserState;
        
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
                _worldUserState = new WorldUserState
                {
                    CreatedDate = DateTime.Now,
                    EditedDate = DateTime.Now,
                    LastDevice = SystemInfo.deviceName,
                    Version = Application.version
                };
                yield return ServerContext.Get<UserStateProvider>().SetUserState(new UserStateId
                {
                    Id = SystemInfo.deviceName
                }, _worldUserState, UserStateType.World);
                yield break;
            }
            
            var promise = ServerContext.Get<UserStateProvider>().GetUserState<WorldUserState>(new UserStateId
            {
                Id = SystemInfo.deviceName
            }, UserStateType.World);
            yield return promise;
            _worldUserState = promise.GetValue();
            IsReady();
        }

        protected override IEnumerator StartClient()
        {
            yield break;
        }
    }
}