using UnityEngine;

namespace Utilities
{
    public class UnitySingleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T _instance;
        public static T Instance => _instance;
        protected virtual bool DoneDestroyOnLoad => false;

        protected void ResetInstance()
        {
            Destroy(this);
            _instance = null;
        }

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