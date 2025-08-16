using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Cysharp.Threading.Tasks;
using Spine;

using Creature.Characters;

namespace Creature.Action
{
    public interface IAct
    {
        void Execute();
        void End();
        UniTaskVoid ChainUpdateAsync();
        void ChainLateUpdate();
        void ChainFixedUpdate();
    }
    
    public abstract class Act<T> : IAct where T : Act<T>.Param
    {
        public interface IListener
        {
            void End();
        }
        
        public class Param
        {
            public IListener IListener = null;

            public string AnimationKey { get; private set; } = string.Empty;
            // public bool NoEnd { get; private set; } = false;

            public Param SetAnimationKey(string key)
            {
                AnimationKey = key;
                return this;
            }

            // public Data SetNoEnd(bool noEnd)
            // {
            //     NoEnd = noEnd;
            //     return this;
            // }
        }
        
        protected T _param = null;
        protected Characters.IActor _iActor = null;
        // protected float _duration = 0;

        private bool _update = false;
        private bool _end = false;
        private System.Action _endAction = null;
        
        public virtual void Initialize(IActor iActor)
        {
            _iActor = iActor;
        }
        
        public virtual void End()
        {
            if (_end)
                return;
            
            _endAction?.Invoke();
            _param?.IListener?.End();

            _update = false;
            _end = true;
            _param = null;
        }

        public void SetParam(T param)
        {
            _param = param;
        }

        public void SetEndActAction(System.Action endAction)
        {
            _endAction = endAction;
        }

        public async UniTaskVoid ChainUpdateAsync()
        {
            _update = true;
            
            await UniTask.Yield();
            await UniTask.WaitWhile(
                () =>
                {
                    ChainUpdate();

                    return _update && !_end;
                });
        }
        
        #region IAct

        public virtual void Execute()
        {
            _end = false;
        }
        
        protected virtual void ChainUpdate()
        {
            
        }

        public virtual void ChainLateUpdate()
        {
            
        }

        public virtual void ChainFixedUpdate()
        {
            
        }
        #endregion

        protected void SetAnimation(string animationName, bool loop, System.Action<TrackEntry> completeAction = null)
        {
            // _iActor?.SetAnimation(animationName, loop);
            // if (_iActor?.Animator != null)
            // {
            //     _iActor.Animator.SetTrigger(animationName);
            //     return;
            // }
            
            var skeletonAnimation = _iActor?.SkeletonAnimation;
            if (skeletonAnimation == null)
                return;
            
            skeletonAnimation.SetAnimation(animationName, loop, completeAction);
            
            // var animationState = skeletonAnimation.AnimationState;
            // if (animationState == null)
            //     return;
            //
            // var animation = skeletonAnimation.skeletonDataAsset?.GetSkeletonData(true)?.Animations?
            //     .Find(animation => animation.Name.Contains(animationName));
            // if (animation == null)
            //     return;
            //
            // var trackEntry = animationState.SetAnimation(0, animationName, loop);
            // if (trackEntry == null)
            //     return;
            //
            // trackEntry.Complete += trackEntry => { completeAction?.Invoke(trackEntry); };
            // trackEntry.Complete += complete;
            // _duration = trackEntry.Animation.Duration;
        }
    }
}

