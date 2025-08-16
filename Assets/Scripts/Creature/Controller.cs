using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Creature
{
    public interface IController<T, V> where V : ISubject
    {
        T Initialize(V v);
        void ChainUpdate();
        void ChainLateUpdate();
        void ChainFixedUpdate();
        void Activate();
        void Deactivate();
    }

    public abstract class Controller : MonoBehaviour
    {
        public bool IsActivate { get; private set; } = false;

        public virtual void Activate()
        {
            IsActivate = true;
        }
        
        public virtual void Deactivate()
        {
            IsActivate = false;
        }
    }
}

