using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Creature.Characters;

namespace Creature
{
    public interface ISubject
    {
        int Id { get; }
        Transform Transform { get; }
        Collider2D Collider { get; }
        
        void Activate();
        void Deactivate(bool deactivateTm = false);
        bool IsActivate { get; }

        void SortingOrder(float order);
        int Order { get; }
    }
}

