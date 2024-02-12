using System;
using UserState;

namespace Game.Inventory
{
    [Serializable]
    public class UserStateInventory : IUserStateEntry
    {
        public int Id { get; set; }
        public ulong OwnerId { get; set; }
        public Inventory Inventory { get; set; } = new Inventory();
    }
}