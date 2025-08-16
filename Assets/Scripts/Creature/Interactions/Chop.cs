using Cysharp.Threading.Tasks;
using UnityEngine;

using Common;

namespace Creature.Interactions
{
    public class Chop : BaseInteraction<Chop.Data>
    {
        public new class Data : BaseInteraction<Chop.Data>.Data
        {
            public EDirection EDirection { get; private set; } = EDirection.None;
            public EItemInteraction EItemInteraction { get; private set; } = EItemInteraction.None;

            public Data WithEDirection(EDirection eDirection)
            {
                EDirection = eDirection;
                return this;
            }
            
            public Data WithEItemInteraction(EItemInteraction eItemInteraction)
            {
                EItemInteraction = eItemInteraction;
                return this;
            }
        }
        
        private int _count = 0;
        
        public bool IsFinished { get; private set; } = false;

        public EItemInteraction EItemInteraction { get; private set; } = EItemInteraction.None;

        protected override string AnimationName
        {
            get { return "cutdown"; }
        }
        
        public override void Execute()
        {
            base.Execute();

            if (IsFinished)
                return;
            
            var skeletonAnimation = _iInteractable?.SkeletonAnimation;
            if (skeletonAnimation == null)
                return;

            var current = skeletonAnimation.AnimationState.GetCurrent(0);
            if (current != null &&
                current.Animation != null)
            {
                if(current.Animation.Name.Contains(AnimationName))
                    return;
            }
            
            if (_iListener == null)
                _iListener = _iInteractable?.IInteractionCtr as InteractionController;

            skeletonAnimation.state?.ClearTrack(0);
            skeletonAnimation.SetAnimation(ResAnimationName, false, 
                trackEntry =>
                {
                    _count += 1;
            
                    if (_count >= 5)
                        IsFinished = true;
                    
                    End();
                });
        }

        public override void End()
        {
            if(_data != null)
                EItemInteraction = _data.EItemInteraction;
            
            base.End();
        }

        public override void Clear()
        {
            base.Clear();
            
            _count = 0;
            IsFinished = false;
        }

        private string ResAnimationName
        {
            get
            {
                var directionKey = string.Empty;
                if (_data != null)
                {
                    if (_data.EDirection == EDirection.Right)
                        directionKey = "_L";
                    else if (_data.EDirection == EDirection.Left)
                        directionKey = "_R";
                }

                return $"{AnimationName}{directionKey}";
            }
        }
    }
}

