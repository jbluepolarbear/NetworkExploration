using UnityEngine;

namespace NetLib.Utility
{
    public class UnitySingleton<T> : MonoBehaviour
    {
        private static T _instance;
        public static T Instance => _instance;
        protected virtual bool DoneDestroyOnLoad => false;

        private void Awake()
        {
            _instance = GetComponent<T>();
            if (DoneDestroyOnLoad)
            {
                DontDestroyOnLoad(this);
            }
        }
    }
}