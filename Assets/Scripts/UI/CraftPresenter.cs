using Cysharp.Threading.Tasks;
using UnityEngine;

namespace UI
{
    public interface ICraftPresenter : IPresenter<ICraftPresenter, ICraftView>
    {
     
    }
    
    public class CraftPresenter : ICraftPresenter
    {
        public ICraftPresenter Initialize(ICraftView iView)
        {
            return this;
        }
        
        async UniTask IPresenter<ICraftPresenter, ICraftView>.ActivateAsync()
        {
            await UniTask.CompletedTask;
        }

        async UniTask IPresenter<ICraftPresenter, ICraftView>.DeactivateAsync()
        {
      
            await UniTask.CompletedTask;
        }
        
        void IPresenter<ICraftPresenter, ICraftView>.ChainUpdate()
        {
            
        }
    }
}

