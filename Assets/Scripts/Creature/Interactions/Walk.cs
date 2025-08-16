using UnityEngine;

namespace Creature.Interactions
{
    public class Walk : BaseInteraction<Walk.Data>
    {
        public new class Data : BaseInteraction<Walk.Data>.Data
        {

        }
        
        protected override string AnimationName
        {
            get { return "walk"; }
        }
        
        public override void Execute()
        {
            base.Execute();
            
            _iInteractable?.SkeletonAnimation?.SetAnimation(AnimationName, true);
        }
    }
}