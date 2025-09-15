using UnityEngine;

using Cysharp.Threading.Tasks;
using DG.Tweening;

using GameSystem;
using Table;

namespace UI.Part
{
    public class GetItem : PartWorld<GetItem.Param>
    {
        public new class Param : PartWorld<GetItem.Param>.Param
        {
            public int ItemId { get; private set; } = 0;
            public System.Action Callback { get; private set; } = null;

            public Param WithItemId(int itemId)
            {
                ItemId = itemId;
                return this;
            }

            public Param WithCallback(System.Action callback)
            {
                Callback = callback;
                return this;
            }
        }

        [SerializeField] private UnityEngine.UI.Image itemImage = null; 
        
        public override async UniTask InitializeAsync()
        {
            await base.InitializeAsync();
        }

        public override UniTask BeforeActivateAsync()
        {
            if(ApplyItemSprite())
                AppearMoveUpEffectAsync().Forget();
            else
                Deactivate();

            return UniTask.CompletedTask;
        }

        public override UniTask AfterActivateAsync()
        {
            return UniTask.CompletedTask;
        }

        public override void Deactivate()
        {
            base.Deactivate();
            
            _param?.Callback?.Invoke();
        }

        private bool ApplyItemSprite()
        {
            itemImage?.SetActive(false);

            if (itemImage == null)
                return false;
            
            if (_param == null)
                return false;
            
            var itemData = ItemDataContainer.Instance?.GetData(_param.ItemId);
            if (itemData == null)
                return false;
            
            var sprite = AtlasManager.Instance?.GetSprite(Common.EAtlasKey.UIItems, itemData.ImageName);
            itemImage.sprite = sprite;
            
            itemImage?.SetActive(true);

            return true;
        }

        private async UniTask AppearMoveUpEffectAsync()
        {
            var imgRectTm = itemImage?.GetComponent<RectTransform>();
            if (!imgRectTm)
                return;
            
            imgRectTm.anchoredPosition = Vector2.one;
            
            await AppearEffectAsync();
            await imgRectTm.DOLocalMoveY(35f, 1f).SetEase(Ease.OutBack);
            
            Deactivate();
        }
    }
}

