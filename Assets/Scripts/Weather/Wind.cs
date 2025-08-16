using UnityEngine;

using Cysharp.Threading.Tasks;
using Spine.Unity;
using Spine;

using GameSystem;

namespace Weather
{
    public class Wind : Common.Component
    {
        private SkeletonAnimation _skeletonAnimation = null; 
    
        public override void Initialize()
        {
            base.Initialize();

            _skeletonAnimation = GetComponentInChildren<SkeletonAnimation>(true);
            if(_skeletonAnimation != null)
                _skeletonAnimation.AnimationState.Complete += OnComplete;

            ActivateAsync().Forget();
        }

        private void SetRandomPosition()
        {
            var iCameraMgr = Manager.Get<ICameraManager>();
            if (iCameraMgr == null)
                return;

            float width = iCameraMgr.Width;
            float height = iCameraMgr.Height;
            var halfWidth = width * 0.5f;
            var halfHeight = height * 0.5f;
        
            var randomX = Random.Range(iCameraMgr.Center.x - halfWidth, iCameraMgr.Center.x + halfWidth);
            var randomY = Random.Range(iCameraMgr.Center.y - halfHeight, iCameraMgr.Center.y + halfHeight);

            transform.position = new Vector3(randomX, randomY, 0);
        }

        private void OnComplete(TrackEntry trackEntry)
        {
            Deactivate();
            ActivateAsync().Forget();
        }

        private async UniTask ActivateAsync()
        {
            SetRandomPosition();
            await UniTask.Yield();
        
            Activate();
        }
    }
}
    

