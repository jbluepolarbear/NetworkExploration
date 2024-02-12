using System;
using System.Collections.Generic;
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
        string Version { get; }
        public IReadOnlyList<IUserStateEntry> UserStateEntries { get; }
        public int AddUserStateEntry(IUserStateEntry userStateEntry);
        public int RemoveUserStateEntry(IUserStateEntry userStateEntry);
        public IUserStateEntry GetUserStateEntry(int id);
        public T GetUserStateEntry<T>(int id) where T : IUserStateEntry;
        public IReadOnlyList<IUserStateEntry> GetUserStateEntries<T>() where T : IUserStateEntry;
        public int RemoveUserStateEntry(int id);
        public T GetOrMakeSingleUserStateEntry<T>() where T : IUserStateEntry, new();
        public T MakeUserStateEntry<T>() where T : IUserStateEntry, new();
        /// <summary>
        /// Owner's are only allowed to have one entry of a type, but the owner can have multiple types of entries
        /// </summary>
        /// <param name="ownerId"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetOrMakeUserStateEntryForOwnerId<T>(ulong ownerId) where T : IUserStateEntry, new();
    }
    
    public interface IUserStateSourceProvider
    {
        Promise SetUserState<T>(UserStateId userStateId, T userState, UserStateType userStateType) where T : IUserState;
        bool HasUserStateEntry(UserStateId userStateId, UserStateType userStateType);
        Promise<T> GetUserState<T>(UserStateId userStateId, UserStateType userStateType) where T : IUserState, new ();
    }
}