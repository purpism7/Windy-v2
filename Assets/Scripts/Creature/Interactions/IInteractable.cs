using UnityEngine;

using Spine.Unity;

namespace Creature.Interactions
{
    public interface IInteractable : ISubject
    {
        SkeletonAnimation SkeletonAnimation { get; }
        IInteractionController IInteractionCtr { get; }
    }
}

