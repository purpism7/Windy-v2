using System;
using System.Collections.Generic;
using Creator;
using UnityEngine;

using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;

using GameSystem;
using Network.Api.Recipe;
using Table;
using UI.Slot;

namespace UI.Part
{
    public class RecipePart : Common.Component<RecipePart.Param>, InputManager.IKeyListener, EmptySlot.IListener
    {
        public new class Param : Common.Component.Param
        {
            
        }
        
        private enum EState
        {
            None,
            
            IsAscending,
            IsDescending,
        }

        [SerializeField] private RectTransform emptySlotRootRectTm = null;
        
        [Header("Selected Item")]
        [SerializeField] private ItemSlot selectedItemSlot = null;
        [SerializeField] private TextMeshProUGUI selectedItemNameTmp = null;
        
        [Header("Material Item List")]
        [SerializeField] private RectTransform materialItemListRootRectTm = null;
        
        private List<EmptySlot> _emptySlotList = null;
        private List<ItemSlot> _materialItemSlotList = null;
        
        private RectTransform _rectTm = null;
        private float _bounceValue = 225f;
        private float _duration = 0.3f;

        private bool _isBounceIn = true;
        private EState _eState = EState.None;
        
        public override async UniTask InitializeAsync()
        {
            _rectTm = GetComponent<RectTransform>();

            InitializeEmptySlotList();
            ApplySelectedItem(0);
            
            GameSystem.Event.EventDispatcher.Register<GameSystem.Event.Recipe>(OnChanged);
            Manager.Get<IInput>()?.AddListener(this);

            await UniTask.CompletedTask;
        }

        public override async UniTask ActivateAsync()
        {
            await ApplyRecipeListAsync();
        }
        
        private void InitializeEmptySlotList()
        {
            if (_emptySlotList == null)
            {
                _emptySlotList = new();
                _emptySlotList.Clear();
            }
            
            var emptySlots = emptySlotRootRectTm?.GetComponentsInChildren<EmptySlot>(true);
            if(!emptySlots.IsNullOrEmpty())
                _emptySlotList?.AddRange(emptySlots);

            for (int i = 0; i < _emptySlotList?.Count; ++i)
            {
                _emptySlotList[i]?.InitializeAsync();
            }
        }
        
        private async UniTask ApplyRecipeListAsync()
        {
            if (_emptySlotList == null)
                return;

            await UniTask.Yield(PlayerLoopTiming.PostLateUpdate);
            
            var recipeIdList = InfoManager.Get<Info.Recipe>()?.RecipeIdList;
            if (recipeIdList == null)
                return;
            
            for (int i = 0; i < _emptySlotList.Count; ++i)
            {
                var emptySlot = _emptySlotList[i];
                if(emptySlot == null)
                    continue;
                
                emptySlot.Deactivate();
                
                if(recipeIdList.Count <= i)
                    continue;

                var recipeData = RecipeDataContainer.Instance?.GetData(recipeIdList[i]);
                if(recipeData == null)
                    continue;
                
                var emptySlotParam = new EmptySlot.Param()
                    .WithIListener(this)
                    .WithItemId(recipeData.ResultItemId);

                await emptySlot.ActivateWithParamAsync(emptySlotParam);
            }
        }

        private async UniTaskVoid BounceOutQuadAsync()
        {
            if (!_rectTm)
                return;
            
            // 이미 올라간 상태면 무시
            if (_eState != EState.None) 
                return;
            
            _eState = EState.IsAscending;
            
            await _rectTm.DOLocalMoveX( _bounceValue, _duration)
                .SetEase(Ease.InBack, 3f); 

            _eState = EState.None;
            _isBounceIn = false;

            AllDeselectEmptySlotList();
            ApplySelectedItem(0);
            AllDeactivateMaterialItemSlotList();
        }
        
        private async UniTaskVoid BounceInQuadAsync()
        {
            if (!_rectTm)
                return;
            
            if (_eState != EState.None) 
                return;
            
            _eState = EState.IsDescending;
            
            await _rectTm.DOLocalMoveX( - _bounceValue, _duration)
                .SetEase(Ease.OutBack, 3f);
            
            _eState = EState.None;
            _isBounceIn = true;
        }

        #region Selected Item
        private void ApplySelectedItem(int itemId)
        {
            selectedItemSlot?.Deactivate();
            selectedItemNameTmp?.SetText(string.Empty);
            
            var itemData = ItemDataContainer.Instance?.GetData(itemId);
            if(itemData == null)
                return;
            
            var recipeData = RecipeDataContainer.Instance?.GetDataByResultItemId(itemId);
            if(recipeData == null)
                return;

            var itemSlotParam = new ItemSlot.Param(itemId)
                .WithItemCount(recipeData.ResultItemCount);
            
            // selectedItemSlot?.Activate(itemSlotParam);
            // var sprite = AtlasManager.Instance.GetSprite(EAtlasKey.UIItems, itemData.ImageName);
            // if (selectedItemImage != null &&
            //     sprite != null)
            // {
            //     selectedItemImage.sprite = sprite;
            //     selectedItemImage?.SetActive(true);
            // }
            
            selectedItemNameTmp?.SetText(itemData.LocalKey?.GetLocalization());
        }

        private void AllDeselectEmptySlotList()
        {
            for (int i = 0; i < _emptySlotList?.Count; ++i)
            {
                IEmptySlot iEmptySlot = _emptySlotList[i];
                iEmptySlot?.SetSelected(false);
            }
        }
        #endregion

        #region Material Item List

        private void AllDeactivateMaterialItemSlotList()
        {
            if (_materialItemSlotList.IsNullOrEmpty())
                return;

            for (int i = 0; i < _materialItemSlotList.Count; ++i)
            {
                _materialItemSlotList[i]?.Deactivate();
            }
        }
        
        private void ApplyMaterialItemList(int itemId)
        {
            Extensions.SetActive(materialItemListRootRectTm, false);
            
            var recipeData = RecipeDataContainer.Instance?.GetDataByResultItemId(itemId);
            if (recipeData == null)
                return;

            var materialItemIds = recipeData.MaterialItemIds;
            if (materialItemIds.IsNullOrEmpty())
                return;

            if (_materialItemSlotList == null)
            {
                _materialItemSlotList = new();
                _materialItemSlotList.Clear();
            }
            
            for (int i = 0; i < materialItemIds.Length; ++i)
            {
                CreateItemSlotAsync(recipeData, materialItemIds[i], i).Forget();
            }
            
            Extensions.SetActive(materialItemListRootRectTm, true);
        }

        private async UniTask CreateItemSlotAsync(Table.RecipeData recipeData, int materialItemId, int index)
        {
            if(recipeData.MaterialItemCounts.IsNullOrEmpty() ||
               recipeData.MaterialItemCounts.Length < index)
                return;
                
            ItemSlot.Param itemSlotParam = new ItemSlot.Param(materialItemId)
                .WithItemCount(recipeData.MaterialItemCounts[index]);
            
            var materialItemSlot = _materialItemSlotList?.Find(itemSlot => !itemSlot.IsActivate);
            if (materialItemSlot == null)
            {
                materialItemSlot = await UICreator<ItemSlot, ItemSlot.Param>.Get
                    .SetRootTm(materialItemListRootRectTm)
                    .CreateAsync();
                    
                _materialItemSlotList?.Add(materialItemSlot); 
            }

            await materialItemSlot.ActivateWithParamAsync(itemSlotParam);
        }
        #endregion

        #region GameSystem.Event
        private void OnChanged(GameSystem.Event.Recipe data)
        {
            switch (data)
            {
                case GameSystem.Event.AddRecipe addRecipe:
                {
                    ApplyRecipeListAsync().Forget();
                    break;
                }
            }
        }
        #endregion
        
        #region InputManager.IKeyListener
        void InputManager.IKeyListener.OnKeyDown(KeyCode keyCode)
        {
            if (keyCode == KeyCode.Tab)
            {
                if (_eState != EState.None)
                    return;
                
                if(_isBounceIn)
                    BounceOutQuadAsync().Forget();
                else 
                    BounceInQuadAsync().Forget();
                
                return;
            }
        }

        void InputManager.IKeyListener.OnKey(KeyCode keyCode)
        {
            
        }
        #endregion
        
        #region EmptySlot.IListener
        void EmptySlot.IListener.Selected(IEmptySlot selectedIEmptySlot)
        {
            if (selectedItemSlot != null &&
                selectedItemSlot.ItemId == selectedIEmptySlot.ItemId)
                return;
            
            AllDeselectEmptySlotList();
            
            selectedIEmptySlot?.SetSelected(true);
            if (selectedIEmptySlot != null)
            {
                ApplySelectedItem(selectedIEmptySlot.ItemId);
                AllDeactivateMaterialItemSlotList();
                ApplyMaterialItemList(selectedIEmptySlot.ItemId);
            }
        }
        #endregion
    }
}

