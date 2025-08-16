using Spine;
using UnityEngine;

using Creature.Interactions;
using Common;
using GameSystem;

namespace Creature.Action
{
    public class CutDown : Act<CutDown.Param>
    {
        public class Param : Act<CutDown.Param>.Param
        {
            public IObject IObject { get; private set; } = null;

            public Param SetIObject(IObject iObject)
            {
                IObject = iObject;
                return this;
            }
        }

        public override void Execute()
        {
            base.Execute();
            
            var skeletonAnimation = _iActor?.SkeletonAnimation;
            if (skeletonAnimation == null)
                return;

            SetSkin();
            
            var eDirection = Direction;
            
            skeletonAnimation.state?.ClearTrack(0);
            SetAnimation(GetAnimationName(eDirection), false,
                (trackEntry) =>
                {
                    End();
                });

            var eItemInteraction = EItemInteraction.None;
            if (_param?.IObject != null)
                eItemInteraction = _param.IObject.EItemInteraction;
            
            if (_param?.IObject != null &&
                (_param.IObject.EItemInteraction == EItemInteraction.Hammer ||
                _param.IObject.EItemInteraction == EItemInteraction.Pickaxe))
                eDirection = EDirection.None;
            else
            {
                if (eDirection == EDirection.Up ||
                    eDirection == EDirection.Down)
                {
                    EDirection[] subset = { EDirection.Right, EDirection.Left };
                    eDirection = subset[Random.Range(0, subset.Length)];
                }
            }
            
            var chopData = new Chop.Data()
                .WithEDirection(eDirection)
                .WithEItemInteraction(eItemInteraction);

            _param?.IObject?.IInteractionCtr?.Execute<Chop, Chop.Data>(chopData);

            if(_param.IObject.EItemInteraction == EItemInteraction.Hammer)
                EffectPlayer.Get?.Play(EffectPlayer.AudioClipData.EType.SwingHammer);
            else if(_param.IObject.EItemInteraction == EItemInteraction.Pickaxe ||
                _param.IObject.EItemInteraction == EItemInteraction.Axe)
                EffectPlayer.Get?.Play(EffectPlayer.AudioClipData.EType.SwingAxe);
        }

        private void SetSkin()
        {
            var eItemInteraction = _param?.IObject.EItemInteraction;
            
            var skinName = "default";
            if (eItemInteraction != EItemInteraction.None)
                skinName = eItemInteraction.ToString().ToLower();
  
            _iActor?.SetSkin(skinName);
        }

        private string GetAnimationName(EDirection eDirection)
        {
            string animationName = "cutdown";
           
            if (eDirection == EDirection.Up)
                return $"{animationName}_B";
             
            if(eDirection == EDirection.Down)
                return $"{animationName}_F";

            return animationName;
        }
        
        private EDirection Direction
        {
            get
            {
                if (_iActor == null)
                    return EDirection.None;

                if (_param?.IObject == null)
                    return EDirection.None;
                
                Vector2 position = _iActor.Transform.position;
                Vector2 targetPosition = _param.IObject.Transform.position;

                // 방향 벡터 (나 -> 상대)
                var direction = targetPosition - position;

                // 상대의 Collider bounds
                Bounds targetBounds = _param.IObject.Collider.bounds;
                Bounds bounds = _iActor.Collider.bounds;

                // 중심 차이 + 충돌 방향 추정
                float dx = direction.x / (bounds.extents.x + targetBounds.extents.x);
                float dy = direction.y / (bounds.extents.y + targetBounds.extents.y);

                if (Mathf.Abs(dx) > Mathf.Abs(dy))
                    return dx > 0 ? EDirection.Right : EDirection.Left;
                else
                    return dy > 0 ? EDirection.Up : EDirection.Down;
            }
        }
    }
}