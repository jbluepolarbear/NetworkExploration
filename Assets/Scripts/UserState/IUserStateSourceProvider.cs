using System;
using Unity.Netcode;
using Utilities;

namespace UserState
{
    public struct UserStateId
    {
        public string Id;
    }
    
    public enum UserStateType
    {
        Player,
        World
    }
    
    /// <summary>
    /// Place holder data for user state
    /// </summary>
    public interface IUserState
    {
        DateTime CreatedDate { get; }
        DateTime EditedDate { get; }
        string LastDevice { get; }
        string Version { get; }
    }
    
    public interface IUserStateSourceProvider
    {
        Promise SetUserState<T>(UserStateId userStateId, T userState, UserStateType userStateType) where T : IUserState;
        bool HasUserStateEntry(UserStateId userStateId, UserStateType userStateType);
        Promise<T> GetUserState<T>(UserStateId userStateId, UserStateType userStateType) where T : IUserState, new ();
    }
}