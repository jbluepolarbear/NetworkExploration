using Input;
using UnityEngine;

namespace Game.Player
{
    public interface IMovable
    {
        bool Active { get; }
        Vector3 Position { get; set; }
        Quaternion Rotation { get; set; }
        Vector3 Velocity { get; set; }
        Vector3 AngularVelocity { get; set; }
        void ProcessInput(InputState inputState);
    }
}