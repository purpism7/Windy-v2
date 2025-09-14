using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Cysharp.Threading.Tasks;

using Creator;
using UI.Part;
using Common;
using UI;

namespace GameSystem
{
    public class UIManager : Singleton<UIManager>
    {
        [SerializeField] private Camera uiCmaera = null;
        [SerializeField] private Canvas canvas = null;
        [SerializeField] private RectTransform rootRectTm = null;
        [SerializeField] private RectTransform topRootRectTm = null;
        [SerializeField] private RectTransform rightRootRectTm = null;
        [SerializeField] private RectTransform viewUIRootRectTm = null;
        [SerializeField] private RectTransform worldUIRootRectTm = null;
        
        private List<Common.Component> _cachedUIComponentList = null;
        private Dictionary<System.Type, Common.Component> _componentDic = null;

        private UI.IView _currIView = null;

        public Camera UICamera => uiCmaera;
        public Canvas Canvas => canvas;
        public RectTransform ViewUIRootRectTm => viewUIRootRectTm;
        public RectTransform WorldUIRootRectTm => worldUIRootRectTm;

        // public UI.IView CurrentIView { get; private set; } = null;

        public override async UniTask InitializeAsync()
        {
            _componentDic = new();
            _componentDic.Clear();

            await LoadAssetAsync();
            // await UniTask.Yield();
                
            await CreateInventoryAsync();
            await CreateRecipeAsync();
        }

        private void Update()
        {
            _currIView?.ChainUpdate();
        }

        private async UniTask CreateInventoryAsync()
        {
            await UICreator<Inventory, Inventory.Param>.Get
                .SetRootTm(topRootRectTm)
                .CreateAsync();
            // inventory?.Activate();
        }
        
        private async UniTask CreateRecipeAsync()
        {
            await UICreator<RecipePart, RecipePart.Param>.Get
                .SetRootTm(rightRootRectTm)
                .CreateAsync();
            // recipePart?.Activate();
        }
        
        private async UniTask LoadAssetAsync()
        {
            await AddressableManager.Instance.LoadAssetAsync<GameObject>("UI",
                (result) =>
                {
                    if (result)
                    {
                        var component = result.GetComponent<Common.Component>();
                        if (component == null)
                            return;
                        
                        // Debug.Log(component.name);
                        _componentDic?.TryAdd(component.GetType(), component);
                    }
                });
        }

        public Common.Component Get<T, V>(out bool already, Transform rootTm = null, bool worldUI = false) 
            where T : Common.Component 
            where V :Common.Component<V>.Param
        {
            already = true;
            
            if (_currIView?.GetType() == typeof(T))
                return null;
            
            if (_cachedUIComponentList == null)
            {
                _cachedUIComponentList = new();
                _cachedUIComponentList.Clear();
            }

            Common.Component component = null;
            for (int i = 0; i < _cachedUIComponentList?.Count; ++i)
            { 
                component = _cachedUIComponentList[i];
                if(component == null)
                    continue;
                
                if(component.GetType() != typeof(T))
                    continue;
                
                if (component is UI.BaseView<V> || !component.IsActivate)
                {
                    if(component is UI.IView)
                        _currIView = component as IView;
                    
                    return component as T;
                }
            }
            
            already = false;
            
            if (_componentDic != null)
            {
                _componentDic.TryGetValue(typeof(T), out component);
                if (component == null)
                    return null;
                
                component = Instantiate(component.gameObject).GetComponent<T>();
                if (component != null)
                    _cachedUIComponentList?.Add(component);
            }

            if (!rootTm)
            {
                if (worldUI)
                    rootTm = worldUIRootRectTm;
                else
                    rootTm = rootRectTm;
            }

            component?.transform.SetParent(rootTm);

            if (component is UI.IView iView)
            {
                // _currIView?.Deactivate();
                _currIView = iView;
            }
            
            return component;
        }

        public void ClearCurrIView()
        {
            _currIView = null;
        }
    }
}

