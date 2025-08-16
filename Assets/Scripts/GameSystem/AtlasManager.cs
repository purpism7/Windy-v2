using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.U2D;

using Cysharp.Threading.Tasks;

namespace GameSystem
{
    public class AtlasManager : Common.Singleton<AtlasManager>
    {
        private Dictionary<Common.EAtlasKey, SpriteAtlas> _spriteAtlasDic = null;
        
        public override async UniTask InitializeAsync()
        {
            // DontDestroyOnLoad(this);

            if (_spriteAtlasDic == null)
            {
                _spriteAtlasDic = new();
                _spriteAtlasDic.Clear();
            }
            
            await AddressableManager.Instance.LoadAssetAsync<SpriteAtlas>("Atlas",
                (result) =>
                {
                    if (result)
                    {
                        if(Enum.TryParse(result.name, out Common.EAtlasKey eAtlasKey))
                            _spriteAtlasDic?.TryAdd(eAtlasKey, result);
                        Debug.Log(result.name);
                        // var component = result.GetComponent<UI.Component>();
                        // if (component == null)
                        //     return;
                        //
                        // Debug.Log(component.name);
                        // _componentDic?.TryAdd(component.GetType(), component);
                    }
                });
        }
        
        public Sprite GetSprite(Common.EAtlasKey eAtlasKey, string name)
        {
            if(_spriteAtlasDic.TryGetValue(eAtlasKey, out SpriteAtlas spriteAtlas))
                return spriteAtlas.GetSprite(name);

            return null;
        }
    }
}

