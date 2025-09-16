using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using NavMeshPlus.Components;
using Spine.Unity;

using Creature.Interactions;
using Creature.Characters;
using Cysharp.Threading.Tasks;

using Table;
using Common;


namespace Creature
{
    public interface IObject : IInteractable
    {
        EItem EItem { get; }
        EItemInteraction EItemInteraction { get; }
        void SetPosition(Vector3 pos);
    }
    
    public class Object : Common.Component, IObject
    {
        [SerializeField] private int id = 0;
        [SerializeField] private EItemInteraction eItemInteraction = EItemInteraction.None;
        [SerializeField] private int orderOffset = 0;

        private MeshRenderer _meshRenderer = null;
        private InteractionObjectMediator _interactionObjectMediator = null;
        private SpriteRenderer[] _spriteRenderers = null;
        private int _order = 0;
        //private NavMeshModifierVolume[] _navMeshModifierVolumes = null;
        
        public SkeletonAnimation SkeletonAnimation { get; private set; } = null;
        public IInteractionController IInteractionCtr { get; private set; } = null;
        public Collider2D Collider { get; private set; } = null;

        public EItem EItem { get; private set; } = EItem.None;

        public int Id
        {
            get { return id; }
        }

        public Transform Transform
        {
            get { return transform; }
        }

        public int Order
        {
            get { return _order; }
        }
        
        public EItemInteraction EItemInteraction { get { return eItemInteraction; } }

        private void Awake()
        {
            Initialize();
        }

        public override void Initialize()
        {
            base.Initialize();

            InitializeSkeletonAnimation();
            InitializeAnimationController();
            InitializeInteractionObjectMediator();

            _spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
            Collider = GetComponentInChildren<Collider2D>(true);

            //_navMeshModifierVolumes = GetComponentsInChildren<NavMeshModifierVolume>(true);
            
            SortingOrder(-transform.position.y);
            Activate();
            InitializeIEItemAsync().Forget();
        }

        private async UniTask InitializeIEItemAsync()
        {
            await UniTask.WaitUntil(() => ItemDataContainer.Instance != null);
            
            var itemData = ItemDataContainer.Instance?.GetData(Id);
            if (itemData != null)
                EItem = itemData.EItem;
        }

        private void InitializeAnimationController()
        {
            IInteractionCtr = transform.AddOrGetComponent<InteractionController>();
            IInteractionCtr?.Initialize(this);
        }
        
        private void InitializeSkeletonAnimation()
        {
            if (SkeletonAnimation != null)
                return;
            
            SkeletonAnimation = transform.GetComponentInChildren<SkeletonAnimation>(true);
            SkeletonAnimation?.Initialize(true);
        }

        private void InitializeInteractionObjectMediator()
        {
            _interactionObjectMediator = transform.AddOrGetComponent<InteractionObjectMediator>();
            _interactionObjectMediator?.Initialize(this);
        }
        
        public override void Activate()
        {
            base.Activate();

            IInteractionCtr?.Activate();
            
            //EnableNavMeshModifierVolume(true);
        }

        public virtual void Deactivate(bool deactivateTm = false)
        {
            _isActivate = false;

            IInteractionCtr?.Deactivate();

            if (deactivateTm)
            {
                // 일단 바위만
                //if(eItemInteraction == EItemInteraction.Hammer ||
                //   eItemInteraction == EItemInteraction.Pickaxe)
                //    EnableNavMeshModifierVolume(false);

                base.Deactivate();
            }
        }
        
        public void SortingOrder(float order)
        {
            var sortingOrder = Mathf.CeilToInt(order * 100f);
            sortingOrder += orderOffset;
                
#if UNITY_EDITOR
            if (_spriteRenderers == null)
                _spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
#endif
            
            if (_spriteRenderers != null)
            {
                foreach (var spriteRenderer in _spriteRenderers)
                {
                    if(spriteRenderer == null)
                        continue;
                    
                    spriteRenderer.sortingOrder = sortingOrder;
                }
            }
                //_spriteRenderer.sortingOrder = sortingOrder;
            
#if UNITY_EDITOR 
            InitializeSkeletonAnimation();
#endif
            _meshRenderer = SkeletonAnimation?.GetComponent<MeshRenderer>();
            if(_meshRenderer != null)
                _meshRenderer.sortingOrder = sortingOrder;

            _order = sortingOrder;
        }

        //private void EnableNavMeshModifierVolume(bool enable)
        //{
        //    if (_navMeshModifierVolumes == null)
        //        return;
            
        //    foreach (var navMeshModifierVolume in _navMeshModifierVolumes)
        //    {
        //        if(navMeshModifierVolume == null)
        //            continue;
                
        //        navMeshModifierVolume.enabled = enable;
        //    }
        //}
        
        #region IObject

        void IObject.SetPosition(Vector3 position)
        {
           Transform.position = position;
        }
        #endregion
    }
}

