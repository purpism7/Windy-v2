using UnityEngine;

using Cysharp.Threading.Tasks;

using Creator;
using GameSystem;
using Table;

namespace UI.Slot
{
    public interface IEmptySlot
    {
        int ItemId { get; }
        void SetSelected(bool isSelected);
    }
    
    public class EmptySlot : Common.Component<EmptySlot.Param>, IEmptySlot
    {
        public new class Param : Common.Component.Param
        {
            public IListener IListener { get; private set; } = null;
            public int ItemId { get; private set; } = 0;
            public int ItemCount { get; private set; } = 0;

            public Param WithIListener(IListener iListener)
            {
                IListener = iListener;
                return this;
            }
            
            public Param WithItemId(int itemId)
            {
                ItemId = itemId;
                return this;
            }
            
            public Param WithItemCount(int itemCount)
            {
                ItemCount = itemCount;
                return this;
            }
        }

        public interface IListener
        {
            void Selected(IEmptySlot selectedIEmptySlot);
        }

        [SerializeField] private RectTransform itemSlotRootRectTm = null;
        [SerializeField] private RectTransform selectedRectTm = null;
        [SerializeField] private UnityEngine.UI.Button btn = null;

        private ItemSlot _itemSlot = null;
        
        public bool IsEmpty
        {
            get { return _itemSlot == null; }
        }

        public override UniTask InitializeAsync()
        {
            btn?.onClick?.AddListener(OnClick);
            
            return UniTask.CompletedTask;
        }

        public override async UniTask BeforeActivateAsync()
        {
            await CreateItemSlotAsync();
        }

        public override UniTask AfterActivateAsync()
        {
            return UniTask.CompletedTask;
        }

        public override void Deactivate()
        {
            // base.Deactivate();
            
            _itemSlot?.Deactivate();
        }

        private async UniTask CreateItemSlotAsync()
        {
            if (_param == null)
                return;
            
            var itemData = ItemDataContainer.Instance?.GetData(_param.ItemId);
            if (itemData == null)
                return;

            var itemSlotParam = new ItemSlot.Param(itemData.Id)
                .WithItemCount(_param.ItemCount);
            
            if (_itemSlot == null)
            {
                _itemSlot = await UICreator<ItemSlot, ItemSlot.Param>.Get
                    .SetRootTm(itemSlotRootRectTm)
                    .CreateAsync();
            }

            await _itemSlot.ActivateWithParamAsync(itemSlotParam);
        }

        #region IEmptySlot
        int IEmptySlot.ItemId
        {
            get
            {
                if (IsEmpty)
                    return 0;

                return _param.ItemId;
            }
        }
        
        void IEmptySlot.SetSelected(bool isSelected)
        {
            Extensions.SetActive(selectedRectTm, isSelected);
        }
        #endregion

        #region OnClick
        private void OnClick()
        {
            if (IsEmpty)
                return;
            
            _param?.IListener?.Selected(this);
        }
        #endregion
    }
}

