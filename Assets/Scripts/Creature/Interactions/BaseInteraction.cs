using JetBrains.Annotations;
using UnityEngine;

namespace Creature.Interactions
{
    public abstract class BaseInteraction
    {
        protected IInteractable _iInteractable = null;
        protected bool _isEnded = false;
        
        public BaseInteraction SetIInteractable(IInteractable iInteractable)
        {
            _iInteractable = iInteractable;
            return this;
        }

        public virtual void Execute()
        {
            _isEnded = false;
        }

        public virtual void End()
        {
            _isEnded = true;
        }
    }
    
    public abstract class BaseInteraction<T> : BaseInteraction where T : BaseInteraction<T>.Data
    {
        public class Data
        {
          
        }
        
        public interface IListener
        {
            void End(BaseInteraction<T> interaction);
        }

        protected T _data = null;
        protected IListener _iListener = null;
        
        protected abstract string AnimationName { get; }

        public override void End()
        {
            base.End();
            
            _iListener?.End(this);
        }

        public BaseInteraction<T> SetData(T data)
        {
            _data = data;
            return this;
        }

        public virtual void Clear()
        {
            
        }
    }
}

