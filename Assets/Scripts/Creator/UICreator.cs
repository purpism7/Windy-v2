using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;

using Cysharp.Threading.Tasks;

using GameSystem;

using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;
using System;

namespace Creator
{
    public class UICreator<T, V> : Common.Creator<UICreator<T, V>> where T : Common.Component where V : Common.Component.Param
    {
        private V _param = null;
        private Transform _rootTm = null;
        private bool _worldUI = false;
        private bool _initializeSize = false;
        private Vector3? _scale = Vector3.one;
  
        public UICreator<T, V> SetParam(V param = null) 
        {
            _param = param;
            return this;
        }
        
        public UICreator<T, V> SetWorldUI(bool worldUI)
        {
            _worldUI = worldUI;
            return this;
        }
        
        public UICreator<T, V> SetInitializeSize(bool initializeSize)
        {
            _initializeSize = initializeSize;
            return this;
        }
        
        public UICreator<T, V> SetRootTm(Transform rootTm)
        {
            _rootTm = rootTm;
            return this;
        }

        public UICreator<T, V> SetScale(Vector3 scale)
        {
            _scale = scale;
            return this;
        }
        
        public async UniTask<T> CreateAsync()
        {
            bool already = false;
            var component = UIManager.Instance?.Get<T, V>(out already, rootTm: _rootTm, worldUI: _worldUI) as Common.Component<V>;
            if (component == null)
                return null;
            
            var rectTm = component?.GetComponent<RectTransform>();
            if (rectTm)
            {
                rectTm.anchoredPosition3D = Vector3.zero;
                rectTm.transform.localScale = _scale.Value;
                
                if(_worldUI || _initializeSize)
                    rectTm.sizeDelta = Vector2.zero;
            }

            component?.SetParam(_param);
            
            if (!already)
            {
                await component.InitializeAsync();

                if (component is UI.BaseView<V> baseView)
                    baseView.CreatePresenter();
            }
            
            await component.BeforeActivateAsync();
            component.Activate();

            await UniTask.Yield(PlayerLoopTiming.LastPostLateUpdate);
            await component.AfterActivateAsync();

            return component as T;
        }
    }
}