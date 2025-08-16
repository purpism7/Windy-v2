using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Cysharp.Threading.Tasks;

namespace GameSystem
{
    public interface IGeneric
    {
        UniTask<IGeneric> InitializeAsync();
        void ChainUpdate();
        void ChainLateUpdate();
        void ChainFixedUpdate();
    }
    
    public interface IManager : IGeneric
    {
        
    }
    
    public abstract class Manager : MonoBehaviour
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
        
        public static T Get<T>() where T : IManager
        {
            var iMgrGenericList = MainManager.Instance?.IMgrGenericList;
            if (iMgrGenericList == null)
                return default;

            foreach (var iMgrGeneric in iMgrGenericList)
            {
                if (iMgrGeneric is T)
                    return (T)iMgrGeneric;
            }
        
            return default;
        }
    }
}
