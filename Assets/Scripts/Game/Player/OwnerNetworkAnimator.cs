using Unity.Netcode;
using Unity.Netcode.Components;

namespace Game.Player
{
    public class OwnerNetworkAnimator : NetworkAnimator
    {
        protected override bool OnIsServerAuthoritative()
        {
            return false;
        }
    }
}