using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Creature.Interactions
{
    public class Sway : BaseInteraction<Sway.Data>
    {
        public class Data : BaseInteraction<Sway.Data>.Data
        {
            
        }
        
        protected override string AnimationName
        {
            get { return "wind"; }
        }
        
        public override void Execute()
        {
            base.Execute();
            
            _iInteractable?.SkeletonAnimation?.SetAnimation(AnimationName, true);
        }
    }
}