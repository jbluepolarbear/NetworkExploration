using System;
using Unity.Netcode;

namespace UserState
{
    [Serializable]
    public class PlayerUserState : IUserState
    {
        public DateTime CreatedDate { get; }
        public DateTime EditedDate { get; }
        public string LastDevice { get; }
        public string Version { get; }

        // Player info goes here
    }
}