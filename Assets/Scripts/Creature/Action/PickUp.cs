using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

using Spine;
using GameSystem;

namespace Creature.Action
{
    public class PickUp : Act<PickUp.Param>
    {
        public class Param : Act<PickUp.Param>.Param
        {
            public IObject IObject = null;
        }

        public override void Execute()
        {
            base.Execute();
            
            var skeletonAnimation = _iActor?.SkeletonAnimation;
            if (skeletonAnimation == null)
                return;
            
            _iActor?.SetSkin("default");
            
            skeletonAnimation.state?.ClearTrack(0);
            SetAnimation("pickup", false,
                (trackEntry) =>
                {
                    End();
                });

            EffectPlayer.Get?.Play(EffectPlayer.AudioClipData.EType.PickItem);

            DisappearObjectAsync().Forget();
        }

        private async UniTask DisappearObjectAsync()
        {
            var skeletonAnimation = _iActor?.SkeletonAnimation;
            if (skeletonAnimation == null)
                return;
            
            TrackEntry trackEntry = skeletonAnimation.AnimationState?.GetCurrent(0);
            if (trackEntry == null)
                return;

            var halfDuration = trackEntry.Animation.Duration * 0.5f - 0.02f;
            await UniTask.Delay(TimeSpan.FromSeconds(halfDuration));
            
            _param?.IObject.Deactivate(true);
        }

        protected override void ChainUpdate()
        {
            base.ChainUpdate();
            
            
        }

        public override void End()
        {
            
            
            base.End();
        }
    }
}