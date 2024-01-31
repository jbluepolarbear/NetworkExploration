using System.Collections.Generic;
using UnityEngine;
using Utilities;

namespace Game.Interactables
{
    public interface IInteractable
    {
        Vector3 Position { get; }
        bool Interactable { get; }
        bool HasInteraction(InteractionType type);
        Promise<IInteractionResult> RunInteraction(InteractionType type);
    }
}