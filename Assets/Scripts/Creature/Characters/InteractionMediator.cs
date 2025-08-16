using System;
using UnityEngine;

using Cysharp.Threading.Tasks;

using Creator;
using Creature.Action;
using UI.Part;
using GameSystem;
using Creature.Characters;

namespace Creature.Characters
{
    public abstract class InteractionMediator<T> : MonoBehaviour, TriggerEvent.IListener, InputManager.IKeyListener
    {
        protected T _t = default;
        
        private TriggerEvent _triggerEvent = null;

        public virtual void Initialize(T t)
        {
            _t = t;
           
            _triggerEvent = transform.GetComponentInChildren<TriggerEvent>(true);
            _triggerEvent?.Initialize(this);
            
            Manager.Get<IInput>()?.AddListener(this);
        }

        public virtual void Activate()
        {
            
        }

        public virtual void Deactivate()
        {
            
        }

        public virtual void ChainUpdate()
        {
            
        }

        public virtual void ChainLateUpdate()
        {
          
        }

        public virtual void ChainFixedUpdate()
        {
           
        }
        
        #region TriggerEvent.IListener

        public abstract void OnEnter(GameObject gameObj);
        public abstract void OnExit(GameObject gameObj);
        
        public virtual void OnStay(GameObject gameObj)
        {
            
        }

        
        #endregion
        
        #region InputManager.IListener
        public virtual void OnKey(KeyCode keyCode)
        {
            
        }
        
        public virtual void OnKeyDown(KeyCode keyCode)
        {
            
        }
        #endregion
    }
}

