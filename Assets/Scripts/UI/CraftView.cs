using Cysharp.Threading.Tasks;
using UI.Slot;
using UnityEngine;

namespace UI
{
    public interface ICraftView : IView
    {
        RectTransform MaterialRootRectTm { get; }
    }
    
    public class CraftView : BaseView<CraftView.Param>, ICraftView
    {
        
        
        public class Param : Common.Component.Param
        {
            
        }

        [SerializeField] private RectTransform materialRootRectTm = null;
        
        private ICraftPresenter _iCraftPresenter = null;
        
        public RectTransform MaterialRootRectTm => materialRootRectTm;
        
        public override void CreatePresenter()
        {
            _iCraftPresenter = new CraftPresenter()
                .Initialize(this);
        }

        public override async  UniTask InitializeAsync()
        {
            await UniTask.CompletedTask;
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

