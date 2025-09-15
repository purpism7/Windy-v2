using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Cysharp.Threading.Tasks;
using DG.Tweening;

using GameSystem;

namespace UI.Part
{
    public class PartWorld<T> : Common.Component<T> where T : PartWorld<T>.Param
    {
        public new class Param : Common.Component.Param
        {
            public Transform TargetTm = null;
            public Vector2 Offset = Vector2.zero;
        }
        
        // protected T _data = null;
        private RectTransform _rectTm = null;
        
        public override async UniTask InitializeAsync()
        {
            // _param = param;
            _rectTm = GetComponent<RectTransform>();

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

        public override void ChainUpdate()
        {
            base.ChainUpdate();
        }

        private void LateUpdate()
        {
            if (!IsActivate)
                return;
            
            ChainLateUpdate();
        }

        public override void ChainLateUpdate()
        {
            base.ChainLateUpdate();
            
            if (!_rectTm)
                return;
        
            if (!_param?.TargetTm)
                return;
        
            var pos = GetScreenPos(_param.TargetTm.position);
            if(pos != null)
                _rectTm.anchoredPosition = pos.Value;
        }
        
        private Vector3? GetScreenPos(Vector3 targetPos)
        {
            var camera = Manager.Get<ICameraManager>()?.MainCamera;
            if (camera == null)
                return null;
            
            var worldUIRootRectTm = UIManager.Instance?.WorldUIRootRectTm;
            if (!worldUIRootRectTm)
                return null;
            
            var uiCamera = UIManager.Instance?.UICamera;
            if (uiCamera == null)
                return null;
            
            var screenPos = camera.WorldToScreenPoint(targetPos);

            Vector2 localPos = Vector2.zero;
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(worldUIRootRectTm, screenPos, uiCamera, out localPos))
            {
                localPos.x += _param.Offset.x;
                localPos.y += _param.Offset.y;
            }
            
            return localPos;
        } 
        
        protected async UniTask AppearEffectAsync()
        {
            // 초기 상태 설정
            transform.localScale = Vector3.zero;

            // 스케일 애니메이션 (DOTween 사용)
            await transform.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutBack).ToUniTask();
        }
    }
}

