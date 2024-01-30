using Unity.Netcode.Components;

namespace Extensions
{
    public class NetworkTransformExt : NetworkTransform
    {
        protected override bool OnIsServerAuthoritative()
        {
            return false;
        }
    }
}