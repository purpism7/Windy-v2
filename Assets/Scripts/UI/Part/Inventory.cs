using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

using Cysharp.Threading.Tasks;
using DG.Tweening;

using GameSystem;
using UI.Slot;
using GameSystem.Event;

namespace UI.Part
{
    public class Inventory : Common.Component<Inventory.Param>, InputManager.IKeyListener
    {
        public new class Param : Common.Component.Param
        {
            
        }

        [SerializeField] private HorizontalLayoutGroup[] _horizontalLayoutGroups = null;

        private enum EState
        {
            None,
            
            IsAscending,
            IsDescending,
        }
        
        private List<EmptySlot> _emptySlotList = null;

        private RectTransform _rectTm = null;
        private float _bounceHeight = 110f;
        private float _duration = 0.3f;

        private bool _isLanded = true;
        private EState _eState = EState.None;

        public override async UniTask InitializeAsync()
        {
            _rectTm = GetComponent<RectTransform>();

            InitializeEmptySlotList();
            
            GameSystem.Event.EventDispatcher.Register<GameSystem.Event.Item>(OnChanged);
            Manager.Get<IInput>()?.AddListener(this);

            await UniTask.CompletedTask;
        }

        public override async UniTask BeforeActivateAsync()
        {
            await ApplyItemListAsync();
        }

        public override UniTask AfterActivateAsync()
        {
            return UniTask.CompletedTask;
        }

        private void InitializeEmptySlotList()
        {
            if (_emptySlotList == null)
            {
                _emptySlotList = new();
                _emptySlotList.Clear();
            }
            
            foreach (var horizontalLayoutGroup in _horizontalLayoutGroups)
            {
                var emptySlots = horizontalLayoutGroup?.GetComponentsInChildren<EmptySlot>(true);
                if(!emptySlots.IsNullOrEmpty())
                    _emptySlotList?.AddRange(emptySlots);
            }
            
            for (int i = 0; i < _emptySlotList?.Count; ++i)
            {
                _emptySlotList[i]?.InitializeAsync();
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
            
            await _rectTm.DOLocalMoveY( _bounceHeight, _duration)
                .SetEase(Ease.InBack, 3f); 

            _eState = EState.None;
            _isLanded = false;
        }
        
        private async UniTaskVoid BounceInQuadAsync()
        {
            if (!_rectTm)
                return;
            
            if (_eState != EState.None) 
                return;
            
            _eState = EState.IsDescending;

            await _rectTm.DOLocalMoveY( - _bounceHeight, _duration)
                .SetEase(Ease.OutBack, 3f);
            
            _eState = EState.None;
            _isLanded = true;
        }

        private async UniTask ApplyItemListAsync()
        {
            if (_emptySlotList == null)
                return;

            await UniTask.Yield(PlayerLoopTiming.PostLateUpdate);
            
            var itemInfoList = InfoManager.Get<Info.Inventory>()?.ItemList;
            if (itemInfoList == null)
                return;


            for (int i = 0; i < _emptySlotList.Count; ++i)
            {
                var emptySlot = _emptySlotList[i];
                if(emptySlot == null)
                    continue;
                
                emptySlot.Deactivate();
                
                if(itemInfoList.Count <= i)
                    continue;
                
                var emptySlotParam = new EmptySlot.Param();
                emptySlotParam.WithItemId(itemInfoList[i].Id)?
                    .WithItemCount(itemInfoList[i].Count);

                await emptySlot.ActivateWithParamAsync(emptySlotParam);
                // _emptySlotList[i]?.Activate(emptySlotParam);
            }
        }
        
        #region GameSystem.Event
        private void OnChanged(GameSystem.Event.Item item)
        {
            switch (item)
            {
                case AddItem addItem:
                case RemoveItem removeItem:
                {
                    ApplyItemListAsync().Forget();
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
                
                if(_isLanded)
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
    }
}

