using UnityEngine;

namespace NetLib.NetMath
{
    public static class NetMath
    {
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
        
        /*
         export function hermiteLerpVec3(
              A: any,
              B: any,
              C: any,
              D: any,
              t: number
            ): any {
              return new pc.Vec3(
                cubicHermite(A.x, B.x, C.x, D.x, t),
                cubicHermite(A.y, B.y, C.y, D.y, t),
                cubicHermite(A.z, B.z, C.z, D.z, t)
              );
            }
         */
        
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