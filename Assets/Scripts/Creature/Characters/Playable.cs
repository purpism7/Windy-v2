using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Creator; 
using Creature.Action;
using GameSystem;
using GameSystem.Event;


namespace Creature.Characters
{
    public class Playable : Character, InputManager.IAxisListener
    {
        private InteractionPlayableMediator _interactionPlayableMediator = null;
        
        public override void Initialize()
        {
            base.Initialize();

            _interactionPlayableMediator = transform.AddOrGetComponent<InteractionPlayableMediator>();
            _interactionPlayableMediator?.Initialize(this);
            
            Manager.Get<IInput>()?.AddListener(this);
            GameSystem.Manager.Get<GameSystem.ICameraManager>()?.SetPlayable(this);
            
            Activate();
        }

        public override string AnimationKey<T>(Act<T> act)
        {
            switch (act)
            {
                case Move:
                    return "walk";
            }
            
            return act?.GetType().Name.ToLower();
        }

        public override void ChainUpdate()
        {
            if (!IsActivate)
                return;
            
            base.ChainUpdate();
            
            _interactionPlayableMediator?.ChainUpdate();
        }

        public override void ChainLateUpdate()
        {
            if (!IsActivate)
                return;
            
            base.ChainLateUpdate();
            
            _interactionPlayableMediator?.ChainLateUpdate();
        }
        
        public override void ChainFixedUpdate()
        {
            if (!IsActivate)
                return;
            
            base.ChainFixedUpdate();
            
            _interactionPlayableMediator?.ChainFixedUpdate();
        }
        
        #region InputManager.IAxisListener
        void InputManager.IAxisListener.OnAxisChanged(float horizontal, float vertical)
        {
            var iActCtr = IActCtr;
            if (iActCtr == null)
                return;
            
            if (iActCtr.IsInteraction)
                return;
            
            var input = Manager.Get<IInput>();
            if (input.IsStopped)
                iActCtr.ExecuteAsync<Idle, Idle.Param>();
            else
                iActCtr.ExecuteAsync<Move, Move.Param>(); 
        }
        #endregion
        
        // #region Event
        // private void OnChanged(Quest quest)
        // {
        //     Debug.Log(quest.Id);
        // }
        // #endregion
    }
}
    
