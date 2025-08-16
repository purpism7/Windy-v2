using System.Collections;
using System.Collections.Generic;
using Creature;
using UnityEngine;

using Cysharp.Threading.Tasks;

using GameSystem;
using Spine.Unity;

namespace Creator
{
    public class CharacterCreator<T> : Common.Creator<CharacterCreator<T>> where T : Creature.Character
    {
        private int _id = 0;
        private string _key = string.Empty;
        private Transform _rootTm = null;

        public CharacterCreator<T> SetId(int id)
        {
            _id = id;
            return this;
        }

        public CharacterCreator<T> SetKey(string key)
        {
            _key = key;
            return this;
        }
        
        public CharacterCreator<T> SetRoot(Transform rootTm)
        {
            _rootTm = rootTm;
            return this;
        }
        
        public T Create()
        {
            // var character = await Manager.Get<CharacterManager>().LoadAsync<T>(1, _rootTm);
            
            var character = AddressableManager.Instance?.LoadCharacter<T>(_id);
            var gameObj = GameObject.Instantiate(character, _rootTm);
            if (!gameObj)
                return null;
            
            var t = gameObj.GetComponent<T>();
            
            // var skeletonDataAsset = await AddressableManager.Instance.LoadAssetByNameAsync<SkeletonDataAsset>("WindySkeletonDataAsset");
            // t?.SetSkeletonDataAsset(skeletonDataAsset);
            
            Debug.Log(t);
                
                
            return t;
        }
    }
}