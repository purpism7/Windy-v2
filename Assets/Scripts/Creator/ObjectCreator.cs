using System.Collections;
using System.Collections.Generic;
using Creature;
using UnityEngine;

using Cysharp.Threading.Tasks;

using GameSystem;
using Spine.Unity;

namespace Creator
{
    public class ObjectCreator : Common.Creator<ObjectCreator> //where T : Creature.Object
    {
        private int _id = 0;
        private Transform _rootTm = null;
        private Vector3 _position = Vector3.zero;

        public ObjectCreator SetId(int id)
        {
            _id = id;
            return this;
        }
        
        public ObjectCreator SetRoot(Transform rootTm)
        {
            _rootTm = rootTm;
            return this;
        }
        
        public ObjectCreator SetPosition(Vector3 position)
        {
            _position = position;
            return this;
        }
        
        public Creature.IObject Create()
        {
            var obj = AddressableManager.Instance?.LoadObject<Creature.Object>(_id);
            var gameObj = GameObject.Instantiate(obj, _rootTm);
            if (!gameObj)
                return null;

            // (objectAsset as IObject)?.SetPosition(_position);
            // gameObj.Transform.position = _position;
            
            IObject iObject = gameObj.GetComponent<Creature.Object>();
            iObject?.SetPosition(_position);
            // var skeletonDataAsset = await AddressableManager.Instance.LoadAssetByNameAsync<SkeletonDataAsset>("WindySkeletonDataAsset");
            // t?.SetSkeletonDataAsset(skeletonDataAsset);
            
            Debug.Log(iObject);
                
                
            return iObject;
        }
    }
}