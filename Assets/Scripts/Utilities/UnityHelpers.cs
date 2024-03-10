using System;
using UnityEditor;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace Utilities
{
    public static class UnityHelpers
    {
        public static bool IsUnityNull(this object obj)
        {
            // Checks whether an object is null or Unity pseudo-null
            // without having to cast to UnityEngine.Object manually

            return obj == null || ((obj is UnityObject) && ((UnityObject)obj) == null);
        }
        
        private static RaycastHit[] _raycastHits = new RaycastHit[10];
        // a method that casts a ray from tire to ground, return ray that is not self collider
        public static bool ClosestRaycast(Vector3 origin, Vector3 direction, out RaycastHit raycastHit, float maxDistance, int layer, QueryTriggerInteraction queryTriggerInteraction, Rigidbody excludeRigidbody = null)
        {
            var hits = Physics.RaycastNonAlloc(origin, direction, _raycastHits, maxDistance, layer, queryTriggerInteraction);
            var raycastHitIndex = -1;
            var closestDistance = maxDistance;
            for (var i = 0; i < hits; ++i)
            {
                if (_raycastHits[i].rigidbody != excludeRigidbody && _raycastHits[i].distance < closestDistance)
                {
                    raycastHitIndex = i;
                    closestDistance = _raycastHits[i].distance;
                }
            }
            
            if (raycastHitIndex != -1)
            {
                raycastHit = _raycastHits[raycastHitIndex];
                return true;
            }
            
            raycastHit = default;
            return false;
        }
        
        
        public static bool IsTransformInFrontOfTarget(Transform camera, Transform blockTransform, Transform target, float radius, int layer, QueryTriggerInteraction queryTriggerInteraction)
        {
            var direction = target.position - camera.position;
            var distance = direction.magnitude - radius;
            direction.Normalize();
            var num = Physics.SphereCastNonAlloc(camera.position, radius, direction, _raycastHits, distance, layer, queryTriggerInteraction);
            InsertionSort(_raycastHits, 0, num, (a, b) => a.distance < b.distance);
            for (var i = 0; i < num; i++)
            {
                if (_raycastHits[i].transform == target)
                {
                    return false;
                }
                
                if (_raycastHits[i].transform == blockTransform)
                {
                    return true;
                }
            }

            return false;
        }
        
        public static void InsertionSort<T>(T[] array, int index, int num, Func<T, T, bool> comparer)
        {
            for (var i = 1 + index; i < num + index; ++i) { 
                var key = array[i]; 
                var j = i - 1; 
  
                // Move elements of arr[0..i-1], 
                // that are greater than key, 
                // to one position ahead of 
                // their current position 
                while (j >= 0 && !comparer(array[j], key)) { 
                    array[j + 1] = array[j]; 
                    j = j - 1; 
                } 
                array[j + 1] = key; 
            } 
        }
    }
}