using System;
using Unity.Netcode;
using UserState;

namespace Game.Player.UserState
{
    [Serializable]
    public class PlayerUserState : IUserState
    {
        public DateTime CreatedDate { get; set; }
        public DateTime EditedDate { get; set; }
        public string LastDevice { get; set; }
        public string Version { get; set; }
    }
}