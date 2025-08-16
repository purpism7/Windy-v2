using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

using Cysharp.Threading.Tasks;

using Creature.Characters;


namespace Creature.Action
{
    public interface IActController : IController<IActController, IActor>
    {
        IActController MoveToTarget(Vector3? pos = null);
        // void Execute(bool noEnd = false); 
        UniTask ExecuteAsync<T, V>(V param = null) where T : Act<V>, new() where V : Act<V>.Param, new();

        IAct CurrIAct { get; }
        bool InAction { get; }
        bool IsInteraction { get; }
    }
        
    public class ActController : Controller, IActController
    {
        private IActor _iActor = null;
        private Dictionary<System.Type, IAct> _iActDic = null;
        
        private Vector3 _originPos = Vector3.zero;

        public IAct CurrIAct { get; private set; } = null;
        public bool InAction { get; private set; } = false;

        IActController IController<IActController, IActor>.Initialize(IActor iActor)
        {
            _iActor = iActor;

            _iActDic = new();
            _iActDic.Clear();
            
            return this;
        }

        #region IController

        void IController<IActController, IActor>.ChainUpdate()
        {
            if (!IsActivate)
                return;
        }

        void IController<IActController, IActor>.ChainLateUpdate()
        {
            if (!IsActivate)
                return;
            
            CurrIAct?.ChainLateUpdate();
        }
        
        void IController<IActController, IActor>.ChainFixedUpdate()
        {
            if (!IsActivate)
                return;
            
            CurrIAct?.ChainFixedUpdate();
        }

        public override void Activate()
        {
            base.Activate();
            
            if (transform.parent)
            {
                _originPos = transform.parent.position;
                _originPos.z = 0;
            }
        }

        public override void Deactivate()
        {
            base.Deactivate();
            
            Idle();
        }
        #endregion
            
        #region IActController
        IActController IActController.MoveToTarget(Vector3? pos)
        {
            if (!IsActivate)
                return null;

            var targetPos = _originPos;
            if (pos != null)
                targetPos = pos.Value;
            
            var param = new Move.Param
            {
                TargetPos = targetPos,
            };
            
            ExecuteAsync<Move, Move.Param>(param).Forget();

            return this;
        }
        
        private void Idle()
        {
            ExecuteAsync<Idle, Idle.Param>().Forget();

            InAction = false;
        }

        private Act<V> GetAct<T, V>() where T : Act<V>, new() where V : Act<V>.Param, new()
        {
            if (_iActDic == null)
            {
                _iActDic = new();
                _iActDic.Clear();
            }
            
            System.Type type = typeof(T);
            Act<V> act = null;
            
            if (_iActDic.TryGetValue(type, out IAct iAct))
                act = iAct as Act<V>;
            else
            {
                act = new T();
                act.Initialize(_iActor);
                act.SetEndActAction(EndAct);
                
                _iActDic?.TryAdd(type, act);
            }
            
            return act;
        }

        bool IActController.IsInteraction
        {
            get
            {
                return CurrIAct is CutDown || CurrIAct is PickUp || CurrIAct is Conversation;
            }
        }
        #endregion

        private V GetData<V>(Act<V> act, V param) where V : Act<V>.Param, new()
        {
            if (param == null)
                param = new V();
            
            act.SetParam(param);
            if(param.IListener == null)
                param.IListener = _iActor as Act<V>.IListener;
            
            var animationKey = _iActor?.AnimationKey(act);
            param.SetAnimationKey(animationKey);

            return param;
        }

        public async UniTask ExecuteAsync<T, V>(V param = null) where T : Act<V>, new() where V : Act<V>.Param, new()
        {
            var act = GetAct<T, V>();
            if (act == null)
            {
                Idle();
                return;
            }

            if (CurrIAct == act)
                return;
            
            if (act is not Action.Idle &&
                CurrIAct is not Action.Idle)
            {
                CurrIAct?.End();
                await UniTask.Yield();
            }

            param = GetData<V>(act, param);
            // data?.SetNoEnd(noEnd);
            
            act.Execute();
            SetCurrIAct(act);
            
            act.ChainUpdateAsync().Forget();
        }
        
        private void EndAct()
        {
            Idle();
        }

        private void SetCurrIAct(IAct iAct)
        {
            CurrIAct = iAct;
            
            // Debug.Log(name + " = " + iAct?.GetType());
        }
    }
}

