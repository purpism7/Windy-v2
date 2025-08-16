using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Common
{
    public class Component : MonoBehaviour
    {
        public class Param
        {
            
        }
        
        [SerializeField]
        private Transform rootTm = null;

        private bool _isActivate = false;

        public bool IsActivate
        {
            get
            {
                if (!rootTm)
                    return false;
                
                return _isActivate;
            }
        }

        public virtual void Initialize()
        {
            
        }
        
        public virtual void Activate()
        {
            _isActivate = true;
            Extensions.SetActive(rootTm, true);
        }
        
        public virtual void Deactivate()
        {
            _isActivate = false;
            Extensions.SetActive(rootTm, false);
        }

        public virtual void ChainUpdate()
        {
            
        }
        
        public virtual void ChainLateUpdate()
        {
            
        }
    }
    
    public abstract class Component<T> : Component where T : Component.Param
    {
        protected T _param = null;

        public abstract UniTask InitializeAsync();
        public abstract UniTask ActivateAsync();

        public Component<T> SetParam(T param)
        {
            if(param != null)
                _param = param;

            return this;
        }

        public async UniTask<Component<T>> ActivateWithParamAsync(T param)
        {
            await SetParam(param).ActivateAsync();

            Activate();

            return this;
        }
    }
}