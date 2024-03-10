using System.Collections.Generic;
using UnityEngine;

namespace Utilities
{
    public static class Math
    {
        public static float Lerp(float a, float b, float t)
        {
            return a + (b - a) * t;
        }
        
        public static int PowerOf2(int power)
        {
            if (_powerOf2Cache.Count <= power)
            {
                var value = PowerOf2(power - 1) * 2;
                _powerOf2Cache.Add(value);
            }

            return _powerOf2Cache[power];
        }

        public const float RadianToDegree = 57.2958f;
        public const float DegreeToRadian = 1 / RadianToDegree;

        public static (float x, float y, float z) QuaternionToEuler(float x, float y, float z, float w)
        {
            var t0 = 2.0f * (w * x + y * z);
            var t1 = 1.0f - 2.0f * (x * x + y * y);
            var roll = (float) System.Math.Atan2(t0, t1);
            var t2 = 2.0f * (w * y - z * x);
            t2 = t2 > 1.0f ? 1.0f : t2;
            t2 = t2 < -1.0f ? -1.0f : t2;
            var pitch = (float) System.Math.Asin(t2);
            var t3 = 2.0f * (w * z + x * y);
            var t4 = 1.0f - 2.0f * (y * y + z * z);
            var yaw = (float) System.Math.Atan2(t3, t4);
            return (yaw, pitch, roll);
        }

        public static (float x, float y, float z, float w) EulerToQuaternion(float yaw, float pitch, float roll)
        {
            var qx = (float) (System.Math.Sin(roll / 2.0f) * System.Math.Cos(pitch / 2.0f) * System.Math.Cos(yaw / 2.0f) - System.Math.Cos(roll / 2.0f) * System.Math.Sin(pitch / 2.0f) * System.Math.Sin(yaw / 2.0f));
            var qy = (float) (System.Math.Cos(roll / 2.0f) * System.Math.Sin(pitch / 2.0f) * System.Math.Cos(yaw / 2.0f) + System.Math.Sin(roll / 2.0f) * System.Math.Cos(pitch / 2.0f) * System.Math.Sin(yaw / 2.0f));
            var qz = (float) (System.Math.Cos(roll / 2.0f) * System.Math.Cos(pitch / 2.0f) * System.Math.Sin(yaw / 2.0f) - System.Math.Sin(roll / 2.0f) * System.Math.Sin(pitch / 2.0f) * System.Math.Cos(yaw / 2.0f));
            var qw = (float) (System.Math.Cos(roll / 2.0f) * System.Math.Cos(pitch / 2.0f) * System.Math.Cos(yaw / 2.0f) + System.Math.Sin(roll / 2.0f) * System.Math.Sin(pitch / 2.0f) * System.Math.Sin(yaw / 2.0f));
            return (qx, qy, qz, qw);
        }
        
        public static Quaternion ShortestRotation(Quaternion from, Quaternion to)
        {
            var dot = Quaternion.Dot(from, to);
            if (dot < 0.0f)
            {
                to.x = -to.x;
                to.y = -to.y;
                to.z = -to.z;
                to.w = -to.w;
            }

            return to * Quaternion.Inverse(from);
        }

        public static uint SignedToUnsigned(int value)
        {
            return (uint) ((value << 1) ^ (value >> 31));
        }

        public static int UnsignedToSigned(uint value)
        {
            return (int) ((value >> 1) ^ -(value & 1));
        }
        
        private static List<int> _powerOf2Cache = new List<int>(64)
        {
            1, 2, 4, 8, 16, 32, 64, 128, 256, 512, 1024, 2048, 4096, 8192, 16384, 32768, 65536
        };
        
        public const float Epsilon = 0.0001f;
        
        /// <summary>
        /// Algorithm: https://blog.demofox.org/2015/08/08/cubic-hermite-interpolation/
        /// License (MIT): https://blog.demofox.org/license/
        /// </summary>
        /// <returns></returns>
        public static float CubicHermite(float p0, float p1, float p2, float p3, float t)
        {
            var a = -p0 / 2.0f + (3.0f * p1) / 2.0f - (3.0f * p2) / 2.0f + p3 / 2.0f;
            var b = p0 - (5.0f * p1) / 2.0f + 2.0f * p2 - p3 / 2.0f;
            var c = -p0 / 2.0f + p2 / 2.0f;
            var d = p1;

            return a * t * t * t + b * t * t + c * t + d;
        }
        
        //Returns a position between 4 Vector3 with Catmull-Rom spline algorithm
        //http://www.iquilezles.org/www/articles/minispline/minispline.htm
        public static Vector3 CatmullRomPosition(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
        {
            //The coefficients of the cubic polynomial (except the 0.5f * which I added later for performance)
            Vector3 a = 2f * p1;
            Vector3 b = p2 - p0;
            Vector3 c = 2f * p0 - 5f * p1 + 4f * p2 - p3;
            Vector3 d = -p0 + 3f * p1 - 3f * p2 + p3;

            //The cubic polynomial: a + b * t + c * t^2 + d * t^3
            Vector3 pos = 0.5f * (a + (b * t) + (c * t * t) + (d * t * t * t));

            return pos;
        }

        public static uint SetBit(uint value, int bit)
        {
            return value | 1u << bit;
        }

        public static uint ClearBit(uint value, int bit)
        {
            return value &= ~(1u << bit);
        }

        public static uint ToggleBit(uint value, int bit)
        {
            return value ^ 1u << bit;
        }

        public static bool GetBit(uint value, int bit)
        {
            return (value & 1u << bit) > 0;
        }
    }
}