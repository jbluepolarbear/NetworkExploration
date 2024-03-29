﻿using System.Collections;
using Contexts;
using Extensions.GameObjects;
using UnityEngine;
using UserState;

namespace Game.Player.UserState
{
    public class NetworkPlayerUserState : NetworkBehaviourExt
    {
        public UserStateStorage PlayerUserState { get; private set; }
        
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
                PlayerUserState = new UserStateStorage
                {
                    Version = Application.version
                };
                yield return ServerContext.Get<UserStateProvider>().SetUserState(new UserStateId
                {
                    Id = SystemInfo.deviceName
                }, PlayerUserState, UserStateType.Player);
                yield break;
            }
            
            var promise = ServerContext.Get<UserStateProvider>().GetUserState<UserStateStorage>(new UserStateId
            {
                Id = SystemInfo.deviceName
            }, UserStateType.Player);
            yield return promise;
            PlayerUserState = promise.GetValue();
            _lastSaveTime = Time.time;
        }

        protected override IEnumerator StartClient()
        {
            yield break;
        }

        private float _lastSaveTime;
        private float _saveInterval = 60.0f;
        protected override void NetworkFixedUpdate()
        {
            base.NetworkFixedUpdate();
            var changed = PlayerUserState.Changed;
            if (IsServer && (changed || Time.time - _lastSaveTime >= _saveInterval))
            {
                _lastSaveTime = Time.time;
                PlayerUserState.ClearChanged();
                ServerContext.Get<UserStateProvider>().SetUserState(new UserStateId
                {
                    Id = SystemInfo.deviceName
                }, PlayerUserState, UserStateType.Player);
            }
        }
    }
}