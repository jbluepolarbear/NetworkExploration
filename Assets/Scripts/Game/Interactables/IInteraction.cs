using Utilities;

namespace Game.Interactables
{
    public interface IInteraction
    {
        Promise<IInteractionResult> ExecuteClient();
    }
}