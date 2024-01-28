using UnityEngine;

namespace Data.ScriptableObjects
{
    [CreateAssetMenu(fileName = "Default.SystemInfoScriptableObject", menuName = "Tools/Game/CreateSystemInfo", order = 0)]
    public class SystemInfoScriptableObject : ScriptableObject
    {
        public string ServerAddress = "127.0.0.1";
        public string ClientAddress = "127.0.0.1";
        public ushort Port = 9935;
        public ushort PlayerCount = 8;
        public int SessionId;
        public int PassKey = 1234;

        public string GameScene;
        public string StartUpScene;
    }
}