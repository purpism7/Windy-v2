using UnityEngine;

using Cysharp.Threading.Tasks;

using Creator;
using GameSystem;
using UI.Slot;
using Table;

namespace UI
{
    public interface ICraftPresenter : IPresenter<ICraftPresenter, ICraftView>
    {
     
    }
    
    public class CraftPresenter : ICraftPresenter
    {
        private ICraftView _iCraftView = null;
        private EmptySlot[] _craftItemSlots = null;
        
        public ICraftPresenter Initialize(ICraftView iView)
        {
            _iCraftView = iView;
            
            _craftItemSlots = iView?.MaterialRootRectTm.GetComponentsInChildren<EmptySlot>();
            
            return this;
        }
        
        async UniTask IPresenter<ICraftPresenter, ICraftView>.ActivateAsync()
        {
            AllDeactivateCraftItemSlots();
            ApplyCraftItemSlots();
            
            await UniTask.CompletedTask;
        }

        async UniTask IPresenter<ICraftPresenter, ICraftView>.DeactivateAsync()
        {
      
            await UniTask.CompletedTask;
        }
        
        void IPresenter<ICraftPresenter, ICraftView>.ChainUpdate()
        {
            
        }
        
        #region 
        private void AllDeactivateCraftItemSlots()
        {
            if (_craftItemSlots.IsNullOrEmpty())
                return;

            for (int i = 0; i < _craftItemSlots.Length; ++i)
            {
                _craftItemSlots[i]?.Deactivate();
                // _craftItemSlots[i]?.transform.SetActive(false);
            }
        }
        
        private void ApplyCraftItemSlots()
        {
            Extensions.SetActive(_iCraftView?.MaterialRootRectTm, false);
            
            var questData = Manager.Get<IMission>()?.Quest?.CurrentQuestData;
            if (questData == null)
                return;
            
            var recipeDataList = RecipeDataContainer.Instance?.GetRecipeDataListByQuest(questData.Group, questData.Step, true);
            if (recipeDataList == null)
                return;

            for (int i = 0; i < recipeDataList.Count; ++i)
            {
                var recipeData = recipeDataList[i];
                if(recipeData == null)
                    continue;
                
                if(_craftItemSlots.Length <= i)
                    continue;
                
                EmptySlot.Param emptySlotParam = new EmptySlot.Param()
                    .WithItemId(recipeData.ResultItemId)
                    .WithItemCount(1);

                _craftItemSlots[i]?.ActivateWithParamAsync(emptySlotParam);
            }
            
            Extensions.SetActive(_iCraftView?.MaterialRootRectTm, true);
        }
        #endregion
    }
}

