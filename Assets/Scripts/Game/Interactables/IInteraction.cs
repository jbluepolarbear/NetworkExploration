using Game.GameMode;
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
        GameModes RequiredGameModes { get; }
        
        Promise<IInteractionResult> ExecuteClient();
    }
}