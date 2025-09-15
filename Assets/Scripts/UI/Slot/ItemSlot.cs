using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

using TMPro;

using GameSystem;
using Table;


namespace UI.Slot
{
    public class ItemSlot : Common.Component<ItemSlot.Param>
    {
        public class Param : Common.Component.Param
        {
            public int ItemId { get; private set; } = 0;
            public int ItemCount { get; private set; } = 0;

            public Param(int itemId)
            {
                ItemId = itemId;
            }
 
            public Param WithItemCount(int itemCount)
            {
                ItemCount = itemCount;
                return this;
            }
        }

        [SerializeField] private UnityEngine.UI.Image itemImage = null;
        [SerializeField] private TextMeshProUGUI itemCountTMP = null;
        
        private int _itemId = 0;

        public int ItemId => _itemId;

        public override async UniTask InitializeAsync()
        {
            await UniTask.CompletedTask;
        }

        public override async UniTask BeforeActivateAsync()
        {
            if (_param == null)
                return;
            // if (_data.ItemCount <= 0)
            // {
            //     Deactivate();
            //     return;
            // }

            if (_param.ItemId != _itemId)
                ApplyItemSprite();
            
            ApplyItemCount();

            _itemId = _param.ItemId;

            await UniTask.CompletedTask;
        }

        public override UniTask AfterActivateAsync()
        {
            return UniTask.CompletedTask;
        }

        public override void Deactivate()
        {
            base.Deactivate();

            _itemId = 0;
        }

        private void ApplyItemSprite()
        {
            itemImage?.SetActive(false);

            if (itemImage == null)
                return;
            
            if (_param == null)
                return;
            
            var itemData = ItemDataContainer.Instance?.GetData(_param.ItemId);
            if (itemData == null)
                return;
            
            var sprite = AtlasManager.Instance?.GetSprite(Common.EAtlasKey.UIItems, itemData.ImageName);
            itemImage.sprite = sprite;
            
            itemImage?.SetActive(true);
        }

        private void ApplyItemCount()
        {
            if (_param == null)
                return;

            var itemCountText = string.Empty;
            if (_param.ItemCount > 0)
                itemCountText = $"{_param.ItemCount}";
            
            itemCountTMP?.SetText(itemCountText);
        }
    }
}

