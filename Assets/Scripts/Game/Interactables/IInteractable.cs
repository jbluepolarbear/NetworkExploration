using Game.GameMode;
using UnityEngine;
using Utilities;

namespace Game.Interactables
{
    public interface IInteractable
    {
        Vector3 Position { get; }
        bool Interactable { get; }
        bool AllowedToInteract(InteractionType type, GameModes gameMode);
        bool HasInteraction(InteractionType type);
        Promise<IInteractionResult> RunInteraction(InteractionType type);
    }
}