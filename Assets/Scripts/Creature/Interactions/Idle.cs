using UnityEngine;

namespace Creature.Interactions
{
    public class Idle : BaseInteraction<Idle.Data>
    {
        public new class Data : BaseInteraction<Idle.Data>.Data
        {

        }
        
        protected override string AnimationName
        {
            get { return "still"; }
        }
        
        public override void Execute()
        {
            base.Execute();
            
            _iInteractable?.SkeletonAnimation?.SetAnimation(AnimationName, true);
        }
    }
}