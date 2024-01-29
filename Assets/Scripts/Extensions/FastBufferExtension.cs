using Unity.Netcode;
using UnityEngine;

namespace Extensions
{
    public static class FastBufferExtension
    {
        public static Vector3 ReadVector3(this FastBufferReader reader)
        {
            reader.ReadValueSafe(out Vector3 value);
            return value;
        }
        
        public static Quaternion ReadQuaternion(this FastBufferReader reader)
        {
            reader.ReadValueSafe(out Quaternion value);
            return value;
        }

        public static uint ReadUInt(this FastBufferReader reader)
        {
            reader.ReadValueSafe(out uint value);
            return value;
        }

        public static int ReadInt(this FastBufferReader reader)
        {
            reader.ReadValueSafe(out int value);
            return value;
        }
    }
}