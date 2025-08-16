using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

using Spine.Unity;

using Creature.Action;

namespace Creature.Characters
{
    public interface IActor : ISubject
    {
        // Animator Animator { get; }
        NavMeshAgent NavMeshAgent { get; }
        SkeletonAnimation SkeletonAnimation { get; }
        
        IStat IStat { get; }
        IActController IActCtr { get; }
        
        string AnimationKey<T>(Act<T> act) where T : Act<T>.Param;
        float Height { get; }
        void Flip(float x);
        void SetSkin(string skinName);
        
        void SetEventHandler(System.Action<IActor> eventHandler);
        System.Action<IActor> EventHandler { get; }
    }
}
