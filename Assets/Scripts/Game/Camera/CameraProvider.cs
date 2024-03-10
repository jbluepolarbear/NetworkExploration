using Cinemachine;
using UnityEngine;
using Utilities;

namespace Game.Camera
{
    public class CameraProvider : UnitySingleton<CameraProvider>
    {
        public CinemachineBrain Brain;
        public ICinemachineCamera ActiveCamera => Brain.ActiveVirtualCamera as ICinemachineCamera;
        public Quaternion CameraRotation => Brain.transform.rotation;
    }
}