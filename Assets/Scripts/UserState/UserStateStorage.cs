using System;
using System.Collections.Generic;
using System.Linq;
using Utilities;

namespace UserState
{
    [Serializable]
    public class UserStateStorage : IUserState
    {
        public string Version { get; set; }
        
        private int _id;
        private List<IUserStateEntry> _userStateEntries = new List<IUserStateEntry>();
        public IReadOnlyList<IUserStateEntry> UserStateEntries => _userStateEntries;
        
        public int AddUserStateEntry(IUserStateEntry userStateEntry)
        {
            userStateEntry.Id = _id++;
            _userStateEntries.Add(userStateEntry);
            return userStateEntry.Id;
        }

        public int RemoveUserStateEntry(IUserStateEntry userStateEntry)
        {
            var index = _userStateEntries.IndexOf(userStateEntry);
            if (index == -1)
            {
                return -1;
            }
            _userStateEntries.RemoveAt(index);
            return index;
        }

        public IUserStateEntry GetUserStateEntry(int id)
        {
            return _userStateEntries.Find(entry => entry.Id == id);
        }
        
        public T MakeUserStateEntry<T>() where T : IUserStateEntry, new()
        {
            var entry = new T();
            AddUserStateEntry(entry);
            return entry;
        }

        public T GetOrMakeUserStateEntryForOwnerId<T>(ulong ownerId) where T : IUserStateEntry, new()
        {
            var entry = GetUserStateEntries<T>().SingleOrDefault(e => e.OwnerId == ownerId);
            if (entry != null)
            {
                return (T) entry;
            }
            entry = MakeUserStateEntry<T>();
            entry.OwnerId = ownerId;
            
            return (T) entry;
        }

        public bool Changed => _userStateEntries.Any(entry => entry.Changed);
        public void ClearChanged()
        {
            foreach (var entry in _userStateEntries)
            {
                entry.ClearChanged();
            }
        }

        public T GetOrMakeSingleUserStateEntry<T>() where T : IUserStateEntry, new()
        {
            var entry = GetUserStateEntries<T>();
            if (entry.Count > 0)
            {
                Assert.True(entry.Count == 1);
                return (T) entry[0];
            }
            return MakeUserStateEntry<T>();
        }

        public T GetUserStateEntry<T>(int id) where T : IUserStateEntry
        {
            return (T) _userStateEntries.Find(entry => entry.Id == id);
        }

        public IReadOnlyList<IUserStateEntry> GetUserStateEntries<T>() where T : IUserStateEntry
        {
            return _userStateEntries.FindAll(entry => entry is T);
        }

        public int RemoveUserStateEntry(int id)
        {
            var index = _userStateEntries.FindIndex(entry => entry.Id == id);
            if (index == -1)
            {
                return -1;
            }
            _userStateEntries.RemoveAt(index);
            return index;
        }
    }
}