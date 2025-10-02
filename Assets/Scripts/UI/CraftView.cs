using Cysharp.Threading.Tasks;
using UnityEngine;

namespace UI
{
    public interface ICraftView : IView
    {
        
    }
    
    public class CraftView : BaseView<CraftView.Param>, ICraftView
    {
        private ICraftPresenter _iCraftPresenter = null;
        
        public class Param : Common.Component.Param
        {
            
        }

        public override void CreatePresenter()
        {
            _iCraftPresenter = new CraftPresenter()
                .Initialize(this);
        }

        public override UniTask InitializeAsync()
        {
            return UniTask.CompletedTask;
        }

        public override UniTask BeforeActivateAsync()
        {
            return UniTask.CompletedTask;  
        }

        public override UniTask AfterActivateAsync()
        {
            return UniTask.CompletedTask;
        }
    }
}

