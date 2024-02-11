using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using Utilities;

namespace UserState.SourceProviders
{
    public class PlayerPrefsUserStateSourceProvider : IUserStateSourceProvider
    {
        public bool Locked { get; private set; }

        public Promise SetUserState<T>(UserStateId userStateId, T userState, UserStateType userStateType) where T : IUserState
        {
            var promise = new Promise();
            if (Locked)
            {
                promise.FillError(new PromiseError{Code = PromiseErrorCodes.NotAllowed, Reason = "User state is locked"});
            }
            else
            {
                Locked = true;
                SetUserState(promise, userStateId, userState, userStateType);
            }
            return promise;
        }

        public Promise<T> GetUserState<T>(UserStateId userStateId, UserStateType userStateType) where T : IUserState, new ()
        {
            var promise = new Promise<T>();
            if (Locked)
            {
                promise.FillError(new PromiseError{Code = PromiseErrorCodes.NotAllowed, Reason = "User state is locked"});
            }
            else
            {
                Locked = true;
                GetUserState(promise, userStateId, userStateType);
            }
            return promise;
        }

        private async void SetUserState(Promise promise, UserStateId userStateId, IUserState userState, UserStateType userStateType)
        {
            var key = GetKey(userStateId, userStateType);
            var stream = new MemoryStream();
            var formatter = new BinaryFormatter();
            formatter.Serialize(stream, userState);
            string base64 = Convert.ToBase64String(stream.GetBuffer(), 0, (int) stream.Position);
            
            PlayerPrefs.SetString(key, base64);
            PlayerPrefs.Save();
            
            Locked = false;
            promise.Fulfill();
        }

        public bool HasUserStateEntry(UserStateId userStateId, UserStateType userStateType)
        {
            var key = GetKey(userStateId, userStateType);
            return PlayerPrefs.HasKey(key);
        }
        
        private async void GetUserState<T>(Promise<T> promise, UserStateId userStateId, UserStateType userStateType) where T : IUserState, new ()
        {
            var key = GetKey(userStateId, userStateType);
            if (!PlayerPrefs.HasKey(key))
            {
                promise.FillError(new PromiseError(){Code = PromiseErrorCodes.NotFound, Reason = "User state not found"});
                return;
            }
            
            var base64 = PlayerPrefs.GetString(key);
            var bytes = Convert.FromBase64String(base64);
            var stream = new MemoryStream(bytes);
            var formatter = new BinaryFormatter();
            var userState = (T) formatter.Deserialize(stream);
            
            Locked = false;
            promise.Fulfill(userState);
        }
        
        private string GetKey(UserStateId userStateId, UserStateType userStateType)
        {
            return $"{userStateType}_{userStateId.Id}";
        }
    }
}