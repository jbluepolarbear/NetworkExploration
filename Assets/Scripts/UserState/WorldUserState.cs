using System;
using Unity.Netcode;

namespace UserState
{
    [Serializable]
    public class WorldUserState : IUserState
    {
        public DateTime CreatedDate { get; set; }
        public DateTime EditedDate { get; set; }
        public string LastDevice { get; set; }
        public string Version { get; set; }
    }
}