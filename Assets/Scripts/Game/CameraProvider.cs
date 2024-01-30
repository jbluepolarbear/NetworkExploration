using Cinemachine;
using NetLib.Utility;

namespace Game
{
    public class CameraProvider : UnitySingleton<CameraProvider>
    {
        public CinemachineBrain Brain;
        public CinemachineVirtualCamera ActiveCamera => Brain.ActiveVirtualCamera as CinemachineVirtualCamera;
    }
}