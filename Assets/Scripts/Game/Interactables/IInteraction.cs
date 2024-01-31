using Utilities;

namespace Game.Interactables
{
    public enum InteractionType
    {
        Action,
        Action2
    }
    
    public interface IInteraction
    {
        InteractionType Type { get; }
        Promise<IInteractionResult> ExecuteClient();
    }
}