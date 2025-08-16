using Creature.Characters;
using GameSystem;
using Spine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Creature.Action
{
    public class Move : Act<Move.Param>, InputManager.IAxisListener
    {
        public new class Param : Act<Move.Param>.Param
        {
            public Vector3 TargetPos = Vector3.zero;
        }
        
        private string _animationKey = string.Empty;
        
        public override void Initialize(IActor iActor)
        {
            base.Initialize(iActor);
            
            Manager.Get<IInput>()?.AddListener(this);
        }

        public override void Execute()
        {
            base.Execute();
            
            if (_param == null)
                return;

            var navMeshAgent = _iActor?.NavMeshAgent;
            if (navMeshAgent != null)
                navMeshAgent.speed = _iActor.IStat.Get(Stat.EType.MoveSpeed);

            if (_iActor is NonPlayable)
            {
                SetAnimation(_param.AnimationKey, true);
                navMeshAgent?.SetDestination(_param.TargetPos);
            }
        }

        protected override void ChainUpdate()
        {
            base.ChainUpdate();
            
            UpdateNonPlayableMove();
        }

        public override void End()
        {
            base.End();

            _animationKey = string.Empty;
        }

        private void SetPlayableAnimation(float horizontal, float vertical)
        {
            if (horizontal == 0)
            {
                var directionKey = vertical < 0 ? "_F" : "_B";
                if (!_animationKey.Contains(directionKey) ||
                    string.IsNullOrEmpty(_animationKey))
                {
                    var animationKey = $"{_param.AnimationKey}{directionKey}";
                    SetAnimation(animationKey, true);

                    _animationKey = animationKey;
                }
            }
            else
            {
                if (_param.AnimationKey != _animationKey)
                {
                    SetAnimation(_param.AnimationKey, true);
                    _animationKey = _param.AnimationKey;
                }
            }
        }

        private void UpdatePlayableMove(float horizontal, float vertical)
        {
            if (_iActor is not Playable)
                return;

            if (_iActor.IActCtr.IsInteraction)
                return;
            
            var navMeshAgent = _iActor?.NavMeshAgent;
            if (navMeshAgent == null)
                return;
            
            var iActorTm = _iActor.Transform;
            if (!iActorTm)
                return;

            if (horizontal == 0 &&
                vertical == 0)
            {
                End();
                return;
            }

            SetPlayableAnimation(horizontal, vertical);
            
            _iActor?.Flip(horizontal);

            var targetPos = iActorTm.position + new Vector3(horizontal, vertical, 0);
            targetPos.z = targetPos.y;
            iActorTm.position = Vector3.Lerp(iActorTm.position, targetPos, Time.deltaTime * navMeshAgent.speed);
            
            _iActor?.SortingOrder(iActorTm.position.y);
        }
        
        private void UpdateNonPlayableMove()
        {
            if (_iActor is not NonPlayable)
                return;
            
            var navMeshAgent = _iActor?.NavMeshAgent;
            if (navMeshAgent == null)
                return;
            
            var iActorTm = _iActor.Transform;
            if (!iActorTm)
                return;
            
            Vector2 direction = _param.TargetPos - iActorTm.position;
            _iActor?.Flip(direction.x);
            _iActor?.SortingOrder(iActorTm.position.y);

            if (navMeshAgent.isOnNavMesh)
            {
                var remainDistance = navMeshAgent.remainingDistance;
                if (remainDistance <= navMeshAgent.stoppingDistance)
                    End();
            }
        }
        
        #region InputManager.IAxisListener
        void InputManager.IAxisListener.OnAxisChanged(float horizontal, float vertical)
        {
            UpdatePlayableMove(horizontal, vertical);
        }
        #endregion
    }
}
