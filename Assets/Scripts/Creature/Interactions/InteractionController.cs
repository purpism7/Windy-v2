using System;
using System.Collections.Generic;
using UnityEngine;

using Cysharp.Threading.Tasks;

using Table;

namespace Creature.Interactions
{
    public interface IInteractionController : IController<IInteractionController, IInteractable>
    {
        void Execute<T, V>(V data = null) where T : BaseInteraction<V>, new() where V : BaseInteraction<V>.Data;
    }
    
    public class InteractionController : Controller, IInteractionController, BaseInteraction<Chop.Data>.IListener
    {
        private IInteractable _iInteractable = null;
        private Dictionary<System.Type, BaseInteraction> _cachedInteractionDic = null;
        
        IInteractionController IController<IInteractionController, IInteractable>.Initialize(IInteractable iInteractable)
        {
            _iInteractable = iInteractable;
            
            return this;
        }

        #region IController
        void IController<IInteractionController, IInteractable>.ChainUpdate()
        {
            if (!IsActivate)
                return;
        }

        void IController<IInteractionController, IInteractable>.ChainLateUpdate()
        {
            if (!IsActivate)
                return;
            
            // CurrIAct?.ChainLateUpdate();
        }
        
        void IController<IInteractionController, IInteractable>.ChainFixedUpdate()
        {
            if (!IsActivate)
                return;
            
            // CurrIAct?.ChainFixedUpdate();
        }

        public override void Activate()
        {
            base.Activate();
            
            SwayAsync().Forget();
        }

        public override void Deactivate()
        {
            base.Deactivate();
        }
        #endregion

        private async UniTask SwayAsync()
        {
           _iInteractable?.IInteractionCtr?.Execute<Idle, Idle.Data>();

            var randomSec = UnityEngine.Random.Range(2f, 10f);
            await UniTask.Delay(TimeSpan.FromSeconds(randomSec));
            
           _iInteractable?.IInteractionCtr?.Execute<Sway, Sway.Data>();
            
            randomSec = UnityEngine.Random.Range(2f, 10f);
            await UniTask.Delay(TimeSpan.FromSeconds(randomSec));
            
            if(IsActivate)
                SwayAsync().Forget();
        }
        
        #region IInteractionController

        void IInteractionController.Execute<T, V>(V data)
        {
            if (!IsActivate)
                return;
            
            if (_cachedInteractionDic == null)
            {
                _cachedInteractionDic = new();
                _cachedInteractionDic.Clear();
            }

            BaseInteraction baseInteraction = null;
            if (!_cachedInteractionDic.TryGetValue(typeof(T), out baseInteraction))
            {
                baseInteraction = new T();
                _cachedInteractionDic?.TryAdd(typeof(T), baseInteraction);
            }
            
            var interaction = baseInteraction as T;
            if (interaction == null)
                return;
                    
            interaction.SetData(data)?
                .SetIInteractable(_iInteractable)?
                .Execute();
        }

        #endregion
        
        #region BaseInteraction.IListener

        void BaseInteraction<Chop.Data>.IListener.End(BaseInteraction<Chop.Data> interaction)
        {
            var chop = interaction as Chop;
            if (chop == null)
                return;

            if (chop.IsFinished)
            {
                var breakData = new Break.Data()
                    .WithObjectId(_iInteractable.Id)
                    .WithPosition(_iInteractable.Transform.position)
                    .WithEItemInteraction(chop.EItemInteraction);
            
                _iInteractable?.IInteractionCtr?.Execute<Break, Break.Data>(breakData);
             
                // Debug.Log(chop.Is);
                chop.Clear();
            }
            else
                SwayAsync().Forget();
        }
        #endregion
    }
}

