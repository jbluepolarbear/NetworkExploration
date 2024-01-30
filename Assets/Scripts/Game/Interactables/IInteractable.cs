using System.Collections.Generic;
using Utilities;

namespace Game.Interactables
{
    public interface IInteractable
    {
        bool Interactable { get; }
        IReadOnlyList<IInteraction> Interactions { get; }
        Promise<IInteractionResult> RunInteraction(IInteraction interaction);
    }
}