using System;
using UnityEngine;

namespace Utilities
{
    [Serializable]
    public struct SerializableVector3
    {
        public float x; 
        public float y;
        public float z;
        
        public static implicit operator Vector3(SerializableVector3 a) => new (a.x, a.y, a.z);
        public static implicit operator SerializableVector3(Vector3 a) => new () { x = a.x, y = a.y, z = a.z };
    }
    
    [Serializable]
    public struct SerializableQuaternion
    {
        public float x;
        public float y;
        public float z;
        public float w;
        
        public static implicit operator Quaternion(SerializableQuaternion a) => new (a.x, a.y, a.z, a.w);
        public static implicit operator SerializableQuaternion(Quaternion a) => new () { x = a.x, y = a.y, z = a.z, w = a.w };
    }
}