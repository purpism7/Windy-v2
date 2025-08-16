using UnityEngine;

using Creature.Interactions;

namespace Creature.Characters
{
    public class InteractionObjectMediator : InteractionMediator<Object>
    {
        private IActor _interactionIActor = null;
        
        public override void Initialize(Object obj)
        {
            base.Initialize(obj);
        }

        private void StartSways(GameObject gameObj)
        {
            if (_interactionIActor != null)
                return;
            
            var iActor = gameObj.GetComponentInParent<IActor>(true);
            if (iActor != null)
            {
                if (_t.Order <= iActor.Order)
                    return;
                
                _t?.IInteractionCtr?.Execute<Walk, Walk.Data>();
                _interactionIActor = iActor;
            }
        }

        private void StopSways()
        {
            if (_t.Order > _interactionIActor.Order)
                return;
            
            _t?.IInteractionCtr?.Execute<Idle, Idle.Data>();
            _interactionIActor = null;
        }
        
        #region TriggerEvent
        public override void OnEnter(GameObject gameObj)
        {
            StartSways(gameObj);
        }

        public override void OnExit(GameObject gameObj)
        {
            _t?.IInteractionCtr?.Execute<Idle, Idle.Data>();
            _interactionIActor = null;
        }

        public override void OnStay(GameObject gameObj)
        {
            base.OnStay(gameObj);

            if (_interactionIActor != null)
                StopSways();
            else
                StartSways(gameObj);
        }

        #endregion 
    }
}
