using System;
using UnityEngine;

using Cysharp.Threading.Tasks;
using Spine;
using DG.Tweening;

using Creator;
using GameSystem;
using Table;
using Common;

namespace Creature.Interactions
{
    public class Break : BaseInteraction<Break.Data>
    {
        public new class Data : BaseInteraction<Break.Data>.Data
        {
            public int ObjectId { get; private set; } = 0;
            public Vector3 Position { get; private set; } = Vector3.zero;
            public EItemInteraction EItemInteraction { get; private set; } = EItemInteraction.None;

            public Data WithObjectId(int objectId)
            {
                ObjectId = objectId;
                return this;
            }
            
            public Data WithPosition(Vector3 position)
            {
                Position = position;
                return this;
            }

            public Data WithEItemInteraction(EItemInteraction eItemInteraction)
            {
                EItemInteraction = eItemInteraction;
                return this;
            }
        }

        protected override string AnimationName
        {
            get { return "disappear"; }
        }

        public override void Execute()
        {
            _iInteractable?.Deactivate();
            ExecuteAsync().Forget();
        }

        private async UniTask ExecuteAsync()
        {
            await UniTask.DelayFrame(6);
            _iInteractable?.SkeletonAnimation?.SetAnimation(AnimationName, false, FinishBreak);
            
            await UniTask.DelayFrame(6);
            DropItem();
        }

        private void DropItem()
        {
            if (_data == null)
                return;
            
            var objectData = ObjectDataContainer.Instance?.GetData(_data.ObjectId);
            if (objectData?.DropItemIds == null)
                return;

            var randomIndex = UnityEngine.Random.Range(0, objectData.DropItemIds.Length);
            
            int dropItemId = objectData.DropItemIds[randomIndex];
            var obj = Manager.Get<IObjectManager>()
                ?.CreateItemObject(dropItemId, Manager.Get<IRegion>()?.ItemObjectRootTm, _data.Position);
            
            AnimDropItem(obj?.Transform);   
        }

        private void AnimDropItem(Transform tm)
        {
            if (!tm)
                return;
            
            var endPosition = tm.localPosition;
            endPosition.x += UnityEngine.Random.Range(-1f, 1f);
            endPosition.y += UnityEngine.Random.Range(-0.5f, -0.1f);
            var jumpPower = UnityEngine.Random.Range(0.5f, 1f);
            var duration = UnityEngine.Random.Range(0.2f, 0.6f);
            
            tm.DOLocalJump(endPosition, jumpPower, 1, duration);
        }

        private void FinishBreak(TrackEntry trackEntry)
        {
            _iInteractable?.SkeletonAnimation?.SetAnimation("disappear_still", true);
            _iInteractable?.Deactivate(_data.EItemInteraction != EItemInteraction.Axe);
            
            // if(_data.EItemInteraction != EItemInteraction.Axe)
                // MainManager.Instance?.NavMeshSurface?.BuildNavMesh();
        }
    }
}